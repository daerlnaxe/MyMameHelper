using DxTBoxWPF.MBox;
using MyMameHelper.ContTable;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.SQLite
{
    /// <summary>
    /// ??, ouvrir une connexion
    /// </summary>
    public sealed partial class SQLite_Op
    {
        #region commun


        string tRom = PProp.Default.T_Roms;
        string tGenre = PProp.Default.T_Genres;
        string tMachine = PProp.Default.T_Machines;
        string tGame = PProp.Default.T_Games;
        string tManufacturer = PProp.Default.T_Manufacturers;
        string tDeveloper = PProp.Default.T_Developers;
        #endregion

        #region 1 Element
        public CT_Constructeur Get_Companie(SqlCond[] conds)
        {
            Obj_Select objCompanie = new Obj_Select(table: PProp.Default.T_Developers, all: true);
            objCompanie.AddConds(conds);

            return Get_OneResult<CT_Constructeur>(CT_Constructeur.Result2Class, objCompanie);
        }

        private T Get_OneResult<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect)
        {
            Debug.WriteLine($"Get_OneResult: {typeof(T)}");
            objSelect.Limit = 1;

            T objet = default(T);

            SQLiteDataReader reader = this.ResultSelect(objSelect);

            if (reader.HasRows)
            {
                reader.Read();

                Dictionary<string, object> dico = new Dictionary<string, object>();
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    dico.Add(reader.GetName(i), reader[i]);
                }

                objet = method(dico);
            }

            return objet;
        }
        #endregion


        #region ObservableCollection
        public ObservableCollection<CT_Constructeur> GetById_CollecConstructeurs(int id, string order = "Nom")
        {
            ObservableCollection<CT_Constructeur> collec = new ObservableCollection<CT_Constructeur>();

            Obj_Select objSelect = new Obj_Select(table: PProp.Default.T_Manufacturers, all: true);
            objSelect.Conditions = new SqlCond[] { new SqlCond { Colonne = "ID", Operateur = eWhere.Equal, Valeur = id.ToString() } };
            objSelect.Orders = new SqlOrder[] { new SqlOrder("Nom") };

            return new ObservableCollection<CT_Constructeur>(GetCollectionOf<CT_Constructeur>(CT_Constructeur.Result2Class, objSelect));
        }


        #endregion




        #region generic

        /// <summary>
        /// Méthode générique
        /// </summary>
        /// <param name="table"></param>
        /// <param name="colonnes"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public SQLiteDataReader ResultSelect(Obj_Select objSelect)
        {
            try
            {
                string cols2Sel;
                if (objSelect.All)
                {
                    cols2Sel = "*";
                }
                else
                {
                    cols2Sel = string.Join(", ", objSelect.Colonnes.Select(x => $"[{x}]"));
                }

                string sql = $"SELECT {cols2Sel} FROM \"{objSelect.Table}\"";

                SQLiteCommand command = new SQLiteCommand(sql, this.SQLiteConn);

                Condition_TreatMt(command, objSelect.Conditions);
                Group_TreatMt(command, objSelect.Groups);
                Order_TreatMt(command, objSelect.Orders);

                if (objSelect.Limit > 0)
                    command.CommandText += $" LIMIT {objSelect.Limit}";

                Trace.WriteLine($"Lancement de la commande {command.CommandText}");
                Trace.WriteLine(this.SQLiteConn.State);
                SQLiteDataReader reader = command.ExecuteReader();
                Trace.WriteLine($"Nombre de résultats, {reader.RecordsAffected}");
                return reader;
                /*
                if (reader.HasRows)
                {
                    return reader;
                }*/
            }
            catch (SQLiteException exc)
            {
                DxMBox.ShowDial($"Erreur d'accès TABLE {objSelect.Table} \n {exc.Message}", "Erreur", DxTBoxWPF.Common.DxButtons.Ok);

                Dispose();
            }
            catch (Exception exc)
            {

                DxMBox.ShowDial($"Erreur d'accès TABLE {objSelect.Table} \n {exc.Message}", "Erreur", DxTBoxWPF.Common.DxButtons.Ok);
                Dispose();
            }

            return null;
        }


        public T GetOneResult<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect)
        {
            objSelect.Limit = 1;

            try
            {
                var reader = ResultSelect(objSelect);

                // Récupération des colonnes
                reader.Read();
                //
                Dictionary<string, object> dico = new Dictionary<string, object>();
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    dico.Add(reader.GetName(i), reader[i]);
                }

                //
                T data = method(dico);
                if (data != null)
                    return data;

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                Dispose();
            }

            return default(T);
        }

        /// <summary>
        /// Methode générique de collecte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="objSelect"></param>
        /// <param name="obsCollec"></param>
        /// <param name="reset"></param>
        public IList<T> GetCollectionOf<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect)
        {
            Debug.WriteLine($"GetCollectionOf: {typeof(T)}");

            List<T> obsCollec = new List<T>();


            SQLiteDataReader reader = this.ResultSelect(objSelect);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                    T data = method(dico);
                    if (data == null)
                        continue;

                    obsCollec.Add(data);
                }
            }

            return obsCollec;
        }



        /// <summary>
        /// Renvoie sous forme de liste le résultat de la requête envoyée
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">Methode d'interprétation des données</param>
        /// <param name="objSelect">L'objet qui va lancer la requête</param>
        /// <remarks>Filtre les nulls?</remarks>
        /// <returns></returns>
        public List<T> GetListOf<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect)
        {
            Debug.WriteLine($"GetListOf: {typeof(T)}");
            List<T> lCollec = new List<T>();

            try
            {
                SQLiteDataReader reader = ResultSelect(objSelect);

                // Récupération des colonnes
                while (reader.Read())
                {
                    //
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                    //
                    T data = method(dico);
                    if (data == null)
                        continue;

                    lCollec.Add(data);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                Dispose();
            }

            return lCollec;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objSelect"></param>
        /// <returns></returns>
        public List<string> GetStringOf(Obj_Select objSelect)
        {
            List<string> lCollec = new List<string>();
            objSelect.All = false;

            try
            {
                string colonne = string.Join(", ", objSelect.Colonnes.Select(x => $"[{x}]"));

                string sql = $"SELECT {colonne} FROM [{objSelect.Table}]";
                SQLiteCommand command = new SQLiteCommand(sql, this.SQLiteConn);

                Condition_TreatMt(command, objSelect.Conditions);
                Group_TreatMt(command, objSelect.Groups);
                Order_TreatMt(command, objSelect.Orders);

                if (objSelect.Limit > 0)
                    command.CommandText += $" LIMIT {objSelect.Limit}";

                Trace.WriteLine($"Lancement de la commande {command.CommandText}");
                SQLiteDataReader reader = command.ExecuteReader();

                Trace.WriteLine($"Nombre de résultats, {reader.RecordsAffected}");

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0].ToString());

                        lCollec.Add(reader[0].ToString());
                    }

                }

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                Dispose();
            }


            return lCollec;
        }


        public Dictionary<string, T> GetDictOf<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect, string key)
        {
            Debug.WriteLine($"GetDictOf: {typeof(T)}");
            Dictionary<string, T> lCollec = new Dictionary<string, T>();

            try
            {
                var reader = ResultSelect(objSelect);

                // Récupération des colonnes
                while (reader.Read())
                {
                    //
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                    //
                    T data = method(dico);
                    if (data == null) continue;
                    lCollec.Add(dico[key].ToString(), data);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                Dispose();
            }
            return lCollec;
        }


        /// <summary>
        /// Renvoie sous forme d'observable collection de paire de clés.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="col"></param>
        /// <param name="table"></param>
        /// <param name="All"></param>
        /// <param name="colonnes"></param>
        /// <param name="conditions"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public ObservableCollection<KeyValuePair<string, object>> GetKVPOf<T>(Func<Dictionary<string, object>, T> method, string col, Obj_Select objSelect, ObservableCollection<KeyValuePair<string, object>> obsCollec = null)
        {
            if (obsCollec == null)
            {
                obsCollec = new ObservableCollection<KeyValuePair<string, object>>();
                Debug.WriteLine("GetCollectionOf: Création d'une nouvelle collection");
            }
            else
            {
                Debug.WriteLine("GetCollectionOf: RAZ de la collection");
                obsCollec.Clear();
            }
            var reader = ResultSelect(objSelect);
            while (reader.Read())
            {
                Dictionary<string, object> dico = new Dictionary<string, object>();
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    dico.Add(reader.GetName(i), reader[i]);
                }

                T data = method(dico);
                if (data == null)
                    continue;

                obsCollec.Add(new KeyValuePair<string, object>(key: dico[col].ToString(), value: data));
            }

            return obsCollec;
        }


        public void RefreshKVPOf<T>(ObservableCollection<KeyValuePair<string, object>> obsResult, Func<Dictionary<string, object>, T> method, string col, Obj_Select objSelect)
        {
            var reader = ResultSelect(objSelect);
            while (reader.Read())
            {
                Dictionary<string, object> dico = new Dictionary<string, object>();
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    dico.Add(reader.GetName(i), reader[i]);
                }

                T data = method(dico);
                if (data == null) continue;
                obsResult.Add(new KeyValuePair<string, object>(key: dico[col].ToString(), value: data));
            }
        }
        #endregion


        #region Méthodes Type
        /// <summary>
        /// Compter les valeurs d'une table
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="where"></param>
        /// <remarks>Penser à mettre [champ] et surtout pas 'champ', % correspond à *</remarks>
        /// <returns></returns>
        public int Count(string Table, SqlCond[] conditions = null)
        {

            string sql = $"SELECT COUNT (*) FROM [{Table}]";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            int rows = Convert.ToInt32(command.ExecuteScalar());

            return rows;
        }


        /// <summary>
        /// Récupère la valeur la plus haute d'un champ
        /// </summary>
        /// <param name="table"></param>
        /// <param name="champ"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public int MaxValue(string table, string champ, SqlCond[] conditions = null)
        {
            string sql = $"SELECT MAX({champ}) FROM [{table}]";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            int resultat = Convert.ToInt32(command.ExecuteScalar());

            return resultat;
        }


        /// <summary>
        /// Récupère les noms des tables
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <remarks>Penser à mettre [champ] et surtout pas 'champ', % correspond à *</remarks>
        /// <returns></returns>
        /// <summary>
        /// Liste des tables de la base de donnée sauf sqlite_sequence
        /// </summary>
        /// <remarks>"sqlite_sequence" filtré</remarks>
        /// <returns></returns>
        public List<string> GET_TablesName()
        {
            List<string> listTable = new List<string>();
            string sql = "SELECT [name] FROM sqlite_master WHERE type = 'table'";

            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                switch (reader["Name"])
                {
                    case "sqlite_sequence":
                        break;
                    default:
                        listTable.Add(reader["Name"].ToString());
                        break;
                }
            }

            return listTable;
        }


        /// <summary>
        ///  Verifie la présence d'une donnée
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public bool? Verif_Me(string table, SqlCond[] conditions)
        {
            string sql = $"SELECT * FROM [{table}]";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            var result = command.ExecuteScalar();


            if (result == null) return false;
            return true;
        }


        /// <summary>
        /// Renvoie la dernière id d'une table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public uint? GetLastID(string table_name)
        {
            uint? lastID = null;

            string sql = $"SELECT seq FROM sqlite_sequence WHERE name = '{table_name}'";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);
            var yop = command.ExecuteScalar();
            lastID = Convert.ToUInt32(yop);

            return lastID;
        }


        public T GetLast<T>(Obj_Select objSelect, Func<Dictionary<string, object>, T> method)
        {
            if (objSelect.Orders == null) objSelect.AddOrders(new SqlOrder(Sens.Desc, "ID"));
            //objSelect.Limit = 1;
            T data = default(T);
            //SQLiteDataReader reader = ResultSelect(table_name, All, colonnes);

            try
            {
                string cols2Sel;
                if (objSelect.All)
                {
                    cols2Sel = "*";
                }
                else
                {
                    cols2Sel = string.Join(", ", objSelect.Colonnes.Select(x => $"[{x}]"));
                }

                string sql = $"SELECT {cols2Sel} FROM [{objSelect.Table}]";
                SQLiteCommand command = new SQLiteCommand(sql, this.SQLiteConn);

                //Condition_TreatMt(command, conditions);
                //Group_TreatMt(command, groups);
                Order_TreatMt(command, objSelect.Orders);
                command.CommandText += " LIMIT 1";

                SQLiteDataReader reader = command.ExecuteReader();
                Console.WriteLine($"{command.CommandText} exécutée, {reader.RecordsAffected}");

                while (reader.Read())
                {
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                    data = method(dico);

                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }


            return data;
        }
        #endregion


        #region Aff Machine
        private SQLiteDataReader AffMachine_SQL(SqlCond[] conds, SqlOrder order)
        {
            string constructeurs = PProp.Default.T_Manufacturers;
            //string machines = PProp.Default.T_Machines;

            Dictionary<string, short> dicCol;
            string sql = $"SELECT *, [{constructeurs}].[Nom] AS [Aff_Constructeur] FROM [{tMachine}] " +
                            $"LEFT JOIN [{constructeurs}] ON [{tMachine}].[Constructeur] = [{constructeurs}].[ID] ";


            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(sqlCMD, conds);
            Order_TreatMt(sqlCMD, order);

            Debug.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }

        private Aff_Machine AffMachine_Maker(SQLiteDataReader reader)
        {
            Aff_Machine machine = new Aff_Machine();

            machine.ID = Trans.GetUInt("ID", reader);
            machine.Nom = Trans.GetString("Nom", reader);
            machine.Constructeur = Trans.GetString("Aff_Constructeur", reader);
            machine.IDConstructeur = Trans.GetUInt("Constructeur", reader);
            machine.Revision = Trans.GetString("Revision", reader);
            machine.Year = Trans.GetUShort("Year", reader);
            machine.AllowCPath = Trans.GetBool("AllowCPath", reader);

            return machine;
        }

        /// <summary>
        /// Retourne la liste des machines en version affichable
        /// </summary>
        /// <returns></returns>
        public List<Aff_Machine> List_MachinesJoin(SqlCond[] conds = null, SqlOrder order = null)
        {
            List<Aff_Machine> lMachines = new List<Aff_Machine>();

            SQLiteDataReader reader = AffMachine_SQL(conds, order);

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    lMachines.Add(AffMachine_Maker(reader));
                }
            }

            return lMachines;
        }

        public Dictionary<string, Aff_Machine> Dict_MachinesJoin(SqlCond[] conds = null, SqlOrder order = null)
        {
            Dictionary<string, Aff_Machine> lMachines = new Dictionary<string, Aff_Machine>();

            SQLiteDataReader reader = AffMachine_SQL(conds, order);

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    Aff_Machine machine = AffMachine_Maker(reader);
                    lMachines.Add(machine.ID.ToString(), machine);
                }
            }

            return lMachines;
        }

        #endregion Aff Machine


        #region Games

        public List<CT_Game> Get_ListOf_Games(Obj_Select objS)
        {
            objS.Table = PProp.Default.T_Games;
            return GetListOf<CT_Game>(CT_Game.Result2Class, objS);

        }

        #endregion Games


        #region AffGames




        #endregion


        #region Games
        private CT_Game Game_Maker(SQLiteDataReader reader)
        {

            CT_Game Ag = new CT_Game();

            Ag.ID = Trans.GetUInt("ID", reader);

            Ag.Game_Name = Trans.GetString("Game_Name", reader);


            Ag.Description = Trans.GetString("Description", reader);
            Ag.Machine_Id = Trans.GetNullableUInt("Machine", reader);
            Ag.Unwanted = Trans.GetBool("Unwanted", reader);
            Ag.Genre_Id = Trans.GetNullableUInt("Genre", reader);
            Ag.Rate = Trans.GetUInt("Rate", reader);
            Ag.IsMahjong = Trans.GetBoolFalse("IsMahjong", reader);
            Ag.IsQuizz = Trans.GetBoolFalse("IsQuizz", reader);


            return Ag;

        }


        /// <summary>
        /// Renvoie une liste de roms selon la concaténation prévue | et ♢
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<CT_Rom> RomMapping(SQLiteDataReader reader)
        {
            List<CT_Rom> tmp = new List<CT_Rom>();

            //Roms
            var strRoms = Trans.GetString("Roms", reader);
            if (!string.IsNullOrEmpty(strRoms))
            {


                var arrTmp = strRoms.Split('|');
                for (int i = 0; i < arrTmp.Length; i++)
                {
                    string[] curr = arrTmp[i].Split('♢');

                    tmp.Add(
                        new CT_Rom()
                        {
                            ID = uint.Parse(curr[0]),
                            Archive_Name = curr[1],
                            Description = curr[2]
                        }
                    );
                }

            }

            return tmp;
        }


        /// <summary>
        /// Request to have a list of roms with jointure on games.
        /// </summary>
        /// <param name="conds"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private SQLiteDataReader Select_GameWithRoms(SqlCond[] conds, SqlOrder[] orders)
        {
            Dictionary<string, short> dicCol;
            //string sql = $"SELECT [{tRoms}]*, [{tMachine}].Nom AS Aff_Machine, [{tGenre}].Nom AS Aff_Genre " +
            string sql = $"SELECT [{tGame}].ID, [{tGame}].Game_Name  , " +
                //$"(SELECT group_concat([{tRom}].Archive_Name, '|') " +
                $"(SELECT group_concat(Roms.ID || '♢' || Roms.Archive_Name || '♢' || Roms.Description, '|')" +
                    $"FROM [{tRom}] " +
                    $"WHERE [{tRom}].Game_Id=[{tGame}].ID) AS \"Roms\"" +
                $" FROM [{tGame}]";


            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
            Condition_TreatMt(sqlCMD, conds);
            Order_TreatMt(sqlCMD, orders);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }


        }

        /// <summary>
        /// Retourne les jeux totalement mappés
        /// </summary>
        /// <returns></returns>
        private SQLiteDataReader Select_GameFullM()
        {
            Dictionary<string, short> dicCol;
            //string sql = $"SELECT [{tRoms}]*, [{tMachine}].Nom AS Aff_Machine, [{tGenre}].Nom AS Aff_Genre " +
            string sql = $"SELECT [{tGame}].*" +
                            $", [{tDeveloper}].Nom AS \"Developer\"" +
                            $", [{tMachine}].Nom AS \"Machine\"" +
                            $", [{tGenre}].Nom AS \"Genre\"" +
                            //$"(SELECT group_concat([{tRom}].Archive_Name, '|') " +
                            /*$"(SELECT group_concat(Roms.ID || '♢' || Roms.Archive_Name || '♢' || Roms.Description, '|')" +
                            /    $"FROM [{tRom}] " +
                            /    $"WHERE [{tRom}].Game_Id=[{tGame}].ID) AS \"Roms\"" +*/
                            $" FROM [{tGame}]" +
                            $" LEFT JOIN [{tDeveloper}] ON [{tDeveloper}].ID=[{tGame}].Developer_Id" +
                            $" LEFT JOIN [{tMachine}] ON [{tMachine}].ID=[{tGame}].Machine_Id" +
                            $" LEFT JOIN [{tGenre}] ON [{tGenre}].ID=[{tGame}].Genre_Id" +
                            $"";


            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
            /* Condition_TreatMt(sqlCMD, conds);
             Order_TreatMt(sqlCMD, orders);*/

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }

        }



        /// <summary>
        /// Get a list of Games with Roms
        /// </summary>
        /// <remarks>
        /// Need a request with roms separated by '|'
        /// No other jointure
        /// </remarks>
        /// <returns></returns>
        internal List<CT_Game_Mapped> QueryGameWithRoms()
        {
            List<CT_Game_Mapped> lGames = new List<CT_Game_Mapped>();
            SQLiteDataReader reader = Select_GameWithRoms(null, null);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CT_Game_Mapped game = new CT_Game_Mapped(Game_Maker(reader));

                    game.Roms = RomMapping(reader);

                    lGames.Add(game);
                }
            }

            return lGames;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="conds"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<CT_Game_Mapped> QueryGame4Update(SqlCond[] conds = null, SqlOrder order = null)
        {
            Debug.WriteLine($"Requête par {nameof(QueryGame4Update)}");

            List<CT_Game_Mapped> lGames = new List<CT_Game_Mapped>();
            SQLiteDataReader reader = Select_GameFullM();

            //List<CT_Rom> romsList = GetListOf(CT_Rom.Result2Class, new Obj_Select(PProp.Default.T_Roms, all: true));

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    CT_Game_Mapped game = new CT_Game_Mapped(Game_Maker(reader));
                    game.Genre_Id = Trans.GetNullableUInt("Genre_Id", reader);
                    game.Developer_Id = Trans.GetNullableUInt("Developer_Id", reader);

                    // Genre                                       
                    game.Genre = new CT_Genre();
                    game.Genre.ID = Convert.ToUInt16(game.Genre_Id = Trans.GetNullableUInt("Developer_Id", reader));
                    game.Genre.Nom = Trans.GetString("Genre", reader);

                    // Machine                    
                    game.Machine = new CT_Machine();
                    game.Machine.ID = Convert.ToUInt16(game.Machine_Id = Trans.GetNullableUInt("Machine_Id", reader));
                    game.Machine.Nom = Trans.GetString("Machine", reader);

                    // Developer                    
                    game.Developer = new CT_Developer();
                    game.Developer.ID = Convert.ToUInt16(game.Developer_Id = Trans.GetNullableUInt("Developer_Id", reader));
                    game.Developer.Nom = Trans.GetString("Developer", reader);

                    game.Roms = RomMapping(reader);
                    //   game.Machine = CT_Machine.Result2Class(reader);

                    //game.Machine_id


                    lGames.Add(game);
                }
            }

            return lGames;
        }


        /*
        internal SQLiteDataReader AffGamesLink_SQL()
        {
            //string constructeurs = PProp.Default.T_Constructeurs;
            //string tManufacturer = PProp.Default.T_Manufacturers;                            

            Dictionary<string, short> dicCol;
            //string sql = $"SELECT [{tRoms}]*, [{tMachine}].Nom AS Aff_Machine, [{tGenre}].Nom AS Aff_Genre " +
            string sql = $"SELECT [{tRoms}].*, " +
                            $" FROM [{tRoms}]" +
                            $"";
            /*$"LEFT JOIN [{tMachine}] ON Machine = [{tMachine}].ID " +
            $"LEFT JOIN [{tGenre}] ON Genre = [{tGenre}].ID " +*/

        /*

            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
            Condition_TreatMt(sqlCMD, conds);
            Order_TreatMt(sqlCMD, order);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SQLiteDataReader SimpleGames_SQL()
        {
            string sql = $"Select [{tGame}].* " +
                $"FROM [{tGame}]";

            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");


            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }





        #region Deprecated


        #endregion Deprecated

        /// <summary>
        /// Récupère l'id des jeux en fonction d'une de ses roms
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, string> Get_IdRoms(uint romID)
        {
            Dictionary<uint, string> dicoRet = new Dictionary<uint, string>();

            string sql = $"SELECT [ID], [Roms] FROM {PProp.Default.T_Games} WHERE [Roms]LIKE'%{romID}%'";
            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                var reader = sqlCMD.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        uint ID = Convert.ToUInt32(reader.GetValue(0));
                        dicoRet.Add(ID, reader.GetString(1));
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }

            return dicoRet;
        }

        #endregion


        #region Roms

        internal void GetList_RawRoms(Obj_Select objSel)
        {
            SQLiteCommand sqlCommand = new SQLiteCommand(SQLiteConn);
            sqlCommand.CommandText = $"SELECT * FROM {PProp.Default.T_TempRoms} WHERE [Is_Bios]='True'";

            var reader = sqlCommand.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                }
            }
        }

        #endregion roms


        #region Aff Roms
        /// <summary>
        /// Selection des roms avec jointure à gauche avec les développers.
        /// </summary>
        /// <param name="conds"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private SQLiteDataReader AffRoms_SQL(SqlCond[] conds, SqlOrder order)
        {
            //string constructeurs = PProp.Default.T_Constructeurs;

            Dictionary<string, short> dicCol;
            string sql = $"SELECT [{tRom}].*, [{tManufacturer}].Nom AS Aff_Manufacturer " +
                            $"FROM [{tRom}] " +
                            $"LEFT JOIN [{tManufacturer}] ON Manufacturer = [{tManufacturer}].ID " +

                           "";

            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
            Condition_TreatMt(sqlCMD, conds);
            Order_TreatMt(sqlCMD, order);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private CT_Rom AffRom_Maker(SQLiteDataReader reader)
        {
            CT_Rom Ag = new CT_Rom();

            Ag.ID = Trans.GetUInt("ID", reader);
            Ag.Archive_Name = Trans.GetString("Archive_Name", reader);
            Ag.Description = Trans.GetString("Description", reader);
            Ag.Game_Id = Trans.GetUInt("Game_Id", reader);
            Ag.Year = Trans.GetString("Year", reader);

            // Modifié le 09/11/2025 car transformation en objet
            //Ag.Manufacturer = Trans.GetUInt("Manufacturer", reader);

            /*
            CT_Constructeur ctc = new CT_Constructeur()
            {
                Trans.GetUInt()
            }
            */

            Ag.Unwanted = Trans.GetBool("Unwanted", reader);
            Ag.IsParent = Trans.GetBool("IsParent", reader);
            Ag.Clone_Of = Trans.GetUInt("Clone_Of", reader);
            //
            //Ag.Aff_Manufacturer = Trans.GetString("Aff_Manufacturer", reader);


            return Ag;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="conds"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<CT_Rom> AffRoms_List(SqlCond[] conds = null, SqlOrder order = null)
        {
            List<CT_Rom> lGames = new List<CT_Rom>();
            SQLiteDataReader reader = AffRoms_SQL(conds, order);

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    CT_Rom aG = AffRom_Maker(reader);

                    lGames.Add(aG);
                }
            }

            return lGames;
        }

        #endregion Aff Roms


        #region MappedRoms
        /// <summary>
        /// Rom mappées spécialement pour le déplacement
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Jointures optimisées pour le déplacement des roms
        /// - Game
        /// - Machine
        /// Faire évoluer si nécessaire
        /// </remarks>
        private SQLiteDataReader Select_Roms4Move()
        {

            Dictionary<string, short> dicCol;
            string sql = $"SELECT [{tRom}].ID, [{tRom}].Archive_Name, [{tRom}].Game_Id" +
                            $", [{tGame}].Game_Name" +
                            $", [{tMachine}].ID AS \"Machine_Id\", [{tMachine}].Nom AS \"Machine_Name\"" +
                            $" FROM [{tRom}] " +
                            $" LEFT JOIN [{tGame}] ON Game_Id = [{tGame}].ID " +
                            $" LEFT JOIN [{tMachine}] ON Machine_Id = [{tMachine}].ID " +
                           "";

            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);

            SqlCond[] conds = new SqlCond[] { new SqlCond($"Game_Id", eWhere.Is_Not, null) };
            Condition_TreatMt(sqlCMD, conds);

            SqlOrder[] orders = new SqlOrder[] { new SqlOrder($"Game_Name") };
            Order_TreatMt(sqlCMD, orders);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }


        public List<CT_Rom_Mapped> List_Roms4Move(SqlCond[] conds = null, SqlOrder order = null)
        {

            List<CT_Rom_Mapped> lGames = new List<CT_Rom_Mapped>();
            SQLiteDataReader reader = Select_Roms4Move();

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    CT_Rom_Mapped aRom = new CT_Rom_Mapped();
                    aRom.ID = Trans.GetUInt("ID", reader);
                    aRom.Archive_Name = Trans.GetString("Archive_Name", reader);
                    aRom.Game_Id = Trans.GetNullableUInt("Game_Id", reader);
                    aRom.Machine_Id = Trans.GetNullableUInt("Machine_Id", reader);


                    // Jeu                   
                    if (aRom.Game_Id != null)
                    {
                        CT_Game_Mapped game = new CT_Game_Mapped();
                        game.ID = uint.Parse(aRom.Game_Id.ToString());
                        game.Game_Name = Trans.GetString("Game_Name", reader);
                        //game.Machine_Id = Trans.GetNullableUInt("Machine_Id", reader);
                        aRom.Game = game;
                    }


                    // Machine
                    if (aRom.Machine_Id != null)
                    {
                        CT_Machine machine = new CT_Machine();
                        machine.ID = uint.Parse(aRom.Machine_Id.ToString());
                        machine.Nom = Trans.GetString("Machine_Name", reader);
                        aRom.Machine = machine;
                    }

                    lGames.Add(aRom);
                }
            }

            return lGames;
        }
        #endregion MappedRoms



        #region Obsolete ?


        /// <summary>
        /// Request to have a list of roms with jointure on games.
        /// </summary>
        /// <param name="conds"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        //[Deprecated]
        internal SQLiteDataReader AffGames_SQL(SqlCond[] conds, SqlOrder order)
        {

            //string constructeurs = PProp.Default.T_Constructeurs;


            Dictionary<string, short> dicCol;
            //string sql = $"SELECT [{tRoms}]*, [{tMachine}].Nom AS Aff_Machine, [{tGenre}].Nom AS Aff_Genre " +
            string sql = $"SELECT [{tRom}].*,[{tGame}].Game_Name,  [{tManufacturer}].Nom AS Aff_Machine" +
                            $" FROM [{tRom}]" +
                            $" LEFT JOIN [{tManufacturer}] ON [{tRom}].Manufacturer = [{tManufacturer}].ID" +
                            $" LEFT  JOIN [{tGame}] ON [{tGame}].ID = [{tRom}].Game_Id";
            /*$"LEFT JOIN [{tMachine}] ON Machine = [{tMachine}].ID " +
            $"LEFT JOIN [{tGenre}] ON Genre = [{tGenre}].ID " +*/



            SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
            Condition_TreatMt(sqlCMD, conds);
            Order_TreatMt(sqlCMD, order);

            Trace.WriteLine($"Requete SQL: {sqlCMD.CommandText}");

            try
            {
                return sqlCMD.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// </remarks>
        internal List<Map_RomGame> Build4Game_List()
        {
            List<Map_RomGame> lGames = new List<Map_RomGame>();
            SQLiteDataReader reader = AffGames_SQL(null, null);

            if (reader.HasRows)
            {
                //dicCol = Get_Poss(reader);

                while (reader.Read())
                {
                    Map_RomGame mr = new Map_RomGame()
                    {


                    };
                    mr.ID = Trans.GetUInt("ID", reader);
                    //mr.Archive_Name = Trans.GetString("Archive_Name", reader);
                    mr.Game_Name = Trans.GetString("Game_Name", reader);


                    //Ag.Game_Name = Trans.GetString("Game_Name", reader);
                    lGames.Add(mr);
                }
            }

            return lGames;
        }

        /* collection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Converter"></param>
        /// <param name="objSelect"></param>
        /// <returns></returns>
        public void Collect_Vrac(Func<Dictionary<string, object>, CT_Game> Converter, Obj_Select objSelect, ObservableCollection<CT_Game> collVrac)
        {
            objSelect.Table = PProp.Default.T_Vrac;
            GetCollectionOf<CT_Game>(Converter, objSelect, collVrac);           
        }
        */

        /* Collec_Versions
public ObservableCollection<T> Collec_Versions<T>(uint gamePos, ObservableCollection<T> obsCollec = null) where T : ICT_SupportLabel
{
    Debug.WriteLine($"Collec_Version");

    if (obsCollec == null)
    {
        obsCollec = new ObservableCollection<T>();
        Debug.WriteLine("GetCollectionOf: Création d'une nouvelle collection");
    }
    else
    {
        obsCollec.Clear();
        Debug.WriteLine("GetCollectionOf: Raz de la collection");
    }

    // Version dématérialisée
    using (SQLiteCommand command = new SQLiteCommand(this.SQLiteConn))
    {
        command.CommandText = $"SELECT [{ PProp.Default.T_Supports_Demat}].ID, [{PProp.Default.T_Companies_Demat}].Nom FROM [{PProp.Default.T_Supports_Demat}] INNER JOIN [{PProp.Default.T_Companies_Demat}] ON [{PProp.Default.T_Supports_Demat}].Company = [{PProp.Default.T_Companies_Demat}].ID WHERE [{PProp.Default.T_Supports_Demat}].GamePos = '{gamePos}'";
        Debug.WriteLine($"Lancement de la requête {command.CommandText}");
        SQLiteDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                T demat = Activator.CreateInstance<T>();
                demat.Support_Type = SupportType.Demat;
                for (short i = 0; i < reader.FieldCount; i++)
                {
                    Debug.WriteLine(reader.GetName(i));
                    switch (reader.GetName(i))
                    {
                        case "ID":
                            demat.ID = Convert.ToUInt32(reader[i]);
                            break;
                        case "Nom":
                            demat.Value = Convert.ToString(reader[i]);
                            break;
                    }

                }
                obsCollec.Add(demat);
            }
        }
    }
    */

        /* Gifters
/// <summary>
/// Retourne la liste des gifters
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="method"></param>
/// <param name="Forced"></param>
/// <returns></returns>
public List<T> List_Gifters<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_Personne
{
objSelect.Table = PProp.Default.T_Personnes;
List<T> ctGifters = GetListOf<T>(method, objSelect);

return ctGifters;
}*/

        /* Version Physique

           // Version Physique
           using (SQLiteCommand command = new SQLiteCommand(this.SQLiteConn))
           {
               command.CommandText = $"SELECT [{ PProp.Default.T_Supports_Phy}].ID, [{PProp.Default.T_Supports_Type}].Support FROM [{PProp.Default.T_Supports_Phy}] INNER JOIN [{PProp.Default.T_Supports_Type}] ON [{PProp.Default.T_Supports_Phy}].Support_Type = [{PProp.Default.T_Supports_Type}].ID WHERE [{PProp.Default.T_Supports_Phy}].GamePos = '{gamePos}'";
               Debug.WriteLine($"Lancement de la requête {command.CommandText}");
               SQLiteDataReader reader = command.ExecuteReader();

               if (reader.HasRows)
               {
                   while (reader.Read())
                   {
                       T demat = Activator.CreateInstance<T>();
                       demat.Support_Type = SupportType.Physique;
                       for (short i = 0; i < reader.FieldCount; i++)
                       {
                           Debug.WriteLine(reader.GetName(i));
                           switch (reader.GetName(i))
                           {
                               case "ID":
                                   demat.ID = Convert.ToUInt32(reader[i]);
                                   break;
                               case "Support":
                                   demat.Value = Convert.ToString(reader[i]);
                                   break;
                           }

                       }
                       obsCollec.Add(demat);
                   }
               }
           }
           */

        /* Version HDD ??
           // Version HDD

           using (SQLiteCommand command = new SQLiteCommand(this.SQLiteConn))
           {
               command.CommandText = $"SELECT [ID], [Chemin] FROM [{PProp.Default.T_Supports_HDD}] WHERE [{PProp.Default.T_Supports_HDD}].GamePos = '{gamePos}'";
               Debug.WriteLine($"Lancement de la requête {command.CommandText}");
               SQLiteDataReader reader = command.ExecuteReader();

               if (reader.HasRows)
               {
                   while (reader.Read())
                   {
                       T demat = Activator.CreateInstance<T>();
                       demat.Support_Type = SupportType.HDD;
                       for (short i = 0; i < reader.FieldCount; i++)
                       {
                           Debug.WriteLine(reader.GetName(i));
                           switch (reader.GetName(i))
                           {
                               case "ID":
                                   demat.ID = Convert.ToUInt32(reader[i]);
                                   break;
                               case "Chemin":
                                   demat.Value = Convert.ToString(reader[i]);
                                   break;
                           }

                       }
                       obsCollec.Add(demat);
                   }
               }
           }

           return obsCollec;
       }
       */


        /* minJeux
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ObservableCollection<T> Collect_MinJeux<T>(Func<Dictionary<string, object>, T> convertToMinGame, Obj_Select objSelect, ObservableCollection<T> collecGames = null) where T : ICT_MinJeu
    {
        objSelect.Table = PProp.Default.T_GamesPos;

        Debug.WriteLine($"GetCollectionOf: {typeof(T)}");

        if (collecGames == null)
        {
            collecGames = new ObservableCollection<T>();
            Debug.WriteLine("GetCollectionOf: Création d'une nouvelle collection");
        }
        else
        {
            Debug.WriteLine("GetCollectionOf: RAZ de la collection");
            collecGames.Clear();
        }

        // Création de la requête
        try
        {
            SQLiteCommand sqlCommand = new SQLiteCommand(SQLiteConn);
            sqlCommand.CommandText = $"SELECT [{PProp.Default.T_GamesPos}].[ID], [{PProp.Default.T_Titres}].[Titre], [{PProp.Default.T_Plateformes}].[Plateforme] " +
                $"FROM [{PProp.Default.T_GamesPos}]" +
                $"INNER JOIN [{PProp.Default.T_Titres}] ON [{PProp.Default.T_GamesPos}].[GameFi] = [{PProp.Default.T_Titres}].[ID] " +
                $"INNER JOIN [{PProp.Default.T_Plateformes}] ON [{PProp.Default.T_GamesPos}].[Plateforme] = [{PProp.Default.T_Plateformes}].[ID]";

            Condition_TreatMt(sqlCommand, objSelect.Conditions);

            Debug.WriteLine($"Lancement de la commande {sqlCommand.CommandText}");

            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Dictionary<string, object> dico = new Dictionary<string, object>();
                    for (short i = 0; i < reader.FieldCount; i++)
                    {
                        dico.Add(reader.GetName(i), reader[i]);
                    }

                    T data = convertToMinGame(dico);
                    if (data == null) continue;
                    collecGames.Add(data);
                }
            }
        }
        catch (SQLiteException sqExc)
        {
            Debug.WriteLine($"Erreur SQLite: {sqExc}");
        }

        return collecGames;
    }
    */

        /* Liste des pegis
/// <summary>
/// Liste des pegis
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="method"></param>
/// <returns></returns>
public List<T> List_PEGIs<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_PEGI
{
objSelect.Table = PProp.Default.T_PEGIs;
List<T> ctPEGI = GetListOf<T>(method, objSelect);

return ctPEGI;
}
*/

        /* R2gions
  /// <summary>
  /// Renvoie une collection de regions
  /// </summary>
  /// <param name="ConvertToRegion"></param>
  /// <param name="All"></param>
  /// <param name="colonnes"></param>
  /// <param name="conditions"></param>
  /// <param name="groups"></param>
  /// <returns></returns>
  public ObservableCollection<T> Collect_Regions<T>(Func<Dictionary<string, object>, T> ConvertToRegion, Obj_Select objSelect, ObservableCollection<T> obsCollec = null) where T : ICT_Region
  {
      objSelect.Table = PProp.Default.T_Regions;
      ObservableCollection<T> collRegions = GetCollectionOf<T>(ConvertToRegion, objSelect, obsCollec);
      return collRegions; }



  */

        /* Sagas

/// <summary>
/// Liste des sagas
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="method"></param>
/// <returns></returns>
public List<T> List_Sagas<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_Saga
{
  objSelect.Table = PProp.Default.T_Sagas;
  List<T> ctSagas = GetListOf<T>(method, objSelect);

  return ctSagas;
}*/

        /* Liste des Plateformes
/// <summary>
/// Liste des Plateformes
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="method"></param>
/// <returns></returns>
public List<T> List_RawPlateformes<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_Plateforme
{
objSelect.Table = PProp.Default.T_Plateformes;
List<T> cT_Plateformes = GetListOf<T>(method, objSelect);

return cT_Plateformes;
}*/

        /* liste des Plateformes exploitable
      /// <summary>
      /// Retourne la liste des Plateformes exploitable
      /// </summary>
      /// <returns></returns>
      public List<Aff_Plateforme> List_FormPlateformes()
      {
          List<Aff_Plateforme> lPlateformes = new List<Aff_Plateforme>();
          string Plateformes = PProp.Default.T_Plateformes;
          string constructeurs = PProp.Default.T_Constructeurs;
          Dictionary<string, short> dicCol;

          string sql = $"SELECT [{Plateformes}].[ID], [IDPlateforme], [{Plateformes}].[Generation], [Plateforme], [{constructeurs}].[Nom] AS [Constructeur] FROM[{Plateformes}] " +
                              $"INNER JOIN[{constructeurs}] ON [{Plateformes}].[Constructeur] = [{constructeurs}].[ID] " +
                              $"ORDER BY [Constructeur], [Generation]";

          SQLiteCommand sqlCMD = new SQLiteCommand(sql, SQLiteConn);
          SQLiteDataReader reader = sqlCMD.ExecuteReader();

          if (reader.HasRows)
          {
              //dicCol = Get_Poss(reader);

              while (reader.Read())
              {
                  Aff_Plateforme affM = new Aff_Plateforme();
                  affM.ID = Convert.ToInt32(reader["ID"]);
                  affM.IDPlateforme = GetNullableInteger(reader["IDPlateforme"]);
                  affM.Generation = GetNullableInteger(reader["Generation"]);
                  affM.Constructeur = reader["Constructeur"].ToString();
                  affM.Plateforme = reader["Plateforme"].ToString();

                  lPlateformes.Add(affM);
              }
          }



          return lPlateformes;
      }
      */


        /* Personnes
public ObservableCollection<T> Collect_Personnes<T>(Func<Dictionary<string, object>, T> ConvertToGifter, Obj_Select objSelect, ObservableCollection<T> obsCollec = null) where T : ICT_Personne
{
objSelect.Table = PProp.Default.T_Personnes;
return GetCollectionOf<T>(ConvertToGifter, objSelect, obsCollec);
}*/


        /* Collection de problèmes ??
    /// <summary>
    /// Collection de... problèmes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ConvertToProblem"></param>
    /// <param name="objSelect"></param>
    /// <param name="obsCollec"></param>
    /// <returns></returns>
    public ObservableCollection<T> Collect_Problems<T>(Func<Dictionary<string, object>, T> ConvertToProblem, Obj_Select objSelect, ObservableCollection<T> obsCollec = null) where T : ICT_Problem
    {
        objSelect.Table = PProp.Default.T_ProblemType;
        return GetCollectionOf<T>(ConvertToProblem, objSelect, obsCollec);
    }
    */


        /* Liste des problèmes
 /// <summary>
 /// Liste des problèmes
 /// </summary>
 /// <typeparam name="T"></typeparam>
 /// <param name="method"></param>
 /// <returns></returns>
 public List<T> List_Problems<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_Problem
 {
     objSelect.Table = PProp.Default.T_ProblemType;
     List<T> ctProblems = GetListOf<T>(method, objSelect);

     return ctProblems;
 }*/

        /* Liste des langues
/// <summary>
/// Liste des langues
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="method"></param>
/// <returns></returns>
public List<T> List_Langues<T>(Func<Dictionary<string, object>, T> method, Obj_Select objSelect) where T : ICT_Langue
{
   objSelect.Table = PProp.Default.T_Langues;
   List<T> ctLangues = GetListOf<T>(method, objSelect);

   return ctLangues;
}*/
        #endregion
    }
}
