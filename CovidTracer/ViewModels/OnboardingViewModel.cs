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

using System.Collections.Generic;

namespace CovidTracer.ViewModels
{
    public class OnboardingPane
    {
        public string ImageSource { get; set; }
        public string Text { get; set; }

        public OnboardingPane(string imageSource_, string text_)
        {
            ImageSource = imageSource_;
            Text = text_;
        }
    }

    public class OnboardingViewModel : BaseViewModel
    {
        public readonly IList<OnboardingPane> Panes;

        int currentPaneIndex;
        public int CurrentPaneIndex
        {
            get { return currentPaneIndex; }
            set
            {
                CurrentPane = Panes[value];
                SetProperty(ref currentPaneIndex, value);
            }
        }

        OnboardingPane currentPane;
        public OnboardingPane CurrentPane
        {
            get { return currentPane; }
            protected set { SetProperty(ref currentPane, value); }
        }

        public OnboardingViewModel()
        {
            Panes = new List<OnboardingPane> {
                new OnboardingPane(
                    "bluetooth.png",
                    Resx.Localization.OnboardingBluetooth
                ),
                new OnboardingPane(
                    "shield.png",
                    Resx.Localization.OnboardingPrivacy
                ),
                new OnboardingPane(
                    "recommended_uses.png",
                    Resx.Localization.OnboardingRecommendedUses
                ),
            };

            CurrentPaneIndex = 0;
        }

        public bool IsLastPane()
        {
            return CurrentPaneIndex == (Panes.Count - 1);
        }

        public void NextPane()
        {
            if (!IsLastPane()) {
                ++CurrentPaneIndex;
            }
        }

        public void PreviousPane()
        {
            if (CurrentPaneIndex > 0) {
                --CurrentPaneIndex;
            }
        }
    }
}
