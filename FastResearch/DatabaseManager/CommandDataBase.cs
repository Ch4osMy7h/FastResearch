using FastResearch.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.DatabaseManager
{
    public static class CommandDataBase
    {
        private static SQLiteConnection db;

        public static void InitializeDatabase()
        {
            db = new SQLiteConnection("CommandDataBase");
            db.CreateTable<Command>();
            db.CreateTable<OptionPair>();
        }

        public static void Insert<T>(T item)
        {
            db.InsertWithChildren(item);
        }

        public static void Update<T>(T item)
        {
            db.UpdateWithChildren(item);
            db.Update(item);
        }

        public static void Delete<T>(T item)
        {
            db.Delete(item);
        }

        public static ObservableCollection<Command> GetCommand()
        {
            return new ObservableCollection<Command>(db.GetAllWithChildren<Command>().ToList());           
        }

        public static void DeleteAll()
        {
            db.DeleteAll<Command>();
            db.DeleteAll<OptionPair>();
        }
    }
}
