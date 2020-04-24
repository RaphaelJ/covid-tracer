using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;
using System.Linq;

namespace CovidTracer.Services
{
    /** Periodically listen for CovidTracer devices and save their tracer key in
     * an SQLite database.
     */
    public class TracerService
    {
        // The duration of a scan.
        const int SCAN_TIMEOUT = 15 * 1000;

        // Delay between two Bluetooth scans.
        const int SCAN_REPEAT = 45 * 1000;

        // Discards devices with a RSSI below that value
        const int MIN_RSSI = -85; 

        // Will timeout if one's device tracer key can't be retreived within
        // that delay.
        const int DEVICE_CONNECT_TIMEOUT = 20 * 1000;

        // Does not try to discover the tracer key of a device a second time
        // until that amount of time.
        const int DISCOVERY_COOLDOWN = 15 * 60 * 1000;

        readonly Guid SERVICE_NAME =
            Guid.Parse("22FC7440-9ED6-48B8-85B3-DA69AF417AED");
        readonly Guid CHARACTERISTIC_NAME =
            Guid.Parse("04B9494A-477F-4D46-B9A3-BD06C7E1E5E6");

        public readonly TracerKey Key;

        public readonly ContactDatabase Contacts;

        readonly IBLEServer server;

        bool started = false;

        public TracerService(IBLEServer server_)
        {
            Key = TracerKey.CurrentAppInstance();

            Contacts = new ContactDatabase();

            server = server_;

            Logger.Info(
                $"TracerService instanced with key " +
                $"'{Key.ToHumanReadableString()}'");
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

            // TODO key generation could be cached instead of being recomputed
            // at every characteristic read.
            HourlyTracerKey currentKey = Key
                .DerivateDailyKey(now.AsDate())
                .DerivateHourlyKey(now);

            Logger.Info($"BLE characteristic read request.");

            return currentKey.Value;
        }

        /** Repeatedly scan for nearby Bluetooth devices. */
        protected async Task ScanDevicesAsync()
        {
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;

            ble.StateChanged += (s, e) => {
                Logger.Info($"BLE state changed to {e.NewState}.");
            };

            adapter.ScanTimeout = SCAN_TIMEOUT;
            adapter.ScanMode = ScanMode.LowPower;

            // Will contain the devices discovered during the asynchonous scan.
            var lastScanDevices = new List<IDevice>();
            adapter.DeviceDiscovered += (s, e) => {
                lastScanDevices.Add(e.Device);
            };

            // Will ignore the devices that have already been discovered less
            // than `DISCOVERY_COOLDOWN` ago.
            var discoveredDevices = new Dictionary<Guid, DateTime>();

            for (; ; ) {
                try {
                    if (ble.IsOn) {
                        Logger.Info("Initiate new BLE scan");

                        await adapter.StartScanningForDevicesAsync();
                        await adapter.StopScanningForDevicesAsync();

                        var now = DateTime.UtcNow;

                        // Removes devices that have been discovered more than
                        // `DISCOVERY_COOLDOWN` ago from `discoveredDevices`.
                        {
                            var expirationDate = now
                                .AddMilliseconds(-DISCOVERY_COOLDOWN);
                            var expired = discoveredDevices
                                .Where(d => d.Value < expirationDate);

                            foreach (var d in expired) {
                                discoveredDevices.Remove(d.Key);
                            }
                        }

                        // Scan finished, now connect and retreive the
                        // discovered devices.

                        {
                            int ignored = 0;

                            foreach (var device in lastScanDevices) {
                                if (device.Rssi < MIN_RSSI) {
                                    ++ignored;
                                    continue;
                                }

                                if (discoveredDevices.ContainsKey(device.Id)) {
                                    ++ignored;
                                    continue;
                                }

                                await DiscoverDevice(adapter, device);

                                discoveredDevices.Add(device.Id, now);
                            }

                            Logger.Info(
                                $"Scan finished, found " +
                                $"{lastScanDevices.Count} devices " +
                                $"(of which {ignored} were ignored)."
                            );

                            lastScanDevices.Clear();
                        }
                    } else {
                        Logger.Warning("Bluetooth is OFF, skip scan");
                    }
                } catch (Exception e) {
                    Logger.Error(
                        $"BLE scan device exception: '{e.Message}'.");
                }

                Thread.Sleep(SCAN_REPEAT);
            }
        }

        /** Processes a discovery response of a Bluetooth device.
         *
         * Adds the device to the persistent data-store when required.
         */
        protected async Task DiscoverDevice(IAdapter adapter, IDevice device)
        {
            var deviceIdStr = FormatDeviceId(device.Id);

            Logger.Info(
                $"Discovering BLE device: " +
                $"{deviceIdStr}/{device.Name} ({device.Rssi} dBm)"
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
                    Logger.Warning($"Failed to connect to {deviceIdStr}");
                    return;
                }

                var service = await device.GetServiceAsync(SERVICE_NAME, token);

                if (service == null) {
                    Logger.Info(
                        $"Device {deviceIdStr} does not have CovidTracer " +
                        "service");
                    return;
                }

                var characteristic =
                    await service.GetCharacteristicAsync(CHARACTERISTIC_NAME);

                if (characteristic == null) {
                    Logger.Info(
                        $"Device {deviceIdStr} does not support CovidTracer " +
                        "characteristic.");
                    return;
                }

                var keyBytes = await characteristic.ReadAsync(token);

                if (keyBytes == null) {
                    Logger.Error(
                        $"Device {deviceIdStr} characteristic can not be read."
                    );
                    return;
                }

                var key = new HourlyTracerKey(keyBytes);

                // Logs the successful encounter

                Contacts.NewContact(key);
            } catch (Exception e) {
                Logger.Error($"Bluetooth exception: '{e.Message}'.");
            } finally {
                tokenSource.Dispose();
                device.Dispose();
            };
        }

        /** Formats the Mac address of a Bluetooth device as an hexadecimal
         * string. */
        static private string FormatDeviceId(Guid deviceId)
        {
            var idBytes = deviceId.ToByteArray().Skip(10).ToArray();
            return Misc.Hex.ToHumanReadableString(idBytes, 2, ":");
        }
    }
}
