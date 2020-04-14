using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

using CovidTracer.Models;
using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.Services
{
    /** Periodically listen for CovidTracer devices and save their tracer key in
     * an SQLite database.
     */
    public /* Singleton */ class TracerService
    {
        const int SCAN_TIMEOUT = 15 * 1000;
        const int SCAN_REPEAT = 60 * 1000;
        const int MIN_RSSI = -85; // Discards devices with a RSSI below

        // Will timeout if one's device tracer key can't be retreived within
        // that delay.
        const int DEVICE_CONNECT_TIMEOUT = 20 * 1000;

        readonly Guid SERVICE_NAME =
            Guid.Parse("22FC7440-9ED6-48B8-85B3-DA69AF417AED");
        readonly Guid CHARACTERISTIC_NAME =
            Guid.Parse("04B9494A-477F-4D46-B9A3-BD06C7E1E5E6");

        readonly IBluetoothLE ble;
        readonly IAdapter adapter;
        readonly IBLEServer server;

        readonly TracerKey key;

        bool started = false;

        static TracerService instance = null;

        static public TracerService GetInstance(IBLEServer server)
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new TracerService(server);
                return instance;
            }
        }

        TracerService(IBLEServer server_)
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            server = server_;

            key = TracerKey.CurrentAppInstance();

            Logger.Info($"TracerService instanced with key '{key.ToString()}'");
        }

        public void Start()
        {
            lock (this) {
                if (!started) {
                    server.AddReadOnlyService(
                        SERVICE_NAME, new Dictionary<Guid, Func<byte[]>> {
                            { CHARACTERISTIC_NAME, HandleReadCharacteristic }
                        }
                    );

                    new Thread(async () => await ScanDevicesAsync()).Start();

                    Logger.Info("TracerService started");
                    started = true;
                }
            }
        }

        /** Returns the current tracer key that should be sent over BLE. */
        protected byte[] HandleReadCharacteristic()
        {
            var now = DateHour.Now;

            // TODO: key generation could be cached instead of being recomputed
            // at every characteristic read.
            HourlyTracerKey currentKey = key
                .DerivateDailyKey(now.AsDate())
                .DerivateHourlyKey(now);

            return currentKey.Value;
        }

        /** Repeatedly scan for nearby Bluetooth devices. */
        protected async Task ScanDevicesAsync()
        {
            ble.StateChanged += (s, e) => {
                Logger.Info($"BLE state changed to {e.NewState}.");
            };

            adapter.ScanTimeout = SCAN_TIMEOUT;
            adapter.ScanMode = ScanMode.LowPower;

            // Devices 
            var lastScanDevices = new List<IDevice>();
            adapter.DeviceDiscovered += (s, e) => {
                lastScanDevices.Add(e.Device);
            };

            for (; ; ) {
                try {
                    if (ble.IsOn) {
                        Logger.Info("Initiate new BLE scan");

                        await adapter.StartScanningForDevicesAsync();
                        await adapter.StopScanningForDevicesAsync();

                        Logger.Info(
                            $"Scan finished, found {lastScanDevices.Count} " +
                            "devices.");

                        // Scan finished, now connect and retreive the
                        // discovered devices.

                        foreach (var device in lastScanDevices) {
                            await DiscoverDevice(device);
                        }

                        lastScanDevices.Clear();
                    } else {
                        Logger.Info("Bluetooth is OFF, skip scan");
                    }
                } catch (Exception e) {
                    Logger.Info(
                        $"BLE scan device exception: '{e.Message}'.");
                }

                Thread.Sleep(SCAN_REPEAT);
            }
        }

        /** Processes a discovery response of a Bluetooth device.
         *
         * Adds the device to the persistent data-store when required.
         */
        protected async Task DiscoverDevice(IDevice device)
        {
            if (device.Rssi < MIN_RSSI) {
                Logger.Info(
                    $"BLE device ignored: {device.Id}/{device.Name} " +
                    $"({device.Rssi} dBm)."
                );
                return;
            }

            Logger.Info(
                $"BLE device discovered: " +
                $"{device.Id}/{device.Name} ({device.Rssi} dBm)"
            );

            // Cancellation token that will abort the discovery process after
            // DEVICE_CONNECT_TIMEOUT.
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(DEVICE_CONNECT_TIMEOUT);
            var token = tokenSource.Token;

            try {
                // Retrives the CovidTracerID from the BLE characteristic.

                if (device.State != DeviceState.Connected) {
                    var connectParams = new ConnectParameters(
                        /* autoConnect = */ false,
                        /* forceBleTransport = */ true);

                    await adapter.ConnectToDeviceAsync(
                        device, connectParams, token);
                }

                if (device.State != DeviceState.Connected) {
                    Logger.Info($"Failed to connect to {device.Id}");
                    return;
                }

                var service = await device.GetServiceAsync(SERVICE_NAME, token);

                if (service == null) {
                    Logger.Info(
                        $"Device {device.Id} does not have CovidTracer " +
                        "service");
                    return;
                }

                var characteristic =
                    await service.GetCharacteristicAsync(CHARACTERISTIC_NAME);

                if (characteristic == null) {
                    Logger.Info(
                        $"Device {device.Id} does not support CovidTracer " +
                        "characteristic.");
                    return;
                }

                var keyBytes = await characteristic.ReadAsync(token);

                if (keyBytes == null) {
                    Logger.Info(
                        $"Device {device.Id} characteristic can not be read.");
                    return;
                }

                var key = new HourlyTracerKey(keyBytes);

                // Logs the successful encounter

                ContactDatabase.GetInstance().NewContact(key);
            } catch (Exception e) {
                Logger.Info($"Bluetooth exception: '{e.Message}'.");
                Console.WriteLine(e.StackTrace);
            } finally {
                tokenSource.Dispose();
            };
        }
    }
}
