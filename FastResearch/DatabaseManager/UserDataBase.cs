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
                new SqliteConnection("Filename=user.db"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT " +
            "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
            "Text_Entry NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();
            }
        }

    }
}
