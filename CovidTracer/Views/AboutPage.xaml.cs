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
                await Navigation.PushAsync(new DetailsPage(true));
            }
        }

        async void WebsiteClicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/RaphaelJ/covid-tracer");
        }
    }
}