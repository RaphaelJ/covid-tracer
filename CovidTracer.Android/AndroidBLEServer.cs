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
using System.Threading;

using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;

namespace CovidTracer.Droid
{
    public class AndroidBLEServer : Services.IBLEServer
    {
        public readonly Context context;

        public AndroidBLEServer(Context context_)
        {
            context = context_;
        }

        public void AddReadOnlyService(
            Guid serviceName, Dictionary<Guid, Func<byte[]>> characteristics)
        {
            var manager = (BluetoothManager)context.GetSystemService(
                Context.BluetoothService);
            var adapter = manager.Adapter;

            if (!adapter.IsEnabled) {
                // TODO: restarts the server when the BLE adapter goes down.
                adapter.Enable();
                Thread.Sleep(10 * 1000);
            }

            // Creates a Gatt server with the requested service

            {

                var callback = new AndroidBLEGattServerCallback(
                    characteristics);

                var server = manager.OpenGattServer(context, callback);

                callback.Server = server;

                var service = new BluetoothGattService(AsJavaUUID(serviceName),
                    GattServiceType.Primary);

                foreach (var i in characteristics) {
                    var ch = new BluetoothGattCharacteristic(AsJavaUUID(i.Key),
                        GattProperty.Read | GattProperty.Notify,
                        GattPermission.Read);

                    service.AddCharacteristic(ch);
                }

                server.AddService(service);
            }

            // Starts advertising for the availaible services over BLE.

            {
                var advertiser = adapter.BluetoothLeAdvertiser;

                var setts = new AdvertiseSettings.Builder()
                    .SetAdvertiseMode(AdvertiseMode.Balanced)
                    .SetConnectable(true)
                    .SetTimeout(0)
                    .SetTxPowerLevel(AdvertiseTx.PowerMedium)
                    .Build();

                var data = new AdvertiseData.Builder()
                    //.AddServiceUuid(AsParcelUuid(ser))
                    .Build();

                var callback = new AndroidBLEAdvertiseCallback();

                advertiser.StartAdvertising(setts, data, callback);
            }
        }

        static Java.Util.UUID AsJavaUUID(Guid guid)
        {
            return Java.Util.UUID.FromString(guid.ToString());
        }

        //static ParcelUuid AsParcelUuid(Guid guid)
        //{
        //    return ParcelUuid.FromString(guid.ToString());
        //}
    }
}
