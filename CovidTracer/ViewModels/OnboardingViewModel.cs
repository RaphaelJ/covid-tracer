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
                    "CovidTracer utilise la technologie Bluetooth pour vous " +
                    "notifier anonymement des interactions que vous auriez " +
                    "pu avoir avec des personnes infectées au coronavirus."
                ),
                new OnboardingPane(
                    "shield.png",
                    "CovidTracer est totalement anonyme et ne collecte " +
                    "ni donnée personelle, ni votre localisation. " +
                    "CovidTracer respecte ainsi votre vie privée."
                ),
                new OnboardingPane(
                    "recommended_uses.png",
                    "Pour une efficacité optimale, laissez l'application " +
                    "ouverte ou en arrière plan lorsque vous interagissez " +
                    "avec des personnes extérieures à votre foyer " +
                    "(transports en commun, travail, supermarchés, sorties " +
                    "extérieures ...)"
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
