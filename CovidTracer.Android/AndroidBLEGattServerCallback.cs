using System;
using System.Collections.Generic;
using Android.Bluetooth;

namespace CovidTracer.Droid
{
    /** Answers to BLE server request with the provided characteristic values.
     */
    class AndroidBLEGattServerCallback : BluetoothGattServerCallback
    {
        readonly Dictionary<Guid, byte[]> characteristics;

        public AndroidBLEGattServerCallback(
            Dictionary<Guid, byte[]> characteristics_)
        {
            characteristics = characteristics_;
        }

        public override void OnCharacteristicReadRequest(
            BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic dest)
        {
            Logger.write($"BLE read request for {dest.Uuid}");
            dest.SetValue(characteristics[AsGuid(dest.Uuid)]);
        }

        static Guid AsGuid(Java.Util.UUID uuid)
        {
            return Guid.Parse(uuid.ToString());
        }
    }
}
