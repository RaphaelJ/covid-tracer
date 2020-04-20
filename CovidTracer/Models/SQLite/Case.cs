using SQLite;

namespace CovidTracer.Models.SQLite
{
    [Table("cases")]
    public class Case
    {
        [PrimaryKey, Column("key")]
        public byte[] Key { get; set; } // 20 bytes hourly key

        [Column("type")]
        public string Type { get; set; } // 'symptomatic' or 'positive'

        [Column("year")]
        public int Year { get; set; }
        [Column("month")]
        public int Month { get; set; }
        [Column("day")]
        public int Day { get; set; }
    }
}
