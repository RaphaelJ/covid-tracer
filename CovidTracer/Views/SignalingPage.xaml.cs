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

using Xamarin.Forms;

using CovidTracer.ViewModels;

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