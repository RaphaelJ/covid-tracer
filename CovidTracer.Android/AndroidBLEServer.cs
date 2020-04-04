using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using CovidTracer.Models;

namespace CovidTracer.Droid
{
    public class AndroidBLEServer : Services.IBLEServer

    {
        public readonly BluetoothManager manager;
        public readonly BluetoothAdapter adapter;
        public readonly BluetoothGattServer server;


        public AndroidBLEServer(Context context_)
        {
            manager = (BluetoothManager) context_.GetSystemService(
                Context.BluetoothService);
            adapter = manager.Adapter;

            server = manager.OpenGattServer(context_,
                new AndroidBLEGattServerCallback(this));
        }

        public void AddReadOnlyService(
            Guid serviceName, Dictionary<Guid, byte[]> characteristics)
        {
            // TODO: restart the server when the BLE adapter goes down.

            // Creates a Gatt server with the requested service

            {
                var service = new BluetoothGattService(AsJavaUUID(serviceName),
                    GattServiceType.Primary);

                foreach (var i in characteristics) {
                    var ch = new BluetoothGattCharacteristic(AsJavaUUID(i.Key),
                        GattProperty.Read | GattProperty.Notify,
                        GattPermission.Read);

                    ch.SetValue(i.Value);

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

        static ParcelUuid AsParcelUuid(Guid guid)
        {
            return ParcelUuid.FromString(guid.ToString());
        }
    }
}
