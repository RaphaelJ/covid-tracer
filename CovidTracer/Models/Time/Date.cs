// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

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

        public DateHour WithHour(int hour)
        {
            return new DateHour(Year, Month, Day, hour);
        }

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day);
        }

        /** Returns a ISO 8601 formatted date string. */
        public override string ToString()
        {
            return $"{Year:D4}-{Month:D2}-{Day:D2}";
        }

        public static Date ParseISO(string value)
        {
            var year = int.Parse(value.Substring(0, 4));
            var month = int.Parse(value.Substring(5, 2));
            var day = int.Parse(value.Substring(8, 2));
            return new Date(year, month, day);
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

        public override bool Equals(object other)
        {
            if (other is Date) {
                return Equals((Date)other);
            } else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Date lhs, Date rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Date lhs, Date rhs)
        {
            return !(lhs == rhs);
        }
    }
}
