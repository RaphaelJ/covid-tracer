using System;
using System.Windows.Input;
using CovidTracer.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        //
        // Infection status
        //

        InfectionStatus status;
        public InfectionStatus Status
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

        public MainPageViewModel()
        {
            Title = "CovidTracer";

            ChangeStatus(InfectionStatus.Safe);
        }

        public void ChangeStatus(InfectionStatus newStatus)
        {
            Status = newStatus;

            switch (newStatus) {
            case InfectionStatus.Safe:
                StatusTitle = "Rien à signaler";
                StatusText = "Aucune interaction avec une personne " +
                             "infectée n'a été détectée.";
                StatusTextColor = "#313a33";
                StatusBackgroundColor = "#d4edda";
                break;
            case InfectionStatus.Symptomatic:
                StatusTitle = "Risque modéré";
                StatusText = "Vous avez été en contact rapproché avec " +
                             "un ou plusieurs cas suspectés d'infection " +
                             "au COVID-19.";
                StatusTextColor = "#856404";
                StatusBackgroundColor = "#fff3cd";
                break;
            case InfectionStatus.Positive:
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