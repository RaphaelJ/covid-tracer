using System;
using System.Collections.Generic;
using System.Threading;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace CovidTracer.Services
{
    /** Periodically listen for Bluetooth LE devices and save them in an SQLite
     * database.
     */
    public /* Singleton */ class CovidTracerService
    {
        const int SCAN_TIMEOUT = 10 * 1000;
        const int SCAN_REPEAT = 10 * 1000;
        const int MIN_RSSI = -85; // Discards devices with a RSSI below

        readonly Guid SERVICE_NAME =
            Guid.Parse("22FC7440-9ED6-48B8-85B3-DA69AF417AED");
        readonly Guid CHARACTERISTIC_NAME =
            Guid.Parse("04B9494A-477F-4D46-B9A3-BD06C7E1E5E7");

        readonly IBluetoothLE ble;
        readonly IAdapter adapter;
        readonly IBLEServer bleServer;

        bool started = false;

        static private CovidTracerService instance = null;

        static public CovidTracerService getInstance(IBLEServer bleServer)
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new CovidTracerService(bleServer);
                return instance;
            }
        }

        private CovidTracerService(IBLEServer bleServer_)
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            bleServer = bleServer_;
        }

        public void Start()
        {
            lock (this) {
                if (!started) {
                    bleServer.AddReadOnlyService(
                        SERVICE_NAME, new Dictionary<Guid, byte[]> {
                            {
                                CHARACTERISTIC_NAME,
                                new byte[] { 1, 2, 3, 4 }
                            }
                        }
                    );

                    new Thread(() => ScanDevices()).Start();

                    Logger.write("CovidTracerService started");
                    started = true;
                }
            }
        }

        /** Repeatedly scan for nearby Bluetooth devices. */
        protected void ScanDevices()
        {
            ble.StateChanged += (s, e) => {
                Logger.write($"Bluetooth state changed to {e.NewState}.");
            };

            adapter.DeviceDiscovered += (s, e) => {
                DiscoverDevice(e.Device);
            };

            adapter.ScanTimeout = SCAN_TIMEOUT;
            adapter.ScanMode = ScanMode.LowPower;

            for (; ; ) {
                Logger.write("Initiate new Bluetooth scan");

                try {
                    adapter.StartScanningForDevicesAsync();
                } catch (Exception e) {
                    Logger.write($"Bluetooth exception: '{e.Message}'.");
                }
 

                Thread.Sleep(SCAN_REPEAT);
            }
        }

        /** Processes a discovery response of a Bluetooth device.
         *
         * Adds the device to the persistent data-store when required.
         */
        protected async void DiscoverDevice(IDevice device)
        {
            if (device.Rssi < MIN_RSSI) {
                return;
            }

            Logger.write(
                $"Bluetooth device discovered: " +
                $"{device.Id}/{device.Name} ({device.Rssi} dBm)"
            );

            try {
                await adapter.ConnectToDeviceAsync(device);
                var services = await device.GetServicesAsync();

                Logger.write($"{services.Count} services discovered.");

                foreach (var service in services) {
                    Logger.write(
                        $"Service name: {service.Name} - Is primary: {service.IsPrimary}"
                    );

                    var characteristics = await service.GetCharacteristicsAsync();

                    Logger.write($"{characteristics.Count} characteristics.");

                    foreach (var charact in characteristics) {
                        Logger.write($"{charact.Name}");
                    }
                }
            } catch (Exception e) {
                Logger.write($"Bluetooth exception: '{e.Message}'.");
            }
        }
    }
}
