using System.ComponentModel;

using Xamarin.Forms;

using CovidTracer.ViewModels;
using CovidTracer.Services;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(TracerService tracerService, bool isDebug)
        {
            BindingContext = new DetailsViewModel(tracerService.Contacts);

            InitializeComponent();
        }
    }
}