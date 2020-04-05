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
            model.Tested = !model.Tested;
        }
    }
}