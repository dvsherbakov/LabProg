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
            var commands = new List<SQLiteCommand> {GetLogTable(con), GetTemperatureTable(con)};

            con.Open();
            using var transaction = con.BeginTransaction();
            foreach (var command in commands)
            {
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        private static SQLiteCommand GetLogTable(SQLiteConnection con)
        {
            const string sql = @"CREATE TABLE Logs(
                               Id INTEGER PRIMARY KEY AUTOINCREMENT,
                               Dt datetime default current_timestamp,
                               Message TEXT,
                               Code INT
                            );";
            return new SQLiteCommand(sql, con);
        }

        private static SQLiteCommand GetTemperatureTable(SQLiteConnection con)
        {
            const string sql = @"CREATE TABLE Temperatures(
                               Id INTEGER PRIMARY KEY AUTOINCREMENT,
                               Dt datetime default current_timestamp,
                               Tmp real
                            );";
            return new SQLiteCommand(sql, con);
        }
    }
}
