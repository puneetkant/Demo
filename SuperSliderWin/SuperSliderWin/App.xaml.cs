using SQLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SuperSliderWin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DatabasePath = string.Empty;
        public App()
        {
            string dbname = "SuperSliderWin_db.sqlite";
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fullPath = Path.Combine(folder, dbname);

            DatabasePath = fullPath;

            using (SQLiteConnection conn =
                    new SQLiteConnection(App.DatabasePath))
            {
                //conn.CreateTable<Post>();
                //int rows = conn.Insert(post);

                conn.CreateTable<Settings>();

            }
        }
    }
}
