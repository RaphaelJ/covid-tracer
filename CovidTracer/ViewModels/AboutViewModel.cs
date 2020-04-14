using CovidTracer.Models.Keys;

namespace CovidTracer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        string appId;
        public string AppId
        {
            get { return appId; }
            set { SetProperty(ref appId, value); }
        }

        public AboutViewModel()
        {
            Title = "Informations";

            AppId = TracerKey.CurrentAppInstance().ToHumanReadableString();
        }
    }
}