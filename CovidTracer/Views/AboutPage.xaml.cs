using System;
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

        int appIdClicked = 0;
        async void AppIdClicked(object sender, EventArgs e)
        {
            // Clicking 10 times on the app ID unlock the debug detail page.

            ++appIdClicked;

            if (appIdClicked >= 10) {
                var button = (Button)sender;
                button.IsEnabled = false;
                await Navigation.PushAsync(new DetailsPage(true));
                button.IsEnabled = true;
            }
        }

        async void WebsiteClicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/RaphaelJ/covid-tracer");
        }
    }
}