using System;
using System.Threading;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace CovidTracer
{
    /** Periodically listen for Bluetooth LE devices and save them in an SQLite
     * database.
     */
    public class BluetoothTracer
    {
        const int SCAN_TIMEOUT = 10 * 1000;
        const int SCAN_REPEAT = 10 * 1000;
        const int MIN_RSSI = -85; // Discards signals with a RSSI below

        IBluetoothLE ble_;
        IAdapter adapter_;

        public BluetoothTracer()
        {
            ble_ = CrossBluetoothLE.Current;
            adapter_ = CrossBluetoothLE.Current.Adapter;
        }

        public void Start()
        {
            new Thread(() => ScanDevices()).Start();
        }

        /** Repeatedly do Bluetooth scans for nearby devices. */
        protected void ScanDevices()
        {
            ble_.StateChanged += (s, e) => {
                Logger.write($"Bluetooth state changed to {e.NewState}.");
            };

            adapter_.DeviceDiscovered += (s, e) => {
                var device = e.Device;

                if (device.Rssi >= MIN_RSSI) {
                    Logger.write(
                        $"Bluetooth device discovered: " +
                        $"{device.Id}/{device.Name} ({device.Rssi} dBm)"
                    );
                }
            };

            adapter_.ScanTimeout = SCAN_TIMEOUT;
            adapter_.ScanMode = ScanMode.LowPower;

            for (; ; ) {
                Logger.write("Initiate new Bluetooth scan");

                try {
                    adapter_.StartScanningForDevicesAsync();
                } catch (Exception e) {
                    Logger.write($"Bluetooth exception: '{e.Message}'.");
                }
 

                Thread.Sleep(SCAN_REPEAT);
            }
        }
    }
}
