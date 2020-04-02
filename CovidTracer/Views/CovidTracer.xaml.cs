using System.ComponentModel;
using Xamarin.Forms;

namespace CovidTracer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class CovidTracer : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        public CovidTracer()
        {
            InitializeComponent();
        }
    }
}
