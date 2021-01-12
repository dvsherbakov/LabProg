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
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            const string sql = @"CREATE TABLE Student(
                               ID INTEGER PRIMARY KEY AUTOINCREMENT,
                               FirstName           TEXT      NOT NULL,
                               LastName            TEXT       NOT NULL
                            );";
            var con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            con.Open();
            var cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
