using System;
using System.ComponentModel;
using CovidTracer.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class SignalingPage : ContentPage
    {
        public SignalingPage()
        {
            BindingContext = new SignalingViewModel();

            InitializeComponent();
        }

        void TestedViewVellTapped(object sender, EventArgs e)
        {
            var model = (SignalingViewModel)BindingContext;
            model.IsTested = !model.IsTested;
        }

        async void SendButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            var model = (SignalingViewModel)BindingContext;

            var result = Services.RestService.Notify(
                model.SymptomsOnset, model.IsTested, model.Comment);

            Logger.Info(result.ToString());

            button.IsEnabled = true;
        }
    }
}