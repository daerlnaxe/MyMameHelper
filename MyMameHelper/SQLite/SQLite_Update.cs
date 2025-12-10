using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Data.SqlTypes;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_OP
    {
        #region Update Unique
        /// <summary>
        /// Update Game
        /// </summary>
        /// <param name="gameCont"></param>
        internal void Update_Game(CT_Game gameCont)
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{tGame}]" +
                                    $"SET " +
                                    $"[Game_Name]=@Game_Name, " +
                                    $"[Description]=@Description, " +
                                    $"[Machine_Id]=@Machine, " +
                                    $"[Genre_Id]=@Genre, " +
                                    $"[Rate]=@Rate, " +
                                    $"[Unwanted]=@Unwanted, " +
                                    $"[IsMahjong]=@IsMahjong, " +
                                    $"[IsQuizz]=@IsQuizz " +
                                    $"[Developer_Id]=@Developer " +
                                    $"WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Game_Name", DbType.String).Value = gameCont.Game_Name;
            sqlCmd.Parameters.Add($"@Description", DbType.String).Value = gameCont.Description;
            sqlCmd.Parameters.Add($"@Machine_Id", DbType.UInt32).Value = gameCont.Machine_Id;
            sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = gameCont.Unwanted;
            sqlCmd.Parameters.Add($"@Genre_Id", DbType.UInt32).Value = gameCont.Genre_Id;
            sqlCmd.Parameters.Add($"@Rate", DbType.UInt32).Value = gameCont.Rate;
            sqlCmd.Parameters.Add($"@IsMahjong", DbType.Boolean).Value = gameCont.IsMahjong;
            sqlCmd.Parameters.Add($"@IsQuizz", DbType.Boolean).Value = gameCont.IsQuizz;
            //sqlCmd.Parameters.Add($"@Developer_Id", DbType.UInt32).Value = gameCont.Developer_Id; Levé pour le moment

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = gameCont.ID;

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// Update Genre
        /// </summary>
        /// <param name="ctGenre"></param>
        internal void Update_Genre(CT_Genre ctGenre)
        {
            Debug.WriteLine($"Update de {ctGenre.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{tGenre}] SET [Nom]=@Nom WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = ctGenre.Nom;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = ctGenre.ID;

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// Update Machine
        /// </summary>
        /// <param name="Machine"></param>
        public void Update_Machine(CT_Machine Machine)
        {
            Debug.WriteLine($"Update de {Machine.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{tMachine}] " +
                                    "SET [Nom]=@Nom, " +
                                    "[Constructeur]=@Constructeur, " +
                                    "[Year]=@Year, " +
                                    "[AllowCPath]=@AllowCPath " +
                                    //
                                    "WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = Machine.Nom;
            sqlCmd.Parameters.Add($"@Constructeur", DbType.String).Value = Machine.IDConstructeur;
            sqlCmd.Parameters.Add($"@Year", DbType.UInt32).Value = Machine.Year;
            sqlCmd.Parameters.Add($"@AllowCPath", DbType.Boolean).Value = Machine.AllowCPath;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = Machine.ID;

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// Update MameManufacturer
        /// </summary>              
        internal void Update_MameManufacturer(CT_MameManufacturer ctConst)
        {
            Debug.WriteLine($"Update de {ctConst.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{tMameManufacturer}] SET [Nom]=@Nom WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = ctConst.Nom;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = ctConst.ID;

            ExecNQ(sqlCmd);
        }

        #endregion Update Unique


        #region Update Collection

        /// <summary>
        /// Update par l'ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Games"></param>
        public void Update_Games<T>(IList<T> Games) where T : iCT_Games
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);



            for (int i = 0; i < Games.Count; i++)
            {
                T game = Games[i];
                //  string vals = null;

                sqlCmd.CommandText = $"UPDATE [{tGame}]" +
                                        $" SET" +
                                        $" [Game_Name]=@Game_Name" +
                                        $" ,[Description]=@Description" +
                                        $" ,[Machine_Id]=@Machine_Id" +
                                        $" ,[Genre_Id]=@Genre_Id" +
                                        $" ,[Rate]=@Rate" +
                                        $" ,[Unwanted]=@Unwanted" +
                                        $" ,[IsMahjong]=@IsMahjong" +
                                        $" ,[IsQuizz]=@IsQuizz" +
                                        $" ,[Developer_Id]=@Developer_Id" +
                                        $" WHERE ID=@ID";

                sqlCmd.Parameters.Add($"@Game_Name", DbType.String).Value = Games[i].Game_Name;
                sqlCmd.Parameters.Add($"@Description", DbType.String).Value = Games[i].Description;
                sqlCmd.Parameters.Add($"@Machine_Id", DbType.UInt32).Value = Games[i].Machine_Id;
                sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = Games[i].Unwanted;
                sqlCmd.Parameters.Add($"@Genre_Id", DbType.UInt32).Value = Games[i].Genre_Id;
                sqlCmd.Parameters.Add($"@Rate", DbType.UInt32).Value = Games[i].Rate;
                sqlCmd.Parameters.Add($"@IsMahjong", DbType.Boolean).Value = Games[i].IsMahjong;
                sqlCmd.Parameters.Add($"@IsQuizz", DbType.Boolean).Value = Games[i].IsQuizz;
                //sqlCmd.Parameters.Add($"@Developer_Id", DbType.UInt32).Value = Games[i].Developer_Id; Levé pour le moment

                // condition
                sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = Games[i].ID;

                Debug.WriteLine($"Update_Games: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Games.Count);



            }

        }


        /// <summary>
        /// Update sans limitation selon des conditions
        /// </summary>
        internal void Update_MassiveRoms(IList<SQL_Element> sqlElems, SqlCond[] conditions)
        {
            string sql = $"UPDATE [{tRom}] SET";

            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            foreach (var elem in sqlElems)
            {
                sql += $" [{elem.column}]=@{elem.column},";

                Type t = elem.type;


                if (t == typeof(string))
                    sqlCmd.Parameters.Add($"@{elem.column}", DbType.String).Value = elem.value;
                else if (t == typeof(uint))
                    sqlCmd.Parameters.Add($"@{elem.column}", DbType.UInt16).Value = elem.value;
            }

            sql = sql.Substring(0, sql.Length - 1);


            sqlCmd.CommandText = sql;

            Condition_TreatMt(sqlCmd, conditions);

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks> clone et isparent ont été levés</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="Roms"></param>
        public void Update_Roms<T>(IList<T> Roms) where T : iCT_Rom
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Roms.Count; i++)
            {
                T rom = Roms[i];
                //  string vals = null;

                sqlCmd.CommandText = $"UPDATE [{tRom}]" +
                                        $"SET " +
                                        $"[Archive_Name]=@Archive_Name, " +
                                        $"[Description]=@Description, " +
                                        $"[Game_Id]=@Game_Id, " +
                                        $"[Year]=@Year, " +
                                        $"[Manufacturer_Id]=@Manufacturer_Id, " +
                                        $"[Unwanted]=@Unwanted " +
                                        //
                                        $"WHERE ID=@ID";

                sqlCmd.Parameters.Add($"@Archive_Name", DbType.String).Value = rom.Archive_Name;
                sqlCmd.Parameters.Add($"@Description", DbType.String).Value = rom.Description;
                if (rom.Game_Id != null)
                {
                    sqlCmd.Parameters.Add($"@Game_Id", DbType.UInt32).Value = rom.Game_Id;
                }
                else
                {
                    sqlCmd.Parameters.Add($"@Game_Id", DbType.UInt16).Value = null;
                }
                sqlCmd.Parameters.Add($"@Year", DbType.String).Value = rom.Year;
                sqlCmd.Parameters.Add($"@Manufacturer_Id", DbType.UInt32).Value = rom.Manufacturer.ID;
                sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = rom.Unwanted;

                // condition
                sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = rom.ID;

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Roms.Count);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Roms"></param>
        /// <remarks>
        /// Beaucoup plus rapide
        /// </remarks>
        public void Update_MassiveRoms<T>(IList<T> roms, params string[] fields) where T : iCT_Rom
        {
            Debug.WriteLine($"Update massif de la collection de roms");

            // Début de la transaction
            using (var transaction = SQLiteConn.BeginTransaction())
            {
                using (var sqlCmd = SQLiteConn.CreateCommand())
                {
                    sqlCmd.CommandText = $"UPDATE [{tRom}]" +
                        $"SET " +
                        $"[Archive_Name]=@Archive_Name, " +
                        $"[Description]=@Description, " +
                        $"[Game_Id]=@Game_Id, " +
                        $"[Year]=@Year, " +
                        $"[Manufacturer_Id]=@Manufacturer_Id, " +
                        $"[Unwanted]=@Unwanted " +
                    //
                        $"WHERE ID=@ID";

                    // Paramètres
                    //-- Archives
                    var pArchiveName = sqlCmd.CreateParameter();
                    pArchiveName.ParameterName="@Archive_Name";
                    pArchiveName.DbType= DbType.String;
                    sqlCmd.Parameters.Add(pArchiveName);


                    // Condition
                    var pId = sqlCmd.CreateParameter();
                    pId.ParameterName = "@ID";
                    pId.DbType= DbType.Int32;
                    sqlCmd.Parameters.Add(pId);


                    //---- A continuer
                    sqlCmd.Parameters.Add($"@Description", DbType.String).Value = rom.Description;
                    if (rom.Game_Id != null)
                    {
                        sqlCmd.Parameters.Add($"@Game_Id", DbType.UInt32).Value = rom.Game_Id;
                    }
                    else
                    {
                        sqlCmd.Parameters.Add($"@Game_Id", DbType.UInt16).Value = null;
                    }
                    sqlCmd.Parameters.Add($"@Year", DbType.String).Value = rom.Year;
                    sqlCmd.Parameters.Add($"@Manufacturer_Id", DbType.UInt32).Value = rom.Manufacturer.ID;
                    sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = rom.Unwanted;


                    
                    
                    
                    //---



     

                    sqlCmd.Prepare(); // compile le SQL une fois

                    foreach (var rom in roms)
                    {
                        //sqlCmd.Parameters.Add($"@Archive_Name", ).Value = rom.Archive_Name;
                        pArchiveName.Value  = rom.Archive_Name;
                        
                        
                        
                        // condition
                        pId.Value = rom.ID;

                        sqlCmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

        }

        #endregion
    }
}
