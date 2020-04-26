// Copyright 2020 Raphael Javaux
//
// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

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
