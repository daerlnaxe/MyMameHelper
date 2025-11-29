using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMameHelper.Properties;
using System.Diagnostics;
using System.IO;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_OP
    {
        internal const string MOT_DE_PASSE = @"jw9s4X#7~S4#4P-y65_Sk-k@GmWG}y3r~V7e476-:DC-4VxgpB";
        public delegate void SendIntValue(object sender, int value);
        public event SendIntValue UpdateProgress;




        string tRom = PProp.Default.T_Roms;
        string tBios = PProp.Default.T_Bios;
        string tGenre = PProp.Default.T_Genres;
        string tMachine = PProp.Default.T_Machines;
        string tGame = PProp.Default.T_Games;
        //string tManufacturer = PProp.Default.T_MameManufacturers;
        static string tMameManufacturer = Properties.Settings.Default.T_MameManufacturers;
        static string tMechanical = Properties.Settings.Default.T_Mechanics;
        string tConstructor = PProp.Default.T_Constructors;
        static string tTempRom = Properties.Settings.Default.T_TempRoms;
        static string tSQLInfo = Properties.Settings.Default.T_SQLInfo;


        public SQLiteConnection SQLiteConn { get; set; }
        public ConnectionState state
        {
            get { return SQLiteConn.State; }
        }


        /// <summary>
        /// Connexion
        /// </summary>
        /// <param name="mot_de_passe"></param>
        /// <returns></returns>
        // todo: voir pour un nombre de tentatives
        public SQLite_OP()
        {
            // Vérification de l'existance du fichier
            if (!File.Exists(Settings.Default.DataBase_Path))
            {
                this.Dispose();
                throw new IOException("Base de donnée inaccessible");
            }

            // Connexion
            try
            {
                SQLiteConn = new SQLiteConnection($"Data Source={Settings.Default.DataBase_Path};Version=3");
                string path = Directory.GetCurrentDirectory();
                Trace.WriteLine($"Current directory: '{path}'");


                SQLiteConn.Open();

                while (SQLiteConn.State == ConnectionState.Closed)
                {
                    Debug.WriteLine("waiting");
                }

                Debug.WriteLine($"SQlite_Test, connexion à '{SQLiteConn.DataSource}': {SQLiteConn.State}");
                Trace.WriteLine($"Connect to {SQLiteConn.FileName}");
            }
            catch (SQLiteException sqlEXC)
            {
                Debug.WriteLine(sqlEXC.Message);
                this.Dispose();
                throw new Exception("Erreur SQlite");
            }



        }
    }
}
