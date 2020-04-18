using System;
using Xamarin.Forms;

using CovidTracer.ViewModels;

namespace CovidTracer.Views
{
    public partial class OnboardingPage : ContentPage
    {
        readonly Action OnLastPane;

        public OnboardingPage(Action onLastPane_)
        {
            BindingContext = new OnboardingViewModel();

            OnLastPane = onLastPane_;

            InitializeComponent();
        }

        void ContinueClicked(object sender, EventArgs e)
        {
            var vm = (OnboardingViewModel)BindingContext;
            if (vm.IsLastPane()) {
                OnLastPane();
            } else {
                vm.NextPane();
            }
        }
    }
}
