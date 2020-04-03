using Android.App;
using Android.Content;
using Android.OS;
using CovidTracer.Services;

namespace CovidTracer.Droid
{
    [Service]
    public class AndroidCovidTracerService : Service
    {
        public AndroidCovidTracerService()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(
            Intent intent, StartCommandFlags flags, int startId)
        {
            var bleServer = new AndroidBLEServer(this.ApplicationContext);

            var tracer = CovidTracerService.getInstance(bleServer);
            tracer.Start();

            return StartCommandResult.Sticky;
        }
    }
}
