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

        DateTime symptomsDate;
        public DateTime SymptomsDate
        {
            get { return symptomsDate; }
            set { SetProperty(ref symptomsDate, value); }
        }

        bool tested;
        public bool Tested
        {
            get { return tested; }
            set { SetProperty(ref tested, value); }
        }

        string comment;
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        public SignalingViewModel()
        {
            Title = "Me signaler";

            CurrentDate = DateTime.Now;

            SymptomsDate = CurrentDate;
            Tested = false;
            Comment = "";
        }
    }
}