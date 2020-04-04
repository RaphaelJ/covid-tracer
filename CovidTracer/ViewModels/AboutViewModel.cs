using CovidTracer.Models;

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

            AppId = CovidTracerID.GetCurrentInstance().ToHumanReadableString();
        }
    }
}