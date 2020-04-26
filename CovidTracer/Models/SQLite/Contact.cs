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
                new string[] { "key", "year", "month", "day", "hour", }, true);

            conn.CreateIndex("contacts",
                new string[] { "year", "month", "day", "hour", }, true);
        }
    }
}
