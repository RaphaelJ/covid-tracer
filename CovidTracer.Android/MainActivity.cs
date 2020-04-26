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

using System.Collections.Generic;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.Content;

using CovidTracer.Services;

namespace CovidTracer.Droid
{
    [Activity(
        Label = "CovidTracer",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize
                               | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity
        : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string[] PERMISSIONS = new string[] {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessCoarseLocation,
        };

        public static TracerService TracerService = null;

        App app;

        ComponentName bluetoothService_ = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Starts the UI

            ToolbarResource = Resource.Layout.Toolbar; // Needed ?
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            if (TracerService == null) {
                var bleServer = new AndroidBLEServer(this.ApplicationContext);
                TracerService = new TracerService(bleServer);
            }

            app = new App(TracerService, StartTracerService);

            LoadApplication(app);
        }

        /** Tries to start the tracer racer service if all permissions are
         * granted.
         *
         * Otherwise, repeatitly ask for the required permissions */
        private void StartTracerService()
        {
            List<string> denied = new List<string>();

            foreach (string perm in PERMISSIONS) {
                var permStatus = ContextCompat.CheckSelfPermission(this, perm);

                if (permStatus == Permission.Denied) {
                    Logger.Info($"Permission '{perm}' denied.");
                    denied.Add(perm);
                }
            }

            if (denied.Count > 0) {
                ActivityCompat.RequestPermissions(this, denied.ToArray(), 0);
            } else if (bluetoothService_ == null) {
                bluetoothService_ = StartService(
                    new Intent(this, typeof(AndroidTracerService)));
            }
        }

        public override void OnRequestPermissionsResult(
            int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(
                requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(
                requestCode, permissions, grantResults);

            StartTracerService();
        }
    }
}