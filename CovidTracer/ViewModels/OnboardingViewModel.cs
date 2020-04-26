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
