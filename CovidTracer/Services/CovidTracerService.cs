using System;
using System.Threading;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace CovidTracer
{
    /** Periodically listen for Bluetooth LE devices and save them in an SQLite
     * database.
     */
    public /* Singleton */ class CovidTracerService
    {
        const int SCAN_TIMEOUT = 10 * 1000;
        const int SCAN_REPEAT = 10 * 1000;
        const int MIN_RSSI = -85; // Discards devices with a RSSI below

        readonly IBluetoothLE ble;
        readonly IAdapter adapter;

        bool started = false;

        static private CovidTracerService instance = null;

        static public CovidTracerService getInstance()
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new CovidTracerService();
                return instance;
            }
        }

        private CovidTracerService()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
        }

        public void Start()
        {
            lock (this) {
                if (!started) {
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
                var device = e.Device;

                if (device.Rssi >= MIN_RSSI) {
                    Logger.write(
                        $"Bluetooth device discovered: " +
                        $"{device.Id}/{device.Name} ({device.Rssi} dBm)"
                    );
                }
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
    }
}
