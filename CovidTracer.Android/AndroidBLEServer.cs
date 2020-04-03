using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;

namespace CovidTracer.Droid
{
    public class AndroidBLEServer : Services.IBLEServer

    {
        readonly Context context;

        public AndroidBLEServer(Context context_)
        {
            context = context_;
        }

        public void AddReadOnlyService(
            Guid serviceName, Dictionary<Guid, byte[]> characteristics)
        {
            var manager = (BluetoothManager) context.GetSystemService(
                Context.BluetoothService);
            var adapter = manager.Adapter;

            // Register for system Bluetooth events
            //IntentFilter filter = new IntentFilter(BluetoothAdapter.ActionStateChanged);
            //registerReceiver(mBluetoothReceiver, filter);

            //if (!bluetoothAdapter.isEnabled()) {
            //    Log.d(TAG, "Bluetooth is currently disabled...enabling");
            //    bluetoothAdapter.enable();
            //} else {
            //adapter.enable();

            // Create a Gatt server with the requested service

            {
                var server = manager.OpenGattServer(
                    context, new AndroidBLEGattServerCallback(characteristics));

                var service = new BluetoothGattService(AsJavaUUID(serviceName),
                    GattServiceType.Primary);

                foreach (var i in characteristics) {
                    var ch = new BluetoothGattCharacteristic(AsJavaUUID(i.Key),
                        GattProperty.Read, GattPermission.Read);

                    service.AddCharacteristic(ch);
                }

                server.AddService(service);
            }

            // Starts advertising for the service over BLE

            {
                var advertiser = adapter.BluetoothLeAdvertiser;

                var setts = new AdvertiseSettings.Builder()
                    .SetAdvertiseMode(AdvertiseMode.LowPower)
                    .SetConnectable(true)
                    .SetTimeout(0)
                    .SetTxPowerLevel(AdvertiseTx.PowerLow)
                    .Build();

                var data = new AdvertiseData.Builder()
                    .SetIncludeDeviceName(true)
                    .SetIncludeTxPowerLevel(false)
                    .AddServiceUuid(AsParcelUUID(serviceName))
                    .Build();

                var callback = new AndroidBLEAdvertiseCallback();

                advertiser.StartAdvertising(setts, data, callback);
            }
        }

        static Java.Util.UUID AsJavaUUID(Guid guid)
        {
            return Java.Util.UUID.FromString(guid.ToString());
        }

        static ParcelUuid AsParcelUUID(Guid guid)
        {
            return ParcelUuid.FromString(guid.ToString());
        }
    }
}
