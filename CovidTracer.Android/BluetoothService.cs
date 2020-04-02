
using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;

namespace CovidTracer
{
    [Service]
    public class BluetoothService : Service
    {
        public BluetoothService()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(
            Intent intent, StartCommandFlags flags, int startId)
        {
            Logger.write("BluetoothService started");

            var tracer = new BluetoothTracer();
            tracer.Start();

            return StartCommandResult.Sticky;
        }
    }
}
