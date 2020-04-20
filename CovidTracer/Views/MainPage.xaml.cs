using System;
using System.ComponentModel;

using Xamarin.Forms;

using CovidTracer.Services;
using CovidTracer.ViewModels;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        TracerService tracerService;

        public MainPage(TracerService tracerService_)
        {
            tracerService = tracerService_;

            BindingContext = new MainViewModel(tracerService.Contacts);

            InitializeComponent();
        }

        async void StatusDetailsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            await Navigation.PushAsync(new DetailsPage(tracerService, false));
            button.IsEnabled = true;
        }

        async void AboutClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            await Navigation.PushAsync(new AboutPage(tracerService));
            button.IsEnabled = true;
        }

        async void SignalingClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            await Navigation.PushAsync(new SignalingPage());
            button.IsEnabled = true;
        }
    }
}
