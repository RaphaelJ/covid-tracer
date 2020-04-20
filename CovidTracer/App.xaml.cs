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
