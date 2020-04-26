using System;
using System.Windows.Input;
using CovidTracer.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CovidTracer.ViewModels
{
    public class SignalingViewModel : BaseViewModel
    {
        DateTime currentDate;
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set { SetProperty(ref currentDate, value); }
        }

        DateTime symptomsOnset;
        public DateTime SymptomsOnset
        {
            get { return symptomsOnset; }
            set { SetProperty(ref symptomsOnset, value); }
        }

        bool isTested;
        public bool IsTested
        {
            get { return isTested; }
            set { SetProperty(ref isTested, value); }
        }

        string comment;
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        public SignalingViewModel()
        {
            CurrentDate = DateTime.Now;

            SymptomsOnset = CurrentDate;
            IsTested = false;
            Comment = "";
        }
    }
}