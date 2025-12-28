using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.SQLite
{
    /// <summary>
    /// Contient les instructions pour créer la structure de la table
    /// </summary>
    class SQLiteNewDb
    {

        //static string tDeveloppers = Properties.Settings.Default.T_Developers;
        static string tConstructors = Properties.Settings.Default.T_Constructors;
        static string tGenres = Properties.Settings.Default.T_Genres;
        static string tRoms = Properties.Settings.Default.T_Roms;



        //static SQLiteConnection _MaConn;
        static SQLite_OP _SQLite_Op = null;

        /// <summary>
        /// Création de la connexion
        /// </summary>
        /// <param name="dbLink"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Create(string dbLink)
        {
            try
            {
                if (File.Exists(dbLink))
                {
                    MessageBox.Show("Abort, existing Db, remove it manually", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SQLiteConnection.CreateFile(dbLink);
            }
            catch (IOException ioe)
            {
                Debug.WriteLine("SQliteDb: " + ioe.Message);
                //  DxTBoxWPF.MBox.DxMBox.ShowDial(ioe.Message, "Error", DxTBoxWPF.Common.DxButtons.Ok);
                throw new NotImplementedException("Base de donnée occupée");
                return;
            }

            /*_MaConn*/
            //SQLiteConnection conn = new SQLiteConnection($"Data Source={dbLink};Version=3");

            string tMachines = Properties.Settings.Default.T_Machines;


            try
            {


                // Connexion
                //_MaConn.Open();
                _SQLite_Op = new SQLite_OP();

                Create_Structure();
                //Alter_Structure();
                Fill_Basics_Data();

                MessageBox.Show("Database Created");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                //_MaConn.Close();
                _SQLite_Op.Dispose();
            }

        }


        /// <summary>
        /// Création de la structure de la base
        /// </summary>
        private static void Create_Structure()
        {
            // Création du minimum
            // Table Games (Elle permet des options en plus, personnalisables, pour les roms. Elle est liée à roms, genres)
            _SQLite_Op.Create_TGame();

            // Bios
            _SQLite_Op.Create_TBios();

            // Mechanicals
            _SQLite_Op.Create_TMechanical();

            // Constructeurs, va faire le lien 
            _SQLite_Op.Create_TMameManufacturer();

            // Developers Désactivé pour le moment
            //CreateTable($"CREATE TABLE [{tDeveloppers}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");

            // Constructors
            _SQLite_Op.Create_TConstructor();

            // Genres (liée à Games)
            _SQLite_Op.Create_TGenre();

            // Roms (Contenu réel des roms, feedé par l'utilisateur)
            _SQLite_Op.Create_TRom();

            // Table temporaire, feedée depuis un xml de M.A.M.E
            _SQLite_Op.Create_TTempRom();

            // Machine (lié aux roms)
            _SQLite_Op.Create_TMachine();

            // Information pour SQLITE
            _SQLite_Op.Create_TSqlInfo();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        internal static void Update_Structure(string fileName)
        {/*
            throw new NotImplementedException("A identifier");

            using (StreamReader stream = new StreamReader(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);

                _MaConn = new SQLiteConnection($"Data Source={PProp.Default.DataBase_Path};Version=3");
                _MaConn.Open();


                SQLiteCommand command = new SQLiteCommand(_MaConn);

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("#"))
                        continue;

                    command.CommandText = line;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exc)
                    {
                        Trace.WriteLine(exc.Message);
                    }
                }

                _MaConn.Close();
            }*/
        }


        /// <summary>
        /// Rajoute les champs supplémentaires aux tables
        /// </summary>
        private static void Alter_Structure()
        {
            // bios
            // mecanics
            // machines
            // roms
            // Temproms
            //
        }



        private static void Fill_Basics_Data()
        {
            /*string tDeveloppers = Properties.Settings.Default.T_Developers;
            string tGenres = Properties.Settings.Default.T_Genres;
            string tMachines = Properties.Settings.Default.T_Machines;*/

            // Constructeurs de Bornes (Feed manuel)
           /* List<CT_Constructor> constructors = new List<CT_Constructor>();/
            constructors.Add(new CT_Constructor() { ID = 1, Nom = "Amiga" });
            constructors.Add(new CT_Constructor() { ID = 2, Nom = "Atari" });
            constructors.Add(new CT_Constructor() { ID = 3, Nom = "Capcom" });
            constructors.Add(new CT_Constructor() { ID = 4, Nom = "Data East" });
            constructors.Add(new CT_Constructor() { ID = 5, Nom = "Konami" });
            constructors.Add(new CT_Constructor() { ID = 6, Nom = "Irem" });
            constructors.Add(new CT_Constructor() { ID = 7, Nom = "Midway" });
            constructors.Add(new CT_Constructor() { ID = 8, Nom = "Namco" });
            constructors.Add(new CT_Constructor() { ID = 9, Nom = "Sega" });
            constructors.Add(new CT_Constructor() { ID = 10, Nom = "SNK" });
            constructors.Add(new CT_Constructor() { ID = 11, Nom = "Taito" });
            constructors.Add(new CT_Constructor() { ID = 12, Nom = "Williams" });

            _SQLite_Op.Insert_Constructors(constructors, ignore: false, preservePK: true);*/


            // Genres
            List<CT_Genre> genres = new List<CT_Genre>();
            genres.Add(new CT_Genre() { ID=1, Nom= "Beat Them up" });
            genres.Add(new CT_Genre() { ID=2, Nom= "Fight" });
            genres.Add(new CT_Genre() { ID=3, Nom= "Platform" });
            genres.Add(new CT_Genre() { ID=4, Nom= "Puzzle" });
            genres.Add(new CT_Genre() { ID=5, Nom= "Shoot Them Up'" });

            _SQLite_Op.Insert_Genres(genres, ignore: false, preservePK:true);


            // Table Machines
            // Capcom
           /* CT_MameManufacturer ct = Query_One<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, $"SELECT [ID] FROM [{tConstructors}] WHERE [Nom]='Capcom'");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [MameCode], [MainCPU], [Constructeur], [Year]) VALUES (1, 'Capcom Play System 1', 'CPS1',,'CPS-1', {ct.ID}, 1988)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES (2, 'Capcom Play System 2', 'CPS-2', {ct.ID}, 1993)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES (3, 'Capcom Play System 3', 'CPS-3', {ct.ID}, 1996)");


            // Sega
            ct = Query_One<CT_MameManufacturer>(CT_MameManufacturer.Result2Class, $"SELECT [ID] FROM [{tConstructors}] WHERE [Nom]='Sega'");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 2'      , {ct.ID}, 1980)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 3'      , {ct.ID}, 1982)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 1'      , {ct.ID}, 1983)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('Appoooh'       , {ct.ID}, 1984)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System E'      , {ct.ID}, 1985)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('Hang-On'       , {ct.ID}, 1985)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 16'     , {ct.ID}, 1986)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('OutRun'        , {ct.ID}, 1986)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('X-Board'       , {ct.ID}, 1987)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('Y-Board'       , {ct.ID}, 1988)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 24'     , {ct.ID}, 1988)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System C'      , {ct.ID}, 1989)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 18'     , {ct.ID}, 1989)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System 32'     , {ct.ID}, 1990)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES ('Mega-Play' , 'MP',  {ct.ID}, 1991)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES ('Model 1'   , 'Model-1' ,{ct.ID}, 1992)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES ('Model 2'   , 'Model-2',{ct.ID}, 1993)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('Titan Video'   ,  {ct.ID}, 1994)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('H1-Board'      ,  {ct.ID}, 1995)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('Model 3'       ,  {ct.ID}, 1996)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES ('Naomi'     , 'Naomi', {ct.ID}, 1998)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [HardwareName], [Constructeur], [Year]) VALUES ('Naomi 2'   , 'Naomi-2', {ct.ID}, 2000)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('AtomisWave'    ,  {ct.ID}, 2003)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([ID], [Nom], [Constructeur], [Year]) VALUES ('System SP'     ,  {ct.ID}, 2005)");
           */
            // 





        }

        static internal T Query_One<T>(Func<Dictionary<string, object>, T> method, string reqSql)
        {
            throw new NotImplementedException("A voir à quoi ça servait");
            /*
            try
            {
                SQLiteCommand command = new SQLiteCommand(reqSql, _MaConn);
                SQLiteDataReader reader = command.ExecuteReader();

                reader.Read();

                Dictionary<string, object> dico = new Dictionary<string, object>();
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    dico.Add(reader.GetName(i), reader[i]);
                }

                T data = method(dico);
                if (data != null)
                    return data;

            }
            catch (SQLiteException exc)
            {
                Debug.WriteLine($"Erreur SQliteDb CreateTable: {reqSql} \n {exc.Message} \n");
            }*/
            return default(T);
        }






        /*
        static internal short RequeteNonQuery(string reqSql)
        {

            SQLiteCommand creatTables = new SQLiteCommand(reqSql, _MaConn);
            creatTables.ExecuteNonQuery();
            return 0;

        }*/


    }
}
