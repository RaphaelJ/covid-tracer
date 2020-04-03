using System.ComponentModel;
using CovidTracer.ViewModels;
using Xamarin.Forms;

namespace CovidTracer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = new MainViewModel();

            InitializeComponent();
        }

        void StatusDetailsClicked(System.Object sender, System.EventArgs e)
        {
            var model = (MainViewModel) BindingContext;

            Models.InfectionStatus newStatus;
            switch (model.Status) {
            case Models.InfectionStatus.Safe:
                newStatus = Models.InfectionStatus.Symptomatic;
                break;
            case Models.InfectionStatus.Symptomatic:
                newStatus = Models.InfectionStatus.Positive;
                break;
            case Models.InfectionStatus.Positive:
            default:
                newStatus = Models.InfectionStatus.Safe;
                break;
            }

            model.ChangeStatus(newStatus);
        }

        async void AboutClicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }
    }
}
