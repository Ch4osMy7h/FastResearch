using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.DatabaseManager
{
    public static class CommandDatabase
    {
        public static void InitializeDatabase()
        {

            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=commandData.db"))
                {
                    db.Open();

                    String TableCreateCommand = "CREATE TABLE IF NOT " +
                                "EXISTS Command(ComandId INTEGER, CommandName Text)";
                    SqliteCommand createTable = new SqliteCommand(TableCreateCommand, db);
                    createTable.ExecuteReader();
                    String TableCreateOption = "CREATE TABLE IF NOT " +
                                "EXISTS Option(OptionId INTEGER, " + "OptionName Text, " + "OptionValue Text," + "Command Text)";
                    createTable = new SqliteCommand(TableCreateOption, db);
                    createTable.ExecuteReader();
                }
            }
            catch
            {
                Debug.WriteLine("无法打开数据库");
            }
        }
        /// <summary>
        ///  在数据库中加入PaperArea
        /// </summary>
        /// <param name="PaperArea"></param>
        public static void addPaperArea(string PaperArea)
        {
            try
            {
                using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;
                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO PaperAreas VALUES (NULL, @PaperArea);";
                    insertCommand.Parameters.AddWithValue("@PaperArea", PaperArea);
                    insertCommand.ExecuteReader();
                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("加入不了Area数据");
            }
        }
        /// <summary>
        /// 
        /// 在数据库中加入论文名
        /// </summary>
        /// <param name="paperAreaId"></param>
        /// <param name="paper"></param>
        public static void addPaper(string paperArea, string paper, string paperPath)
        {
            try
            {
                using (SqliteConnection db =
                new SqliteConnection("Filename=userdata.db"))
                {
                    //db.
                    db.Open();

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;
                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO Papers VALUES (Null, @Paper, @BelongToPaperArea, @PapersPath)";
                    insertCommand.Parameters.AddWithValue("@Paper", paper);
                    insertCommand.Parameters.AddWithValue("@BelongToPaperArea", paperArea);
                    insertCommand.Parameters.AddWithValue("@PapersPath", paperPath);

                    insertCommand.ExecuteReader();
                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("加入不了Paper数据");
            }
        }
        /// <summary>
        /// 在数据中获取论文
        /// </summary>
        /// <returns></returns>
        public static List<String> getPaperArea()
        {
            List<String> PaperArea = new List<string>();

            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand
                        ("SELECT PaperArea from PaperAreas", db);

                    SqliteDataReader query = selectCommand.ExecuteReader();

                    while (query.Read())
                    {
                        PaperArea.Add(query.GetString(0));

                    }

                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("读取不了PaperArea类中的内容");
            }

            return PaperArea;
        }
        public static List<String> GetPaperName(String paperArea)
        {
            List<String> PaperName = new List<string>();

            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand
                        ("SELECT Paper FROM Papers WHERE BelongToPaperArea='" + paperArea + "'", db);
                    //Debug.WriteLine("SELECT Paper FROM Papers WHERE BelongToPaperArea='" + paperArea + "'");
                    //Debug.WriteLine(selectCommand);
                    SqliteDataReader query = selectCommand.ExecuteReader();

                    while (query.Read())
                    {

                        PaperName.Add(query.GetString(0));
                    }
                    //Debug.WriteLine(PaperName.Count);
                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
            return PaperName;
        }


        /// <summary>
        /// 获取PaperPath对应的Paper文件夹
        /// </summary>
        /// <param name="paper"> Paper 名字</param>
        /// <returns></returns>
        public static String GetPaperPath(String paper)
        {
            string paperPath = "";

            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand
                        ("SELECT PapersPath FROM Papers WHERE Paper='" + paper + "'", db);
                    //Debug.WriteLine("SELECT Paper FROM Papers WHERE BelongToPaperArea='" + paperArea + "'");
                    //Debug.WriteLine(selectCommand);
                    SqliteDataReader query = selectCommand.ExecuteReader();

                    Debug.WriteLine("go here");
                    while (query.Read())
                    {

                        paperPath = query.GetString(0);
                    }
                    //Debug.WriteLine(PaperName.Count);
                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
            return paperPath;
        }


        public static bool UpdatePaperPath(String paper, String paperpath)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("UPDATE Papers SET PapersPath='" + paperpath + "' " + "WHERE Paper='" + paper + "'", db);
                    updateCommand.ExecuteReader();
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
            return false;
        }

        public static void deletePaper(string paper)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("DELETE FROM Papers " + "WHERE Paper='" + paper + "'", db);
                    updateCommand.ExecuteReader();
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
        }
    }
}
}
