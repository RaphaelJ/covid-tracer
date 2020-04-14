using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.Models
{
    public enum CaseType
    {
        Symptomatic, Positive
    }

    /** A `Symptomatic` or `Positive` case being contagious on the given day.
     */
    public class Case
    {
        public readonly DailyTracerKey Key;
        public readonly CaseType Type;
        public readonly Date Day;

        public Case(DailyTracerKey key_, CaseType type_, Date day_)
        {
            Key = key_;
            Type = type_;
            Day = day_;
        }
    }
}
