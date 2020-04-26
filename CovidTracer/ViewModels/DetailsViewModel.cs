using System.Collections.Generic;
using System.Linq;

using CovidTracer.Models;
using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;
using CovidTracer.Services;

namespace CovidTracer.ViewModels
{
    public class ContactItem
    {
        public string BackgroundColor { get; protected set; }
        public string TextColor { get; protected set; }

        public string Title { get; protected set; }
        public string Description { get; protected set; }

        public readonly SortedSet<DateHour> Encounters;

        public string History
        {
            get {
                var entries = new List<string>(Encounters.Count);

                foreach (var encounter in Encounters) {
                    var dt = encounter.AsDateTime().ToLocalTime();

                    var text = string.Format(
                        Resx.Localization.DetailsInteractionHistoryDate,
                        dt.ToLongDateString(), dt.ToShortTimeString()
                    );

                    entries.Add(text);
                }

                return string.Join("<br>", entries);
            }
        }

        public ContactItem(
            string backgroundColor_, string textColor_, string title_,
            string description_, SortedSet<DateHour> encounters_
        )
        {
            BackgroundColor = backgroundColor_;
            TextColor = textColor_;

            Title = title_;
            Description = description_;

            Encounters = encounters_;
        }
    }

    public class DetailsViewModel : BaseViewModel
    {
        bool isItemsEmpty;
        public bool IsItemsEmpty
        {
            get { return isItemsEmpty; }
            set { SetProperty(ref isItemsEmpty, value); }
        }

        bool isItemsNonEmpty;
        public bool IsItemsNonEmpty
        {
            get { return isItemsNonEmpty; }
            set { SetProperty(ref isItemsNonEmpty, value); }
        }

        IList<ContactItem> items;
        public IList<ContactItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        private readonly bool isDebug;

        public DetailsViewModel(ContactDatabase contacts)
        {
            OnMatchesChange(this, contacts.Matches);
            contacts.MatchesChange += OnMatchesChange;
        }

        /** Updates the encounters detail list. */
        protected void OnMatchesChange(object sender,
            ContactDatabase.ContactMatches matches)
        {
            lock (matches) {
                var items = new List<ContactItem>();

                if (matches.Positives.Count > 0) {
                    items.Add(new ContactItem(
                        "#f8d7da", "#721c24",
                        Resx.Localization.DetailsInteractionPositiveTitle,
                        Resx.Localization.DetailsInteractionPositiveDetails,
                        matches.Positives
                    ));
                }

                if (matches.Symptomatics.Count > 0) {
                    items.Add(new ContactItem(
                        "#fff3cd", "#856404",
                        Resx.Localization.DetailsInteractionSymptomaticTitle,
                        Resx.Localization.DetailsInteractionSymptomaticDetails,
                        matches.Symptomatics
                    ));
                }

                Items = items;

                IsItemsEmpty = items.Count == 0;
                IsItemsNonEmpty = !IsItemsEmpty;
            }
        }
    }
}