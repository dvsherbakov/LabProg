using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.DataModels
{
    internal class DataInit
    {
        public DataInit()
        {
            if (File.Exists("MyDatabase.sqlite")) return;

            var con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            var commands = new List<SQLiteCommand> {GetTestTable(con)};

            con.Open();
            using (var transaction = con.BeginTransaction())
            {
                foreach (var command in commands)
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private static SQLiteCommand GetTestTable(SQLiteConnection con)
        {
            const string sql = @"CREATE TABLE Student(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT,
                               FirstName           TEXT      NOT NULL,
                               LastName            TEXT       NOT NULL
                            );";
            return new SQLiteCommand(sql, con);
        }
    }
}
