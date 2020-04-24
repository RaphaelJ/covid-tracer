using System;
using System.Collections.Generic;
using Android.Bluetooth;

namespace CovidTracer.Droid
{
    /** Answers to BLE server request with the provided characteristic values.
     */
    class AndroidBLEGattServerCallback : BluetoothGattServerCallback
    {
        readonly Dictionary<Guid, Func<byte[]>> characteristics;

        public BluetoothGattServer Server { get; set; }

        public AndroidBLEGattServerCallback(
            Dictionary<Guid, Func<byte[]>> characteristics_)
        {
            characteristics = characteristics_;
        }

        public override void OnConnectionStateChange(
            BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
        }

        public override void OnCharacteristicReadRequest(
            BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic target)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, target);

            var guid = AsGUID(target.Uuid);

            if (characteristics.ContainsKey(guid)) {
                var value = characteristics[guid]();

                target.SetValue(value);

                Server.SendResponse(device, requestId, GattStatus.Success,
                    offset, value);
            }
        }

        static Guid AsGUID(Java.Util.UUID uuid)
        {
            return Guid.Parse(uuid.ToString());
        }
    }
}
