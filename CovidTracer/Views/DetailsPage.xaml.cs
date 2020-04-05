using System.ComponentModel;
using CovidTracer.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(bool isDebug)
        {
            BindingContext = new DetailsViewModel(isDebug);

            InitializeComponent();
        }
    }
}