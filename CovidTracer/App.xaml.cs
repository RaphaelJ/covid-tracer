using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CovidTracer.Views;
using Xamarin.Essentials;

namespace CovidTracer
{
    public partial class App : Application
    {
        const string ONBOARDING_DONE_KEY = "onboarding_done";

        /** Starts the CovidTracer mobile app.
         *
         * Accepts an action that will start the `TracerService` in the platform
         * specific background-process system.
         */
        public App(Action startTracerService)
        {
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

        protected void StartApp(Action startTracerService)
        {
            startTracerService();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
