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

        int tracerKeyClicked = 0;
        async void TracerKeyClicked(object sender, EventArgs e)
        {
            // Clicking 10 times on the app ID unlock the debug details".

            ++tracerKeyClicked;

            if (tracerKeyClicked >= 10) {
                var button = (Button)sender;
                button.IsEnabled = false;

                await Navigation.PushAsync(new DebugPage(tracerService));
                button.IsEnabled = true;
            }
        }

        async void WebsiteClicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/RaphaelJ/covid-tracer");
        }
    }
}