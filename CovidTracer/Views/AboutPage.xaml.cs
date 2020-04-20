using System;
using System.ComponentModel;

using Xamarin.Essentials;
using Xamarin.Forms;

using CovidTracer.Services;
using CovidTracer.ViewModels;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        TracerService tracerService;

        public AboutPage(TracerService tracerService_)
        {
            tracerService = tracerService_;

            BindingContext = new AboutViewModel(tracerService);

            InitializeComponent();
        }

        int appIdClicked = 0;
        async void AppIdClicked(object sender, EventArgs e)
        {
            // Clicking 10 times on the app ID unlock the debug details".

            ++appIdClicked;

            if (appIdClicked >= 10) {
                ((AboutViewModel)BindingContext).EnableDebug();
            }
        }

        async void WebsiteClicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/RaphaelJ/covid-tracer");
        }
    }
}