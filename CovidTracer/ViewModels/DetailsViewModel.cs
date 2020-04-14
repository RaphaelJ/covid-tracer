using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CovidTracer.Models;
using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.ViewModels
{
    public class ContactItem
    {
        public string BackgroundColor
        {
            get {
                switch (Status) {
                case InfectionStatus.Safe:
                    return "#e2e3e5";
                case InfectionStatus.Symptomatic:
                    return "#fff3cd";
                case InfectionStatus.Positive:
                    return "#f8d7da";
                default:
                    return null;
                }
            }
        }

        public string TextColor
        {
            get {
                switch (Status) {
                case InfectionStatus.Safe:
                    return "#383d41";
                case InfectionStatus.Symptomatic:
                    return "#856404";
                case InfectionStatus.Positive:
                    return "#721c24";
                default:
                    return null;
                }
            }
        }

        public HourlyTracerKey Key { get; set; }

        public bool ShowKey { get; set; }

        public InfectionStatus Status { get; set; }

        public string Title
        {
            get {
                switch (Status) {
                case InfectionStatus.Safe:
                    return "Interaction";
                case InfectionStatus.Symptomatic:
                    return "Interaction suspecte";
                case InfectionStatus.Positive:
                    return "Interaction avec un cas positif";
                default:
                    return null;
                }
            }
        }

        public bool HasDescription
        {
            get {
                return Status != InfectionStatus.Safe;
            }
        }

        public string Description
        {
            get {
                switch(Status) {
                case InfectionStatus.Safe:
                    return null;
                case InfectionStatus.Symptomatic:
                    return "Vous avez été à proximité d'une ou plusieurs " +
                        "personnes ayant des symptomes similaires au " +
                        "COVID-19 aux occasions suivantes:";
                case InfectionStatus.Positive:
                    return "Vous avez été à proximité d'une ou plusieurs " +
                        "personnes testée positive au COVID-19 aux occasions " +
                        "suivantes:";
                default:
                    return null;
                }
            }
        }

        public readonly IList<DateHour> Encounters;

        public string History
        {
            get {
                var entries = new List<string>(Encounters.Count);

                foreach (var encounter in Encounters) {
                    var dt = encounter.AsDateTime();

                    entries.Add(
                        $"Le {dt.ToLongDateString()} à " +
                        $"{dt.ToShortTimeString()}."
                    );
                }

                return string.Join("<br>", entries);
            }
        }

        public ContactItem(InfectionStatus status_, IList<DateHour> encounters_)
        {
            Status = status_;
            Encounters = encounters_;
        }
    }

    public class DetailsViewModel : BaseViewModel
    {
        readonly ContactDatabase database = ContactDatabase.GetInstance();

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

            Contacts = AsContactItemList(database.Contacts);

            IsContactListEmpty = Contacts.Count == 0;
            IsContactListNonEmpty = !IsContactListEmpty;
        }
        
        /** Generates a list of ContactItem object sorted by the last time
         * the contacts have been seen. */
        public IList<ContactItem> AsContactItemList(
            Dictionary<HourlyTracerKey, SortedSet<DateHour>> contacts)
        {
            var caseDb = CaseDatabase.GetInstance();

            return contacts
                .SelectMany(contact => {
                    // Classified all the encounters with the contact key,
                    // depending on time.
                    var key = contact.Key;

                    return
                        from encounter in contact.Value
                        group encounter by caseDb.Classify(key, encounter)
                            into groupedEncounters
                        select new ContactItem(
                            groupedEncounters.Key,
                            groupedEncounters.ToList()
                        );
                })
                // Does not display safe cases if not in debug mode.
                .Where(item => isDebug || item.Status != InfectionStatus.Safe)
                .OrderBy(item => item.Encounters.Last())
                .Reverse()
                .ToList();
        }
    }
}