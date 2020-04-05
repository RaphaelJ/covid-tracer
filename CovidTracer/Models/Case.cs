using System;
namespace CovidTracer.Models
{
    public enum CaseType
    {
        Safe, Symptomatic, Positive
    }

    public class Date
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

        public DateTime AsDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }

    /** A `Sympromatic` or `Positive` being contagious during the
     * [BeginsOn..EndsOn( period.
     */
    public class Case
    {
        public readonly CaseType Type;
        public readonly Date BeginsOn;
        public readonly Date EndsOn;

        public Case(CaseType type_, Date beginsOn_, Date endsOn_)
        {
            Type = type_;
            BeginsOn = beginsOn_;
            EndsOn = endsOn_;
        }
    }
}
