using System;

namespace CovidTracer.Models
{
    /** Information about a single contact encounter.
     *
     * Two encounters are considered equals if they happen during the same hour.
     **/
    public class ContactEncounter : IComparable<ContactEncounter>
    {
        public readonly int Year;
        public readonly int Month;
        public readonly int Day;
        public readonly int Hour;

        public ContactEncounter(int year_, int month_, int day_, int hour_)
        {
            Year = year_;
            Month = month_;
            Day = day_;
            Hour = hour_;
        }

        /** Creates a contact encouter with the current time. */
        static public ContactEncounter Now()
        {
            var now = DateTime.Now;
            return new ContactEncounter(now.Year, now.Month, now.Day, now.Hour);
        }

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, 0, 0);
        }

        public int CompareTo(ContactEncounter other)
        {
            return AsDateTime().CompareTo(other.AsDateTime());
        }

        public override string ToString()
        {
            return AsDateTime().ToString("YYYY-MM-DD HH:00");
        }
    }
}
