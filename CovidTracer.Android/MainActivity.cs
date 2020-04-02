using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Android;
using Android.Support.V4.Content;
using System.Collections.Generic;

namespace CovidTracer.Droid
{
    [Activity(Label = "CovidTracer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string[] PERMISSIONS = new string[] {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessCoarseLocation,
        };

        ComponentName bluetoothService_ = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            startBluetoothService();

            // Starts the UI

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        /** Tries to start the Bluetooth service if all permissions are granted.
         *
         * Otherwise, repeatitly ask for the required permissions */
        private void startBluetoothService()
        {
            List<string> denied = new List<string>();

            foreach (string perm in PERMISSIONS) {
                var permStatus = ContextCompat.CheckSelfPermission(this, perm);

                Logger.write($"Permission '{perm}': {permStatus}");

                if (permStatus == Permission.Denied) {
                    denied.Add(perm);
                }
            }

            if (denied.Count > 0) {
                ActivityCompat.RequestPermissions(this, denied.ToArray(), 0);
            } else if (bluetoothService_ == null) {
                bluetoothService_ =
                    StartService(new Intent(this, typeof(BluetoothService)));
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            startBluetoothService();
        }
    }
}