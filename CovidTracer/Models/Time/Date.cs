using System;

namespace CovidTracer.Models.Time
{
    /** A date without time component. */
    public class Date : IComparable<Date>, IEquatable<Date>
    {
        public readonly int Year;
        public readonly int Month;
        public readonly int Day;

        public Date(int year_, int month_, int day_)
        {
            Year = year_;
            Month = month_;
            Day = day_;
        }

        /** Creates a `Date` instance from a `DateTime` object by ignoring the
         * time component. */
        public Date(DateTime dt)
        {
            Year = dt.Year;
            Month = dt.Month;
            Day = dt.Day;
        }

        static public Date Today
        {
            get {
                return new Date(DateTime.UtcNow);
            }
        }

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day);
        }

        public int CompareTo(Date other)
        {
            if (Year == other.Year) {
                if (Month == other.Month) {
                    return Day.CompareTo(other.Day);
                } else {
                    return Month.CompareTo(other.Month);
                }
            } else {
                return Year.CompareTo(other.Year);
            }
        }

        public bool Equals(Date other)
        {
            return Year == other.Year
                && Month == other.Month
                && Day == other.Day;
        }

        /** Returns a ISO 8601 formatted date string. */
        public override string ToString()
        {
            return $"{Year:D4}-{Month:D2}-{Day:D2}";
        }
    }
}
