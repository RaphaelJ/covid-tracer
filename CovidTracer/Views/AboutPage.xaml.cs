using System.ComponentModel;
using CovidTracer.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            BindingContext = new AboutViewModel();

            InitializeComponent();
        }

        async void OnWebsiteClicked(System.Object sender, System.EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/RaphaelJ/covid-tracer");
        }
    }
}