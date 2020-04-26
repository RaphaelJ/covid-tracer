// Copyright 2020 Raphael Javaux
//
// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

using System;

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