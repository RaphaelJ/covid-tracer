using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CovidTracer.Models
{
    /** Stores the CovidTracer contacts as detected by the Bluetooth scanning
     * service. */
    public /* Singleton */ class ContactDatabase
    {
        // As we can contact a device multiple times, all these contacts are
        // stored. Contacts are defined by a [BeginAt..EndAt( time period.
        public readonly Dictionary<CovidTracerID, SortedSet<ContactEncounter>>
            Contacts =
                new Dictionary<CovidTracerID, SortedSet<ContactEncounter>>();

        static private ContactDatabase instance = null;

        static public ContactDatabase GetInstance()
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new ContactDatabase();
                return instance;
            }
        }

        public ContactDatabase()
        {
            var dev1 = new CovidTracerID("ABCDEFGH");
            var dev2 = new CovidTracerID("12345678");
            var dev3 = new CovidTracerID("A1B2C3D4");
            var dev4 = new CovidTracerID("09876543");

            NewContact(dev1,
                new ContactEncounterTime(2020, 4, 5, 0));
            NewContact(dev1,
                new ContactEncounterTime(2020, 4, 5, 1));
            NewContact(dev1,
                new ContactEncounterTime(2020, 4, 5, 2));
            NewContact(dev1,
                new ContactEncounterTime(2020, 4, 5, 5));

            NewContact(dev2,
                new ContactEncounterTime(2020, 3, 30, 23));
            NewContact(dev2,
                new ContactEncounterTime(2020, 3, 31, 0));

            NewContact(dev3,
                new ContactEncounterTime(2020, 3, 31, 0));

            NewContact(dev4,
                new ContactEncounterTime(2020, 4, 1, 20));
            NewContact(dev4,
                new ContactEncounterTime(2020, 4, 1, 23));
        }


        /** Record a new device contact that happens at the moment. */
        public void NewContact(CovidTracerID id)
        {
            NewContact(id, ContactEncounterTime.Now());
        }

        public void NewContact(CovidTracerID id, ContactEncounterTime time)
        {
            Logger.Info($"New encounter: {id} at {time}");

            lock (Contacts) {
                if (Contacts.ContainsKey(id)) {
                    var prevEncounter = Contacts[id].Max;

                    var prevCmp = prevEncounter.EndsAt.CompareTo(time);

                    // FIXME Currently only supports contact that occur in time
                    // order.
                    Debug.Assert(prevCmp <= 0);

                    if (prevCmp == 0) {
                        // Extends the last encounter by one hour.
                        Contacts[id].Remove(prevEncounter);
                        Contacts[id].Add(new ContactEncounter(
                            prevEncounter.BeginsAt, time.Next));
                    } else {
                        // Can't merge with the last encounter, creates a new
                        // one.
                        Contacts[id].Add(new ContactEncounter(time));
                    }
                } else {
                    // First time we meet that device.
                    Contacts.Add(
                        id,
                        new SortedSet<ContactEncounter> {
                            new ContactEncounter(time) });
                }
            }
        }
    }
}
