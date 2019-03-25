using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;


/// <summary>
/// Sqlite版本一定要是1.1.1以下
/// </summary>
namespace FastResearch.DatabaseManager
{
    public static class UserDataBase
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
            {
                db.Open();

                String TablePaperAreaCommand = "CREATE TABLE PaperArea(PaperArea Text, PaperName Text)";
                SqliteCommand createTable = new SqliteCommand(TablePaperAreaCommand, db);
                createTable.ExecuteReader();
            }
        }
        public static void AddData(string PaperArea, string PaperName)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO PaperArea VALUES (@PaperArea, @PaperName);";
                insertCommand.Parameters.AddWithValue("@PaperArea", PaperArea);
                insertCommand.Parameters.AddWithValue("@PaperName", PaperName);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }
        public static List<String> GetPaperArea()
        {
            List<String> PaperArea = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT PaperArea from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    PaperArea.Add(query.GetString(0));
                }

                db.Close();
            }

            return PaperArea;
        }
        public static List<String> GetPaperName()
        {
            List<String> PaperName = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT PaperName from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    PaperName.Add(query.GetString(0));
                }

                db.Close();
            }

            return PaperName;
        }

    }
}
