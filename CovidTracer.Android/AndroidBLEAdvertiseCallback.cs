using Android.Bluetooth.LE;

namespace CovidTracer.Droid
{
    class AndroidBLEAdvertiseCallback : AdvertiseCallback
    {
        public override void OnStartFailure(AdvertiseFailure error)
        {
            base.OnStartFailure(error);
            Logger.Info($"Advertise start failure: {error}");
        }

        public override void OnStartSuccess(AdvertiseSettings settings)
        {
            base.OnStartSuccess(settings);
            Logger.Info($"Advertise start success: {settings.Mode}");
        }
    }
}