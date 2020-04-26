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

using Xamarin.Forms;
using Xamarin.Essentials;

using CovidTracer.Services;
using CovidTracer.Views;

namespace CovidTracer
{
    public partial class App : Application
    {
        const string ONBOARDING_DONE_KEY = "onboarding_done";

        public readonly TracerService TracerService;

        /** Starts the CovidTracer mobile app.
         *
         * Accepts an action that will be called by the app when the
         * `TracerService` instance shall be started in the platform specific
         * background-process system (by calling the `Start()` method).
         * E.g. on Android, the action will call `TracerService.Start()` in a
         * `Service` process.
         */
        public App(TracerService tracerService_, Action startTracerService)
        {
            TracerService = tracerService_;

            InitializeComponent();

            // Opens the onboarding page if it's the first time the app is
            // opened. Starts the tracer service only if the user went through
            // the onboarding process, as this will trigger permission requests.

            if (Preferences.Get(ONBOARDING_DONE_KEY, false)) {
                StartApp(startTracerService);
            } else {
                MainPage = new OnboardingPage(() => {
                    Preferences.Set(ONBOARDING_DONE_KEY, true);
                    StartApp(startTracerService);
                });
            }
        }

        /** Starts the tracer background service and shows the main page. */
        protected void StartApp(Action startTracerService)
        {
            startTracerService();
            MainPage = new NavigationPage(new MainPage(TracerService));
        }
    }
}
