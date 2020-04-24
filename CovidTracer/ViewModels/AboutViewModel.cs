using CovidTracer.Models.Keys;
using CovidTracer.Services;

namespace CovidTracer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        TracerService tracerService;

        string tracerKey;
        public string TracerKey
        {
            get { return tracerKey; }
            set { SetProperty(ref tracerKey, value); }
        }

        public AboutViewModel(TracerService tracerService_)
        {
            tracerService = tracerService_;

            Title = "Informations";

            TracerKey = tracerService.Key.ToHumanReadableString();
        }
    }
}