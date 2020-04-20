using CovidTracer.Models.Keys;
using CovidTracer.Services;

namespace CovidTracer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        TracerService tracerService;

        string appId;
        public string AppId
        {
            get { return appId; }
            set { SetProperty(ref appId, value); }
        }

        bool showDebug;
        public bool ShowDebug
        {
            get { return showDebug; }
            set { SetProperty(ref showDebug, value); }
        }

        string debug;
        public string Debug
        {
            get { return debug; }
            set { SetProperty(ref debug, value); }
        }


        public AboutViewModel(TracerService tracerService_)
        {
            tracerService = tracerService_;

            Title = "Informations";

            AppId = tracerService.Key.ToHumanReadableString();
        }

        /** This will show debug information about the tracer state. */
        public void EnableDebug()
        {
            var stats = tracerService.Contacts.GetStats();

            Debug =
                $"Contact count: {stats["contacts.Count"]} - " +
                $"Case count: {stats["cases.Count"]}";

            ShowDebug = true;
        }
    }
}