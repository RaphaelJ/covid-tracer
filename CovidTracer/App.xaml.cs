using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CovidTracer.Views;

namespace CovidTracer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CovidTracer.Views.MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
