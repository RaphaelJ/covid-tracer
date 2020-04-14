using System;
using System.Collections.Generic;
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
            // TODO: restart the server when the BLE adapter goes down.

            var manager = (BluetoothManager)context.GetSystemService(
                Context.BluetoothService);
            var adapter = manager.Adapter;

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
