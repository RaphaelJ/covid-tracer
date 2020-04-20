using SQLite;

namespace CovidTracer.Models.SQLite
{
    [Table("contacts")]
    public class Contact
    {
        // Note: composite primary keys are not supported, use an unique index
        // instead.

        // [PrimaryKey]
        [Column("key")]
        public byte[] Key { get; set; } // 20 bytes hourly key

        // [PrimaryKey]
        [Column("year")]
        public int Year { get; set; }
        // [PrimaryKey]
        [Column("month")]
        public int Month { get; set; }
        // [PrimaryKey]
        [Column("day")]
        public int Day { get; set; }
        // [PrimaryKey]
        [Column("hour")]
        public int Hour { get; set; }

        /** Shall be called after the `CreateTable<ContactSqliteEntry>` call.
         * to create the required composite indexes. */
        static public void CreateIndex(SQLiteConnection conn)
        {
            conn.CreateIndex("contacts",
                new string[] { "key", "month", "day", "hour", }, true);
        }
    }
}
