using MyMameHelper.ContTable;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_OP
    {
        internal short CreateTable(string reqSql)
        {
            try
            {
                SQLiteCommand creatTable = new SQLiteCommand(reqSql, SQLiteConn);
                Trace.WriteLine($"Exec: {creatTable.CommandText}");


                creatTable.ExecuteNonQuery();
                return 1;
            }
            catch (SQLiteException exc)
            {
                Debug.WriteLine($"Erreur SQliteDb CreateTable: {reqSql} \n {exc.Message} \n");
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqSql"></param>
        /// <returns></returns>
        internal short AlterTable(string reqSql)
        {
            try
            {
                SQLiteCommand alterTable = new SQLiteCommand(reqSql, SQLiteConn);
                Trace.WriteLine($"Exec: {alterTable.CommandText}");

                alterTable.ExecuteNonQuery();
                return 1;
            }
            catch (SQLiteException exc)
            {
                Debug.WriteLine($"Erreur SQliteDb AlterTable: {reqSql} \n {exc.Message} \n");
                return -1;
            }
        }

        /// <summary>
        /// Création de la table game
        /// </summary>
        /// <returns></returns>
        internal short Create_TGame()
        {


            short status = 0;
            status = CreateTable($"CREATE Table [{tGame}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Game_Name] VARCHAR UNIQUE);");
            if (status > 0)
            {


                AlterTable($"ALTER TABLE [{tGame}] ADD [Machine_Id] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [Description] VARCHAR");
                #region Ancien système, délégué à présent à la table Roms
                //AlterTable($"ALTER TABLE [{tGame}] ADD [Roms] VARCHAR");
                #endregion
                AlterTable($"ALTER TABLE [{tGame}] ADD [Unwanted] BOOLEAN");
                //AlterTable($"ALTER TABLE [{tGame}] ADD [Developer_Id] INTEGER"); Désactivé pour le moment
                AlterTable($"ALTER TABLE [{tGame}] ADD [Rate] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [Genre_Id] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [IsMahJong] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [IsQuizz] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [IsPinball] INTEGER");
                AlterTable($"ALTER TABLE [{tGame}] ADD [IsFruit] INTEGER");
                // AlterTable($"ALTER TABLE [{tGame}] ADD [Description] VARCHAR");
                //AlterTable($"ALTER TABLE [{tGame}] ADD [Year] VARCHAR");
            }

            return status;
        }




        internal short Create_TBios()
        {
            short status = 0;

            CreateTable($"CREATE Table [{tBios}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Bios_Name] VARCHAR UNIQUE);");
            if (status > 0)
            {
                AlterTable($"ALTER TABLE [{tBios}] ADD [Description] INTEGER");
            }

            return status;
        }

        internal short Create_TMechanical()
        {
            short status = 0;
            status = CreateTable($"CREATE Table [{tMechanical}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Meca_Name] VARCHAR UNIQUE);");
            if (status > 0)
            {
                AlterTable($"ALTER TABLE [{tMechanical}] ADD [Description] INTEGER");
            }
            return status;
        }

        internal short Create_TMameManufacturer()
        {
            short status = 0;

            status = CreateTable($"CREATE TABLE [{tMameManufacturer}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            if (status > 0) { }

            return status;
        }



        // Developers Désactivé pour le moment
        //CreateTable($"CREATE TABLE [{tDeveloppers}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");

        // Constructors
        internal short Create_TConstructor()
        {
            short status = 0;
            status = CreateTable($"CREATE TABLE [{tConstructor}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            if (status > 0) { }

            return status;
        }

        internal short Create_TGenre()
        {
            short status = 0;

            status = CreateTable($"CREATE TABLE [{tGenre}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");
            if (status > 0) { }

            return status;
        }

        internal short Create_TRom()
        {
            short status = 0;
            status = CreateTable($"CREATE TABLE [{tRom}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Archive_Name] VARCHAR UNIQUE);");
            if (status > 0)
            {
                AlterTable($"ALTER TABLE [{tRom}] ADD [Description] VARCHAR;");
                #region Nouveau système, c'est ici qu'on va lier à games
                AlterTable($"ALTER TABLE [{tRom}] ADD [Game_Id] INTEGER;");
                #endregion
                AlterTable($"ALTER TABLE [{tRom}] ADD [Year] VARCHAR;");
                AlterTable($"ALTER TABLE [{tRom}] ADD [Manufacturer] INTEGER;");
                AlterTable($"ALTER TABLE [{tRom}] ADD [Unwanted] BOOLEAN;");
                AlterTable($"ALTER TABLE [{tRom}] ADD [IsParent] BOOLEAN;");
                AlterTable($"ALTER TABLE [{tRom}] ADD [Clone_Of] INTEGER;");
            }

            return status;
        }

        internal short Create_TTempRom()
        {
            short status = 0;
            status = CreateTable($"CREATE TABLE [{tTempRom}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Name] VARCHAR UNIQUE);");
            if (status > 0)
            {
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Source_File] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Rom_Of] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Clone_Of] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Sample_Of] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Is_Bios] BOOLEAN");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Is_Mechanical] BOOLEAN");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Description] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Year] VARCHAR");
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [Manufacturer] VARCHAR");     // Correspond au champ xml manufacturer
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [HasSoftwares] BOOLEAN");     // S'il y a des software ce n'est pas un pcb = jeu, mais un hardware comme saturn, ...
                AlterTable($"ALTER TABLE [{tTempRom}] ADD [IsDevice] BOOLEAN");     // S'il y a des software ce n'est pas un pcb = jeu, mais un hardware comme saturn, ...
            }

            return status;
        }



        /// <summary>
        /// Création de la table machine
        /// </summary>
        /// <returns></returns>
        internal short Create_TMachine()
        {
            short status = 0;
            status = CreateTable($"CREATE TABLE [{tMachine}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Nom] VARCHAR UNIQUE);");

            if (status > 0)
            {
                AlterTable($"ALTER TABLE [{tMachine}] ADD [Revision] VARCHAR;");
                AlterTable($"ALTER TABLE [{tMachine}] ADD [HardwareName] VARCHAR;");    // CPS1, CPS2....
                AlterTable($"ALTER TABLE [{tMachine}] ADD [MameCode] VARCHAR;");        // Naomi, Sega16 ... dans sourcefile après /
                AlterTable($"ALTER TABLE [{tMachine}] ADD [MainCPU] VARCHAR;");         // z80,...
                AlterTable($"ALTER TABLE [{tMachine}] ADD [Constructeur_Id] INTEGER");
                AlterTable($"ALTER TABLE [{tMachine}] ADD [Year] INTEGER;");
                AlterTable($"ALTER TABLE [{tMachine}] ADD [AllowCPath] BOOLEAN;");
            }

            return status;

        }

        /// <summary>
        /// Information pour SQLITE         
        /// </summary>
        /// <returns></returns>
        internal short Create_TSqlInfo()
        {
            short status = 0;
            status = CreateTable($"CREATE TABLE [{tSQLInfo}] ([ID] INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, [Name] VARCHAR UNIQUE, [Valeur] VARCHAR);");
            if (status > 0) { }

            return status;
        }

    }
}
