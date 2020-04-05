using System;

namespace CovidTracer.Models
{
    /** Information about the time of a contact encounter.
     *
     * Encounters have a time definition of one hour.
     **/
    public class ContactEncounterTime : IComparable<ContactEncounterTime>
    {
        public readonly int Year;
        public readonly int Month;
        public readonly int Day;
        public readonly int Hour;

        public ContactEncounterTime(int year_, int month_, int day_, int hour_)
        {
            Year = year_;
            Month = month_;
            Day = day_;
            Hour = hour_;
        }

        public ContactEncounterTime(DateTime dt)
        {
            Year = dt.Year;
            Month = dt.Month;
            Day = dt.Day;
            Hour = dt.Hour;
        }

        static public ContactEncounterTime Now()
        {
            return new ContactEncounterTime(DateTime.Now);

        }

        /** Contact encounter time for the next hour. */
        public ContactEncounterTime Next {
            get {
                if (Hour < 23) {
                    return new ContactEncounterTime(Year, Month, Day, Hour + 1);
                } else {
                    return new ContactEncounterTime(AsDateTime().AddHours(1));
                }
            }
        }

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, 0, 0);
        }

        public int CompareTo(ContactEncounterTime other)
        {
            return AsDateTime().CompareTo(other.AsDateTime());
        }

        public override string ToString()
        {
            return AsDateTime().ToString("yyyy-MM-dd HH:00");
        }
    }
}
