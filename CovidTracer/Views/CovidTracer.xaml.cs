using System.ComponentModel;
using CovidTracer.ViewModels;
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
            BindingContext = new CovidTracerViewModel();

            InitializeComponent();
        }

        void StatusDetailsClicked(System.Object sender, System.EventArgs e)
        {
            var model = (CovidTracerViewModel) BindingContext;

            Models.InfectionStatus newStatus;
            switch (model.Status) {
            case Models.InfectionStatus.Safe:
                newStatus = Models.InfectionStatus.Symptomatic;
                break;
            case Models.InfectionStatus.Symptomatic:
                newStatus = Models.InfectionStatus.Positive;
                break;
            default:
                newStatus = Models.InfectionStatus.Safe;
                break;
            }

            model.ChangeStatus(newStatus);
        }
    }
}
