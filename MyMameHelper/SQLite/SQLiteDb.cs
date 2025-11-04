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
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.SQLite
{
    /// <summary>
    /// Contient les instructions pour créer la structure de la table
    /// </summary>
    class SQLiteDb
    {
        static SQLiteConnection _MaConn;

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

            _MaConn = new SQLiteConnection($"Data Source={dbLink};Version=3");

            string tMachines = Properties.Settings.Default.T_Machines;

            try
            {


                // Connexion
                _MaConn.Open();

                Create_Structure();
                Alter_Structure();
                Fill_Basics_Data();
                MessageBox.Show("Database Created");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                _MaConn.Close();
            }

        }


        /// <summary>
        /// Création de la structure de la base
        /// </summary>
        private static void Create_Structure()
        {
            string tGames = Properties.Settings.Default.T_Games;
            string tBios = Properties.Settings.Default.T_Bios;
            string tMechanicals = Properties.Settings.Default.T_Mechanics;
            string tConstructeurs = Properties.Settings.Default.T_Constructeurs;
            string tCompanies = Properties.Settings.Default.T_Developers;
            string tGenres = Properties.Settings.Default.T_Genres;
            string tMachines = Properties.Settings.Default.T_Machines;
            string tRoms = Properties.Settings.Default.T_Roms;
            string tempRoms = Properties.Settings.Default.T_TempRoms;

            // Création du minimum
            // Table Games (Elle permet des options en plus, personnalisables, pour les roms. Elle est liée à roms, genres)
            CreateTable($"CREATE Table [{tGames}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Game_Name] VARCHAR UNIQUE);");
            // Bios
            CreateTable($"CREATE Table [{tBios}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Bios_Name] VARCHAR UNIQUE);");
            // Mechanicals
            CreateTable($"CREATE Table [{tMechanicals}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Meca_Name] VARCHAR UNIQUE);");
            // Constructeurs
            CreateTable($"CREATE TABLE [{tConstructeurs}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            // Companies
            CreateTable($"CREATE TABLE [{tCompanies}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            // Genres (liée à Games)
            CreateTable($"CREATE TABLE [{tGenres}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            // Machines
            CreateTable($"CREATE TABLE [{tMachines}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            // Roms (Contenu réel des roms, feedé par l'utilisateur)
            CreateTable($"CREATE TABLE [{tRoms}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Archive_Name] VARCHAR UNIQUE);");
            // Table temporaire, feedée depuis un xml de M.A.M.E
            CreateTable($"CREATE TABLE [{tempRoms}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Name] VARCHAR UNIQUE);");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        internal static void Update_Structure(string fileName)
        {
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
            }
        }


        /// <summary>
        /// Rajoute les champs supplémentaires aux tables
        /// </summary>
        private static void Alter_Structure()
        {
            string tGames = Properties.Settings.Default.T_Games;
            string tBios = Properties.Settings.Default.T_Bios;
            string tMechanicals = Properties.Settings.Default.T_Mechanics;
            string tConstructeurs = Properties.Settings.Default.T_Constructeurs;
            string tMachines = Properties.Settings.Default.T_Machines;
            string tRoms = Properties.Settings.Default.T_Roms;
            string tempRoms = Properties.Settings.Default.T_TempRoms;

            AlterTable($"ALTER TABLE [{tGames}] ADD [Machine] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [Description] VARCHAR");
            #region Ancien système, délégué à présent à la table Roms
            //AlterTable($"ALTER TABLE [{tGames}] ADD [Roms] VARCHAR");
            #endregion
            AlterTable($"ALTER TABLE [{tGames}] ADD [Unwanted] BOOLEAN");
            AlterTable($"ALTER TABLE [{tGames}] ADD [Developer] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [Rate] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [Genre] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [IsMahJong] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [IsQuizz] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [IsPinball] INTEGER");
            AlterTable($"ALTER TABLE [{tGames}] ADD [IsFruit] INTEGER");
            // AlterTable($"ALTER TABLE [{tGames}] ADD [Description] VARCHAR");
            //AlterTable($"ALTER TABLE [{tGames}] ADD [Year] VARCHAR");

            // bios
            AlterTable($"ALTER TABLE [{tBios}] ADD [Description] INTEGER");

            // mecanics
            AlterTable($"ALTER TABLE [{tMechanicals}] ADD [Description] INTEGER");

            // machines
            AlterTable($"ALTER TABLE [{tMachines}] ADD [Revision] VARCHAR;");
            AlterTable($"ALTER TABLE [{tMachines}] ADD [Constructeur] INTEGER");
            AlterTable($"ALTER TABLE [{tMachines}] ADD [Year] INTEGER;");
            AlterTable($"ALTER TABLE [{tMachines}] ADD [AllowCPath] BOOLEAN;");

            // roms
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Description] VARCHAR;");
            #region Nouveau système, c'est ici qu'on va lier à games
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Game] INTEGER;");
            #endregion
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Year] VARCHAR;");
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Manufacturer] INTEGER;");
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Unwanted] BOOLEAN;");
            AlterTable($"ALTER TABLE [{tRoms}] ADD [IsParent] BOOLEAN;");
            AlterTable($"ALTER TABLE [{tRoms}] ADD [Clone_Of] INTEGER;");

            // Temproms
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Source_File] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Rom_Of] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Clone_Of] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Sample_Of] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Is_Bios] BOOLEAN");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Is_Mechanical] BOOLEAN");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Description] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Year] VARCHAR");
            AlterTable($"ALTER TABLE [{tempRoms}] ADD [Manufacturer] VARCHAR");

            //
        }

        private static void Fill_Basics_Data()
        {
            string tConstructeurs = Properties.Settings.Default.T_Constructeurs;
            string tGenres = Properties.Settings.Default.T_Genres;
            string tMachines = Properties.Settings.Default.T_Machines;

            RequeteNonQuery($"INSERT INTO [{tConstructeurs}] ([Nom])" +
                $"VALUES" +
                $"('Capcom')," +
                $"('Konami')," +
                $"('Sega')");


            RequeteNonQuery($"INSERT INTO [{tGenres}] ([Nom])" +
                $"VALUES" +
                $"('Shoot Them Up')," +
                $"('Fight');");

            // Table Constructeur
            CT_Constructeur ct = Query_One<CT_Constructeur>(CT_Constructeur.Result2Class, $"SELECT [ID] FROM [{tConstructeurs}] WHERE [Nom]='Sega'");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 1',  {ct.ID}, 1983)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Appoooh',  {ct.ID}, 1984)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 2',  {ct.ID}, 1985)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System E',  {ct.ID}, 1985)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Hang-On',  {ct.ID}, 1985)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 16',  {ct.ID}, 1986)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('OutRun',  {ct.ID}, 1986)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('X-Board',  {ct.ID}, 1987)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 24',  {ct.ID}, 1988)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Y-Board',  {ct.ID}, 1988)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 18',  {ct.ID}, 1989)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System C',  {ct.ID}, 1989)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Mega-Play',  {ct.ID}, 1991)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System 32',  {ct.ID}, 1990)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Titan Video',  {ct.ID}, 1994)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('H1-Board',  {ct.ID}, 1995)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Model 1',  {ct.ID}, 1992)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Model 2',  {ct.ID}, 1993)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Model 3',  {ct.ID}, 1996)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('AtomisWave',  {ct.ID}, 1)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Naomi',  {ct.ID}, 1998)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('Naomi 2',  {ct.ID}, 2000)");
            RequeteNonQuery($"INSERT INTO [{tMachines}] ([Nom], [Constructeur], [Year]) VALUES ('System SP',  {ct.ID}, 2005)");
        }

        static internal T Query_One<T>(Func<Dictionary<string, object>, T> method, string reqSql)
        {
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
            }
            return default(T);
        }

        static internal short CreateTable(string reqSql)
        {
            try
            {
                SQLiteCommand creatTables = new SQLiteCommand(reqSql, _MaConn);
                creatTables.ExecuteNonQuery();
                return 0;
            }
            catch (SQLiteException exc)
            {
                Debug.WriteLine($"Erreur SQliteDb CreateTable: {reqSql} \n {exc.Message} \n");
                return -1;
            }
        }

        static internal short AlterTable(string reqSql)
        {
            try
            {
                SQLiteCommand creatTables = new SQLiteCommand(reqSql, _MaConn);
                creatTables.ExecuteNonQuery();
                return 0;
            }
            catch (SQLiteException exc)
            {
                Debug.WriteLine($"Erreur SQliteDb AlterTable: {reqSql} \n {exc.Message} \n");
                return -1;
            }
        }


        static internal short RequeteNonQuery(string reqSql)
        {

            SQLiteCommand creatTables = new SQLiteCommand(reqSql, _MaConn);
            creatTables.ExecuteNonQuery();
            return 0;

        }


    }
}
