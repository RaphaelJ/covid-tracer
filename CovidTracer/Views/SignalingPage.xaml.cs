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

using CovidTracer.ViewModels;
using System.Threading.Tasks;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class SignalingPage : ContentPage
    {
        const string SIGNALING_DONE_PREFERENCE_KEY = "signaling_done";

        public SignalingPage()
        {
            BindingContext = new SignalingViewModel();

            InitializeComponent();
        }

        void TestedViewCellTapped(object sender, EventArgs e)
        {
            var model = (SignalingViewModel)BindingContext;
            model.IsTested = !model.IsTested;
        }

        async void SendButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            if (Preferences.Get(SIGNALING_DONE_PREFERENCE_KEY, false)) {
                // Prevents the user to send two reports.
                await DisplayAlert(
                    Resx.Localization.SignalingFailureTitle,
                    Resx.Localization.SignalingAlreadyDoneText,
                    Resx.Localization.SignalingFailureContinue);
            } else {
                // Request user confirmation before submitting
                bool confirm = await DisplayAlert(
                    Resx.Localization.SignalingFormConfirmTitle,
                    Resx.Localization.SignalingFormConfirmText,
                    Resx.Localization.SignalingFormConfirmYes,
                    Resx.Localization.SignalingFormConfirmNo);

                if (confirm) {
                    var vm = (SignalingViewModel)BindingContext;

                    var result = await Services.RestService.Notify(
                        vm.SymptomsOnset, vm.IsTested, vm.Comment);

                    if (result.IsSuccessStatusCode) {
                        await DisplayAlert(
                            Resx.Localization.SignalingSuccessTitle,
                            Resx.Localization.SignalingSuccessText,
                            Resx.Localization.SignalingSuccessContinue);

                        Logger.Info(
                            $"Case signaling succeeded with status code " +
                            $"'{result.StatusCode}'.");

                        Preferences.Set(SIGNALING_DONE_PREFERENCE_KEY, true);

                        await Navigation.PopAsync();
                    } else {
                        Logger.Error(
                            $"Case signaling failed with status code " +
                            $"'{result.StatusCode}'.");

                        await DisplayAlert(
                            Resx.Localization.SignalingFailureTitle,
                            Resx.Localization.SignalingFailureText,
                            Resx.Localization.SignalingFailureContinue);
                    }
                }
            }

            button.IsEnabled = true;
        }
    }
}