using System;

namespace CovidTracer.Models
{
    /** Information about an encounter with another CovidTracer user.
     *
     * An encounter is defined as a [BeginsAt..EndsAt( period.
     **/
    public class ContactEncounter : IComparable<ContactEncounter>
    {
        public readonly ContactEncounterTime BeginsAt;
        public readonly ContactEncounterTime EndsAt;

        public ContactEncounter(ContactEncounterTime beginsAt_,
            ContactEncounterTime endsAt_)
        {
            BeginsAt = beginsAt_;
            EndsAt = endsAt_;
        }

        /** Creates a one hour encounter from the given contact encounter time.
         */
        public ContactEncounter(ContactEncounterTime at)
        {
            BeginsAt = at;
            EndsAt = at.Next;
        }

        public bool Contains(ContactEncounterTime time)
        {
            return BeginsAt.CompareTo(time) <= 0
                && EndsAt.CompareTo(time) > 0;
        }

        /** Returns true if the encounter's times match the given case
         * infectious period. */
        public bool Intersect(Case case_)
        {
            return BeginsAt.AsDateTime() < case_.EndsOn.AsDateTime()
                && EndsAt.AsDateTime() > case_.BeginsOn.AsDateTime();
        }

        public int CompareTo(ContactEncounter other)
        {
            return BeginsAt.CompareTo(other.BeginsAt);
        }
    }
}
