using System.Collections.Generic;
using System.Linq;
using System.Text;
using CovidTracer.Models;

namespace CovidTracer.ViewModels
{
    public class ContactItem
    {
        public string BackgroundColor
        {
            get {
                switch (Type) {
                case CaseType.Safe:
                    return "#e2e3e5";
                case CaseType.Symptomatic:
                    return "#fff3cd";
                case CaseType.Positive:
                    return "#f8d7da";
                default:
                    return null;
                }
            }
        }

        public string TextColor
        {
            get {
                switch (Type) {
                case CaseType.Safe:
                    return "#383d41";
                case CaseType.Symptomatic:
                    return "#856404";
                case CaseType.Positive:
                    return "#721c24";
                default:
                    return null;
                }
            }
        }

        public string ID { get; set; }

        public bool ShowID { get; set; }

        public CaseType Type { get; set; }

        public string Title
        {
            get {
                switch (Type) {
                case CaseType.Safe:
                    return "Interaction";
                case CaseType.Symptomatic:
                    return "Interaction suspecte";
                case CaseType.Positive:
                    return "Interaction avec un cas positif";
                default:
                    return null;
                }
            }
        }

        public bool HasDescription
        {
            get {
                return Type != CaseType.Safe;
            }
        }

        public string Description
        {
            get {
                switch(Type) {
                case CaseType.Safe:
                    return null;
                case CaseType.Symptomatic:
                    return "Vous avez été à proximité d'une personne " +
                        "ayant des symptomes similaires au COVID-19 aux " +
                        "occasions suivantes:";
                case CaseType.Positive:
                    return "Vous avez été à proximité d'une personne " +
                        "testée positive au COVID-19 aux occasions suivantes:";
                default:
                    return null;
                }
            }
        }

        public readonly IList<ContactEncounter> Encounters;

        public string History
        {
            get {
                var entries = new List<string>(Encounters.Count);

                foreach (var encounter in Encounters) {
                    var beginsAtDt = encounter.BeginsAt.AsDateTime();
                    var endsAtDt = encounter.EndsAt.AsDateTime();

                    if (
                        beginsAtDt.Year == endsAtDt.Year
                        && beginsAtDt.Month == endsAtDt.Month
                        && beginsAtDt.Day == endsAtDt.Day
                    ) {
                        // Encounters begins and ends on the same day.
                        entries.Add(
                            $"Le {beginsAtDt.ToLongDateString()} entre " +
                            $"{beginsAtDt.ToShortTimeString()} et " +
                            $"{endsAtDt.ToShortTimeString()}."
                        );
                    } else {
                        entries.Add(
                            $"Du {beginsAtDt.ToLongDateString()} à " +
                            $"{beginsAtDt.ToShortTimeString()} au " +
                            $"{endsAtDt.ToLongDateString()} à " +
                            $"{endsAtDt.ToShortTimeString()}."
                        );
                    }
                }

                return string.Join("<br>", entries);
            }
        }

        public ContactItem(
            CovidTracerID id_, bool showId, CaseType type_,
            IList<ContactEncounter> encounters_)
        {
            ID = id_.ToHumanReadableString();
            ShowID = showId;
            Type = type_;
            Encounters = encounters_;
        }
    }

    public class DetailsViewModel : BaseViewModel
    {
        readonly ContactDatabase database = ContactDatabase.GetInstance();

        string appId;
        public string AppId
        {
            get { return appId; }
            set { SetProperty(ref appId, value); }
        }

        bool isContactListEmpty;
        public bool IsContactListEmpty
        {
            get { return isContactListEmpty; }
            set { SetProperty(ref isContactListEmpty, value); }
        }

        bool isContactListNonEmpty;
        public bool IsContactListNonEmpty
        {
            get { return isContactListNonEmpty; }
            set { SetProperty(ref isContactListNonEmpty, value); }
        }
        IList<ContactItem> contacts;
        public IList<ContactItem> Contacts
        {
            get { return contacts; }
            set { SetProperty(ref contacts, value); }
        }

        private readonly bool isDebug;

        /** Show encounters with non-positive people and tracer IDs if `debug_`
         * is true. */
        public DetailsViewModel(bool isDebug_)
        {
            isDebug = isDebug_;

            Title = $"Details";

            AppId = CovidTracerID.GetCurrentInstance().ToHumanReadableString();

            Contacts = AsContactItemList(database.Contacts);

            IsContactListEmpty = Contacts.Count == 0;
            IsContactListNonEmpty = !IsContactListEmpty;
        }
        
        /** Generates a list of ContactItem object sorted by the last time
         * the contacts have been seen. */
        public IList<ContactItem> AsContactItemList(
            Dictionary<CovidTracerID, SortedSet<ContactEncounter>> contacts)
        {
            var caseDb = CaseDatabase.GetInstance();

            return contacts
                .SelectMany(contact => {
                    var id = contact.Key;

                    // Groups the encounters by classification and creates a
                    // ContactItem for each of these classification.
                    return
                        from encounter in contact.Value
                        group encounter by caseDb.Classify(id, encounter)
                            into groupedEncounters
                        select new ContactItem(
                             id, isDebug, groupedEncounters.Key,
                             groupedEncounters.ToList());
                })
                // Does not display safe cases if not in debug mode.
                .Where(item => isDebug || item.Type != CaseType.Safe)
                .OrderBy(item => item.Encounters.Last())
                .Reverse()
                .ToList();
        }
    }
}