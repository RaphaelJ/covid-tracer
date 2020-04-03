using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Informations";
            //OpenWebSiteCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
        }
    }
}