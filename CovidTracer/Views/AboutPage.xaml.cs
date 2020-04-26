// Copyright 2020 Raphael Javaux
//
// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

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