using System;
using System.Collections.Generic;
using Android.Bluetooth;

namespace CovidTracer.Droid
{
    /** Answers to BLE server request with the provided characteristic values.
     */
    class AndroidBLEGattServerCallback : BluetoothGattServerCallback
    {
        readonly AndroidBLEServer server;

        public AndroidBLEGattServerCallback(AndroidBLEServer server_)
        {
            server = server_;
        }

        public override void OnConnectionStateChange(
            BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);

            Logger.Info(
                $"BLE device state change: {device.Address} is {newState}.");
        }

        public override void OnCharacteristicReadRequest(
            BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic target)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, target);

            Logger.Info($"BLE characteristic read request for {target.Uuid}.");

            server.server.SendResponse(
                device, requestId, GattStatus.Success, offset,
                target.GetValue());
        }
    }
}
