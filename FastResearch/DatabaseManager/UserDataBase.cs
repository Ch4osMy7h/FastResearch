using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

/// <summary>
/// Sqlite版本一定要是1.1.1以下
/// </summary>
namespace FastResearch.DatabaseManager
{
    /// <summary>
    /// 数据库类
    /// </summary>
    public static class UserDataBase
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitializeDatabase()
        {
            
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();

                    String TablePaperAreaCommand = "CREATE TABLE IF NOT " +
                                "EXISTS PaperAreas(PaperAreaId INTEGER, PaperArea Text)";
                    SqliteCommand createTable = new SqliteCommand(TablePaperAreaCommand, db);
                    createTable.ExecuteReader();
                    String TablePaperCommand = "CREATE TABLE IF NOT " +
                                "EXISTS Papers(PaperId INTEGER, " + "Paper Text, " + "BelongToPaperArea Text," + "PapersPath Text)";
                    createTable = new SqliteCommand(TablePaperCommand, db);
                    createTable.ExecuteReader();
                }
            } catch
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
            } catch
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
            } catch
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
            } catch
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


        /// <summary>
        /// 存储Paper所存储路径
        /// </summary>
        /// <param name="paper"></param>
        /// <param name="paperpath"></param>
        /// <returns></returns>
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
                    db.Close();
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

                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
        }

        public static void deletePaperArea(string paperArea)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("DELETE FROM Papers " + "WHERE BelongToPaperArea='" + paperArea + "'", db);
                    updateCommand.ExecuteReader();
                }

                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("DELETE FROM PaperAreas " + "WHERE PaperArea='" + paperArea + "'", db);
                    updateCommand.ExecuteReader();
                    db.Close();
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
        }

        /// <summary>
        /// 更新Paper名
        /// </summary>
        /// <param name="newPaperName"></param>
        /// <param name="oldPaperName"></param>
        /// <returns></returns>
        public static bool UpdatePaper(String newPaperName, String oldPaperName)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("UPDATE Papers SET Paper='" + newPaperName + "' " + "WHERE Paper='" + oldPaperName + "'", db);
                    updateCommand.ExecuteReader();
                    db.Close();
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
            return false;
        }

        /// <summary>
        /// 更新PaperArea名
        /// </summary>
        /// <param name="newPaperAreaName"></param>
        /// <param name="oldPaperAreaName"></param>
        /// <returns></returns>
        public static bool UpdatePaperArea(String newPaperAreaName, String oldPaperAreaName)
        {
            try
            {
                using (SqliteConnection db =
                    new SqliteConnection("Filename=userdata.db"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand
                        ("UPDATE PaperAreas SET PaperArea='" + newPaperAreaName + "' " + "WHERE PaperArea='" + oldPaperAreaName + "'", db);
                    updateCommand.ExecuteReader();
                    db.Close();
                    return true;
                }
            }
            catch
            {
                Debug.WriteLine("Bug!");
            }
            return false;
        }
    }
}
