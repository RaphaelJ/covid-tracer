
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
            var tracer = CovidTracerService.getInstance();
            tracer.Start();

            return StartCommandResult.Sticky;
        }
    }
}
