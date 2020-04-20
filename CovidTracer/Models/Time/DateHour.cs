using System;

namespace CovidTracer.Models.Time
{
    /** Date with an hour component. **/
    public class DateHour : IComparable<DateHour>, IEquatable<DateHour>
    {
        public readonly int Year;
        public readonly int Month;
        public readonly int Day;
        public readonly int Hour;

        public DateHour(int year_, int month_, int day_, int hour_)
        {
            Year = year_;
            Month = month_;
            Day = day_;
            Hour = hour_;
        }

        public DateHour(DateTime dt)
        {
            Year = dt.Year;
            Month = dt.Month;
            Day = dt.Day;
            Hour = dt.Hour;
        }

        static public DateHour Now
        {
            get {
                return new DateHour(DateTime.UtcNow);
            }
        }

        /** Contact encounter time for the next hour. */
        public DateHour Next {
            get {
                if (Hour < 23) {
                    return new DateHour(Year, Month, Day, Hour + 1);
                } else {
                    return new DateHour(AsDateTime().AddHours(1));
                }
            }
        }

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, 0, 0);
        }

        public Date AsDate()
        {
            return new Date(Year, Month, Day);
        }

        /** Returns a ISO 8601 formatted date and hour string. */
        public override string ToString()
        {
            return $"{AsDate()}T{Hour:D2}";
        }

        public int CompareTo(DateHour other)
        {
            var date = AsDate();
            var otherDate = other.AsDate();

            if (date == otherDate) {
                return Hour.CompareTo(other.Hour);
            } else {
                return date.CompareTo(otherDate);
            }
        }

        public bool Equals(DateHour other)
        {
            return Year == other.Year
                && Month == other.Month
                && Day == other.Day
                && Hour == other.Hour;
        }

        public override bool Equals(object other)
        {
            if (other is DateHour) {
                return Equals((DateHour)other);
            } else {
                return false;
            }
        }

        public static bool operator ==(DateHour lhs, DateHour rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DateHour lhs, DateHour rhs)
        {
            return !(lhs == rhs);
        }
    }
}
