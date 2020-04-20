using CovidTracer.Services;

namespace CovidTracer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //
        // Infection status
        //

        ContactDatabase.InfectionStatus status;
        public ContactDatabase.InfectionStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        string statusTitle;
        public string StatusTitle
        {
            get { return statusTitle; }
            set { SetProperty(ref statusTitle, value); }
        }

        string statusText;
        public string StatusText
        {
            get { return statusText; }
            set { SetProperty(ref statusText, value); }
        }

        string statusTextColor;
        public string StatusTextColor
        {
            get { return statusTextColor; }
            set { SetProperty(ref statusTextColor, value); }
        }

        string statusBackgroundColor;
        public string StatusBackgroundColor
        {
            get { return statusBackgroundColor; }
            set { SetProperty(ref statusBackgroundColor, value); }
        }

        // --

        public MainViewModel(ContactDatabase contacts)
        {
            Title = "CovidTracer";

            OnInfectionStatusChange(this, contacts.CurrentInfectionStatus);
            contacts.CurrentInfectionStatusChange += OnInfectionStatusChange;
        }

        public void OnInfectionStatusChange(object sender,
            ContactDatabase.InfectionStatus newStatus)
        {
            Status = newStatus;

            switch (newStatus) {
            case ContactDatabase.InfectionStatus.Safe:
                StatusTitle = "Rien à signaler";
                StatusText = "Aucune interaction avec une personne " +
                             "infectée n'a été détectée.";
                StatusTextColor = "#313a33";
                StatusBackgroundColor = "#d4edda";
                break;
            case ContactDatabase.InfectionStatus.Symptomatic:
                StatusTitle = "Risque modéré";
                StatusText = "Vous avez été en contact rapproché avec " +
                             "un ou plusieurs cas suspectés d'infection " +
                             "au COVID-19.";
                StatusTextColor = "#856404";
                StatusBackgroundColor = "#fff3cd";
                break;
            case ContactDatabase.InfectionStatus.Positive:
                StatusTitle = "Risque important";
                StatusText = "Vous avez été en contact avec un ou plusieurs " +
                             "cas COVID-19 déclarés.";
                StatusTextColor = "#721c24";
                StatusBackgroundColor = "#f8d7da";
                break;
            };
        }
    }
}