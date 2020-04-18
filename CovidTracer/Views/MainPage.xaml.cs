﻿using System;
using System.ComponentModel;

using Xamarin.Forms;

using CovidTracer.ViewModels;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = new MainViewModel();

            InitializeComponent();
        }

        async void StatusDetailsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            var model = (MainViewModel) BindingContext;

            Models.InfectionStatus newStatus;
            switch (model.Status) {
            case Models.InfectionStatus.Safe:
                newStatus = Models.InfectionStatus.Symptomatic;
                break;
            case Models.InfectionStatus.Symptomatic:
                newStatus = Models.InfectionStatus.Positive;
                break;
            case Models.InfectionStatus.Positive:
            default:
                newStatus = Models.InfectionStatus.Safe;
                break;
            }

            model.ChangeStatus(newStatus);

            await Navigation.PushAsync(new DetailsPage(false));
            button.IsEnabled = true;
        }

        async void AboutClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            await Navigation.PushAsync(new AboutPage());
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
