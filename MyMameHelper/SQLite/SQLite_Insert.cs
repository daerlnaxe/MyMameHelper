
using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PProp = MyMameHelper.Properties.Settings;
/*
    Version objet de la connexion sql
     
     */


namespace MyMameHelper.SQLite
{


    public sealed partial class SQLite_OP
    {


        #region générique
        /// <summary>
        /// Ajoute une donnée générique à la base (basé sur un couple classique donnée valeur)
        /// </summary>
        /// <param name="cGen"></param>
        /// <param name="table"></param>
        /// <returns>ID de l'insertion</returns>
        public int Insert_Gen(CT_Gen cGen, string table, string colonne)
        {
            Debug.WriteLine($"Insertion de la donnée Générique: {cGen.Valeur}");

            string sql = $"INSERT INTO [{table}] ([{colonne}]) VALUES (@Valeur)";

            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);

            sqlCmd.Parameters.Add("@Valeur", DbType.String).Value = cGen.Valeur;

            ExecNQ(sqlCmd);

            sqlCmd.CommandText = "SELECT last_insert_rowid()";

            var id = sqlCmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }


        /// <summary>
        /// Insert générique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="colonne"></param>
        /// <param name="value"></param>
        public void Insert_Gen(string table, Dictionary<string, string> colVals)
        {

            //Debug.WriteLine($"Insertion de: {value} dans la colonne {colonne} de {table}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            string colonnes = "";
            string values = "";

            foreach (KeyValuePair<string, string> kvp in colVals)
            {
                colonnes += kvp.Key;
                values += $"@{kvp.Key}";

                sqlCmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);

                if (kvp.Key == colVals.Keys.Last())
                {
                    break;
                }
                colonnes += ", ";
                values += ", ";
            }

            string sql = $"INSERT INTO [{table}] ([{colonnes}])VALUES({values})";
            sqlCmd.CommandText = sql;

            //sqlCmd.Parameters.AddWithValue("@value", value);

            ExecNQ(sqlCmd);

        }

        #endregion


        #region insertion unique

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="ctC"></param>
        /// <returns></returns>
        public bool Insert_Constructor(CT_MameManufacturer ctC)
        {
            Debug.WriteLine($"Insertion de la companie: {ctC.Nom}");

            string sql = $"INSERT INTO [{tConstructor}] ([Nom]) VALUES (@Nom)";
            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctC.Nom;

            return ExecNQ(sqlCmd);
        }


        /// <summary>
        /// Ajoute un MameManufacturer à la base
        /// </summary>        
        public bool Insert_MameManufacturer(CT_MameManufacturer ctC)
        {
            Debug.WriteLine($"Insertion du constructeur: {ctC.Nom}");

            string sql = $"INSERT INTO [{tMameManufacturer}] ([Nom]) VALUES (@Nom)";
            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);

            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctC.Nom;

            return ExecNQ(sqlCmd);
        }

        /* 19/07
        public void Insert_GameInVrac(CT_Game game)
        {
            //Debug.WriteLine($"Insertion du jeu: {game.Game_Name}");

            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_Vrac}] ([Archive_Name], [Game_Name])Values(@Archive_Name, @Game_Name)";
            sqlCmd.Parameters.Add("@Archive_Name", DbType.String).Value = game.Parent_Name;
            sqlCmd.Parameters.Add("@Game_Name", DbType.String).Value = game.Game_Name;

            ExecNQ(sqlCmd);

        }*/

        /// <summary>
        /// Insertion d'un genre
        /// </summary>
        /// <param name="cT_Genre"></param>
        internal void Insert_Genre(CT_Genre cT_Genre)
        {
            Debug.WriteLine($"Insertion du genre: {cT_Genre.Nom}");

            string sql = $"INSERT INTO [{PProp.Default.T_Genres}] ([Nom]) VALUES (@Nom)";
            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = cT_Genre.Nom;

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// Insère une machine dans la BDD
        /// </summary>
        /// <param name="ctM"></param>
        public void Insert_Machine(CT_Machine ctM, bool ignore, bool preservePK)
        {
            Debug.WriteLine($"Insertion de la machine: {ctM.Nom}");

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";


            string sql = $"INSERT {sqlIgnore} INTO [{tMachine}] ";
            if (preservePK)
            {
                sql += $"([ID], [Nom], [Constructeur_ID], [Year], [AllowCPath]) " +
                        $"VALUES " +
                        $"(@ID, @Nom, @Constructor_ID, @Year, @AllowCPath)";
            }
            else
            {
                sql += $"([Nom], [Constructeur_ID], [Year], [AllowCPath]) " +
                        $"VALUES " +
                        $"(@Nom, @Constructor_ID, @Year, @AllowCPath)";
            }



            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);

            if (preservePK)
                sqlCmd.Parameters.Add("@ID", DbType.UInt64).Value = ctM.ID;

            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctM.Nom;
            sqlCmd.Parameters.Add("@Constructor_ID", DbType.UInt32).Value = ctM.IDConstructeur;
            sqlCmd.Parameters.Add("@Year", DbType.UInt32).Value = ctM.Year;
            sqlCmd.Parameters.Add("@AllowCPath", DbType.Boolean).Value = ctM.AllowCPath;

            Trace.WriteLine($"Inser: {sqlCmd.CommandText}");

            ExecNQ(sqlCmd);
        }


        /// <summary>
        /// 13/11/2025
        /// </summary>
        /// <param name="ctG"></param>
        public void Insert_Game(CT_Game ctG)
        {
            Debug.WriteLine($"Insertion du jeu: {ctG.Game_Name}");

            string sql = $"INSERT INTO [{tGame}] " +
                            $"([Game_Name])" +
                            $"VALUES " +
                            $"(@Nom)";

            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctG.Game_Name;


            ExecNQ(sqlCmd);
        }
        #endregion




        #region insertion de collection

        /// <summary>
        /// Insère une collection de jeux
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Games"></param>
        public void Insert_CollecInGames<T>(IList<T> Games) where T : iCT_Games
        {
            uint max = 100;
            Debug.WriteLine($"Insertion de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Games.Count; i++)
            {
                T game = Games[i];

                sqlCmd.CommandText = $"Insert INTO [{tGame}] " +
                                        $"([Game_Name], [Description], [Unwanted], [Machine_Id], [Genre_Id], [IsMahjong], [IsQuizz], [Rate]) " + // J'ai levé Roms, ça n'en fait plus partie
                                        $"VALUES ";

                for (int j = 0; j < max; j++)
                {
                    if (i == Games.Count)
                        break;
                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(@Game_Name{j}, @Description{j}, @Unwanted{j},@Machine_Id{j}, @Genre_Id{j}, @IsMahjong{j}, @IsQuizz{j}, @Rate{j})";// J'ai levé Roms, ça n'en fait plus partie
                    // parametres
                    sqlCmd.Parameters.Add($"@Game_Name{j}", DbType.String).Value = Games[i].Game_Name;
                    sqlCmd.Parameters.Add($"@Description{j}", DbType.String).Value = Games[i].Description;
                    sqlCmd.Parameters.Add($"@Unwanted{j}", DbType.Boolean).Value = Games[i].Unwanted;
                    sqlCmd.Parameters.Add($"@Machine_Id{j}", DbType.Int32).Value = Games[i].Machine_Id;
                    sqlCmd.Parameters.Add($"@Genre_Id{j}", DbType.Int32).Value = Games[i].Genre_Id;
                    sqlCmd.Parameters.Add($"@Rate{j}", DbType.Int32).Value = Games[i].Rate;
                    sqlCmd.Parameters.Add($"@IsMahjong{j}", DbType.Boolean).Value = Games[i].IsMahjong;
                    sqlCmd.Parameters.Add($"@IsQuizz{j}", DbType.Boolean).Value = Games[i].IsQuizz;
                    // roms
                    /* Rom est levé
                    string romsString = string.Empty;
                    foreach (CT_Rom rom in Games[i].Roms)
                    {
                        if (rom != Games[i].Roms[0])
                            romsString += "|";

                        romsString += rom.ID;
                    }

                    sqlCmd.Parameters.Add($"@Roms{j}", DbType.String).Value = romsString;*/

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Games.Count);
            }

        }


        /// <summary>
        /// Insère une collection de constructeurs
        /// </summary>
        /// <param name="constructors"></param>
        /// <param name="ignore"></param>
        /// <param name="preservePK"></param>
        public void Insert_Constructors(IList<CT_Constructor> constructors, bool ignore, bool preservePK)
        {
            uint max = 10;
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";



            for (int i = 0; i < constructors.Count; i++)
            {
                CT_Constructor dev = constructors[i];
                //  string vals = null;

                sqlCmd.CommandText = $"Insert {sqlIgnore} INTO [{tConstructor}]";
                // Add Key if asked
                if (preservePK)
                {
                    sqlCmd.CommandText += $"([ID],[Nom])" +
                        " VALUES ";
                }
                else
                {
                    sqlCmd.CommandText += $"([Nom])" +
                        " VALUES";
                }
                sqlCmd.CommandText += "(";

                // Cette boucle permet de pusher par lots.
                for (int j = 0; j < max; j++)
                {
                    if (i == constructors.Count)
                        break;

                    if (j != 0)
                        sqlCmd.CommandText += ",(";

                    // Si l'on préserve les PK
                    if (preservePK)
                        sqlCmd.CommandText += $"@ID{j},";

                    sqlCmd.CommandText += $"@Nom{j}";
                    sqlCmd.CommandText += $")";

                    // Si l'on préserve les PK
                    if (preservePK)
                        sqlCmd.Parameters.Add($"@ID{j}", DbType.UInt64).Value = constructors[i].ID;

                    sqlCmd.Parameters.Add($"@Nom{j}", DbType.String).Value = constructors[i].Nom;

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                Trace.WriteLine($"Exec: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / constructors.Count);
                Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");


        }


        /// <summary>
        /// Insère une collection de genres
        /// </summary>
        /// <param name="genres"></param>
        /// <param name="ignore"></param>
        /// <param name="preservePK"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void Insert_Genres(IList<CT_Genre> genres, bool ignore, bool preservePK)
        {
            uint max = 10;
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";


            sqlCmd.CommandText = $"Insert {sqlIgnore} INTO [{tGenre}]";

            // Add Key if asked
            if (preservePK)
            {
                sqlCmd.CommandText += $" ([ID], [Nom])" +
                    " VALUES";
            }
            else
            {
                sqlCmd.CommandText = " ([Nom])" +
                    " VALUES ";
            }


            for (int i = 0; i < genres.Count; i++)
            {
                CT_Genre dev = genres[i];
                //  string vals = null;


                for (int j = 0; j < max; j++)
                {
                    if (i == genres.Count)
                        break;

                    if (j == 0)
                        sqlCmd.CommandText += " (";
                    else
                        sqlCmd.CommandText += ", (";

                    // Si l'on préserve les PK
                    if (preservePK)
                        sqlCmd.CommandText += $"@ID{j}, ";

                    sqlCmd.CommandText += $"@Nom{j} ";
                    sqlCmd.CommandText += $")";

                    // Si l'on préserve les PK
                    if (preservePK)
                        sqlCmd.Parameters.Add($"@ID{j}", DbType.UInt64).Value = genres[i].ID;

                    sqlCmd.Parameters.Add($"@Nom{j}", DbType.String).Value = genres[i].Nom;

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / genres.Count);
                Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");


        }


        /*1907
    public void Insert_CollecInVrac<T>(ObservableCollection<T> Games) where T : iCT_Game
    {
        Debug.WriteLine($"Insertion de la collection");
        SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        for (int i = 0; i < Games.Count; i++)
        {
            T game = Games[i];
            //  string vals = null;
            sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_Vrac}] ([Archive_Name], [Game_Name]) VALUES ";

            for (int j = 0; j < 100; j++)
            {
                if (i == Games.Count)
                    break;
                if (j != 0)
                    sqlCmd.CommandText += ", ";


                sqlCmd.CommandText += $"(@Archive_Name{j}, @Game_Name{j})";
                sqlCmd.Parameters.Add($"@Archive_Name{j}", DbType.String).Value = Games[i].Parent_Name;
                sqlCmd.Parameters.Add($"@Game_Name{j}", DbType.String).Value = Games[i].Game_Name;
                i++;
            }


            ExecNQ(sqlCmd);
            UpdateProgress?.Invoke(this, i * 100 / Games.Count);
            Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
        }
        Debug.WriteLine($"{sw.ElapsedMilliseconds}");

    */
        /*Nope

            sqlCmd.Parameters.Add($"@Archive_Name{i}", DbType.String).Value = Games[i].Archive_Name;
            sqlCmd.Parameters.Add($"@Game_Name{i}", DbType.String).Value = Games[i].Game_Name;

            if (i % 25000 == 0 || i == (Games.Count - 1))
            {
                ExecNQ(sqlCmd);
            }
            /*sqlCmd.CommandText = "[Archive_Name], [Game_Name])Values(@Archive_Name, @Game_Name)";
            sqlCmd.Parameters.Add("@Archive_Name", DbType.String).Value = game.Archive_Name;
            sqlCmd.Parameters.Add("@Game_Name", DbType.String).Value = game.Game_Name;*/


        /* Developers
        /// <summary>
        /// Insère une collection de Developers
        /// </summary>
        /// <param name="developers"></param>
        public void Insert_MameManufacturers(IList<CT_Developer> developers, bool ignore)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de developpeurs");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";

            for (int i = 0; i < developers.Count; i++)
            {
                CT_MameManufacturer dev = developers[i];
                //  string vals = null;




                // 
                sqlCmd.CommandText = $"Insert {sqlIgnore} INTO  [{PProp.Default.T_Constructors}] (" +
                                        "[Nom] " +
                                        ") VALUES ";

                for (int j = 0; j < max; j++)
                {
                    if (i == developers.Count)
                        break;
                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(" +
                                          $"@Nom{j} " +
                                          $")";

                    sqlCmd.Parameters.Add($"@Nom{j}", DbType.String).Value = developers[i].Nom;

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / developers.Count);
                Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");


        }
  */
        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collec"></param>
        /// <param name="ignore"></param>
        /// <param name="preservePK"></param>
        private void Insert_GenericList<T>(IList<T> collec, bool ignore, bool preservePK)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="machines"></param>
        /// <param name="ignore"></param>
        /// <param name="preservePK"></param>
        /// <exception cref="Exception"></exception>
        /// <remarks>
        /// Puissante et générique
        /// </remarks>
        internal void Insert_Machines(IList<CT_Machine> machines, bool ignore, bool preservePK)
        {
            Debug.WriteLine($"Insertion de la collection de machines");

            uint max = 20;
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";



            for (int i = 0; i < machines.Count; i++)
            {
                CT_Machine dev = machines[i];
                //  string vals = null;

                sqlCmd.CommandText = $"INSERT {sqlIgnore} INTO [{tMachine}]";

                // Add Key if asked
                //sqlCmd.CommandText = preservePK == true ? $" ([ID], [Nom]" : " ([Nom]";

                List<string> fields = new List<string>()
                {
                    "ID",
                    "Nom",
                    "Description",
                    "Category"
                };


                // On lèvre ID si on ne veut pas préserver les pk
                if (!preservePK)
                    fields.Remove("ID");


                // On ajoute les champs
                for (int k = 0; k < fields.Count; k++)
                {
                    string field = fields[k];


                    if (k == 0)
                        sqlCmd.CommandText += "(";

                    // Ajout de la virgule entre les champs
                    if (k != 0)
                        sqlCmd.CommandText += ",";

                    sqlCmd.CommandText += $"[{field}]";

                }

                sqlCmd.CommandText += ") Values (";

                // limiteur
                for (int j = 0; j < max; j++)
                {
                    if (i == machines.Count)
                        break;

                    if (j != 0)
                        sqlCmd.CommandText += ",(";

 
                    // ligne
                    for (int k = 0; k < fields.Count; k++)
                    {
                        string field = fields[k];

                        // Récupération de l'accesseur en fonction du champ
                        PropertyInfo prop = machines[i].GetType().GetProperty(field);
                        if (prop != null)
                        {
                            object valeur = prop.GetValue(machines[i]);

                            // Ajout de la virgule entre les champs
                            if (k != 0)
                                sqlCmd.CommandText += ",";

                            // Ajout du champ à remplir
                            sqlCmd.CommandText += $"@{field}{j}";

                            // Ajout de la valeur au champ à remplir
                            if (valeur == null)
                                sqlCmd.Parameters.Add($"@{field}{j}", DbType.String).Value =null;
                            else if (valeur.GetType() == typeof(string))
                                sqlCmd.Parameters.Add($"@{field}{j}", DbType.String).Value = valeur;
                            else if (valeur.GetType() == typeof(uint))
                                sqlCmd.Parameters.Add($"@{field}{j}", DbType.UInt64).Value = valeur;
                        }
                        else
                        {
                            throw new Exception($"Unknown accessor: {field}");
                        }

                        //if (typeof(machines[i].))

                    }

                    //sqlCmd.CommandText += $"@Nom{j}";
                    // On termine la ligne
                    sqlCmd.CommandText += ")";

                    // Si l'on préserve les PK
                    /*if (preservePK)
                        sqlCmd.Parameters.Add($"@ID{j}", DbType.UInt64).Value = machines[i].ID;

                    sqlCmd.Parameters.Add($"@Nom{j}", DbType.String).Value = machines[i].Nom;*/

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                Trace.WriteLine($"Exec: {sqlCmd.CommandText}");

                foreach (SQLiteParameter parameter in sqlCmd.Parameters)
                {
                    Debug.WriteLine($"{parameter.ParameterName} | {parameter.Value}");

                }

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / machines.Count);
                Debug.WriteLine($"{i}/{machines.Count} ({sw.ElapsedMilliseconds})");
            }
            Trace.WriteLine($"{sw.ElapsedMilliseconds}");

        }


        /// <summary>
        /// Insère une collection de manufacturers
        /// </summary>
        /// <param name=""></param>
        public void Insert_Manus(IList<CT_MameManufacturer> manufacturers, bool ignore)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de manufactureurs");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add ignore if asked
            string sqlIgnore = "";
            if (ignore)
                sqlIgnore = "OR IGNORE";


            sqlCmd.CommandText = $"Insert {sqlIgnore} INTO [{tMameManufacturer}] (" +
                          "[Nom] " +
                          ") VALUES ";

            for (int i = 0; i < manufacturers.Count; i++)
            {
                CT_MameManufacturer dev = manufacturers[i];
                //  string vals = null;


                for (int j = 0; j < max; j++)
                {
                    if (i == manufacturers.Count)
                        break;
                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(" +
                                          $"@Nom{j} " +
                                          $")";

                    sqlCmd.Parameters.Add($"@Nom{j}", DbType.String).Value = manufacturers[i].Nom;

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);



                UpdateProgress?.Invoke(this, i * 100 / manufacturers.Count);
                Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");


        }


        /// <summary>
        /// Insère une collection de roms dans la table temp
        /// </summary>
        /// <param name="Roms"></param>
        public void Insert_RawRomsInTemp(IList<RawMameRom> Roms)
        {
            uint max = 75;
            long elapsed = 0;
            Debug.WriteLine($"Insertion de la collection de roms brutes");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);


            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < Roms.Count; i++)
            {
                if (Stopit)
                    break;

                RawMameRom rom = Roms[i];
                //  string vals = null;
                sqlCmd.CommandText = $"Insert INTO [{tTempRom}] (" +
                                        "[Name], " +
                                        "[Source_File], " +
                                        "[Rom_Of], " +
                                        "[Clone_Of], " +
                                        "[Sample_Of], " +
                                        "[Is_Bios], " +
                                        "[Is_Mechanical], " +
                                        "[Description], " +
                                        "[Year], " +
                                        "[Manufacturer], " +
                                        "[HasSoftwares], " +
                                        "[IsDevice] " +
                                        ") VALUES ";

                for (int j = 0; j < max; j++)
                {
                    if (i == Roms.Count)
                        break;
                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(" +
                                          $"@Name{j}, " +
                                          $"@Source_File{j}, " +
                                          $"@Rom_Of{j}, " +
                                          $"@Clone_Of{j}, " +
                                          $"@Sample_Of{j}, " +
                                          $"@Is_Bios{j}, " +
                                          $"@Is_Mechanical{j}, " +
                                          $"@Description{j}, " +
                                          $"@Year{j}, " +
                                          $"@Manufacturer{j}," +
                                          $"@HasSoftwares{j}," +
                                          $"@IsDevice{j}" +
                                          $")";

                    sqlCmd.Parameters.Add($"@Name{j}", DbType.String).Value = Roms[i].Name;
                    sqlCmd.Parameters.Add($"@Source_File{j}", DbType.String).Value = Roms[i].Source_File;
                    sqlCmd.Parameters.Add($"@Rom_Of{j}", DbType.String).Value = Roms[i].Rom_Of;
                    sqlCmd.Parameters.Add($"@Clone_Of{j}", DbType.String).Value = Roms[i].Clone_Of;
                    sqlCmd.Parameters.Add($"@Sample_Of{j}", DbType.String).Value = Roms[i].Sample_Of;
                    sqlCmd.Parameters.Add($"@Is_Bios{j}", DbType.String).Value = Roms[i].Is_Bios;
                    sqlCmd.Parameters.Add($"@Is_Mechanical{j}", DbType.String).Value = Roms[i].Is_Mechanical;
                    sqlCmd.Parameters.Add($"@Description{j}", DbType.String).Value = Roms[i].Description;
                    sqlCmd.Parameters.Add($"@Year{j}", DbType.String).Value = Roms[i].Year;
                    sqlCmd.Parameters.Add($"@Manufacturer{j}", DbType.String).Value = Roms[i].Manufacturer;
                    sqlCmd.Parameters.Add($"@HasSoftwares{j}", DbType.String).Value = Roms[i].HasSoftwares;
                    sqlCmd.Parameters.Add($"@IsDevice{j}", DbType.String).Value = Roms[i].Is_Device;

                    if (j < max - 1)
                        i++;

                    long nowElapsed = sw.ElapsedMilliseconds;
                    if ((nowElapsed - elapsed) > 1000)
                    {
                        UpdateProgress?.Invoke(this, i * 100 / Roms.Count);
                        Debug.WriteLine($"{i}/{Roms.Count} ({nowElapsed} ms)");
                        elapsed = nowElapsed;
                    }
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);


            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Roms"></param>
        public void Insert_Roms<T>(IList<T> Roms, bool ignore) where T : iCT_Rom
        {
            ushort max = 50;

            Debug.WriteLine($"Insertion de la collection de roms brutes");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            string strIgnore = "";
            if (ignore)
                strIgnore = "OR IGNORE";

            for (int i = 0; i < Roms.Count; i++)
            {
                T rom = Roms[i];
                //  string vals = null;
                sqlCmd.CommandText = $"Insert {strIgnore} INTO [{tRom}] (" +
                                        "[Archive_Name], " +
                                        "[Description], " +
                                        "[Source_File], " +
                                        "[Game_Id]," +
                                        "[Unwanted]," +
                                        "[Year], " +
                                        "[Manufacturer_Id], " +
                                        "[Machine_Id], " +
                                        "[IsParent], " +
                                        "[Clone_Of]," +
                                        "[IsPinball]" +
                                        ") VALUES ";

                for (int j = 0; j < max; j++)
                {

                    if (i == Roms.Count)
                        break;

                    /*  if (j >= Roms.Count)
                          break;*/

                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(" +
                                          $"@Archive_Name{j}, " +
                                          $"@Description{j}, " +
                                          $"@Source_File{j}, " +
                                          $"@Game_Id{j}, " +
                                          $"@Unwanted{j}, " +
                                          $"@Year{j}, " +
                                          $"@Manufacturer_Id{j}, " +
                                          $"@Machine_Id{j}, " +
                                          $"@IsParent{j}, " +
                                          $"@Clone_Of{j}, " +
                                          $"@IsPinball{j} " +
                                          $")";

                    sqlCmd.Parameters.Add($"@Archive_Name{j}", DbType.String).Value = Roms[i].Archive_Name;
                    sqlCmd.Parameters.Add($"@Description{j}", DbType.String).Value = Roms[i].Description;
                    sqlCmd.Parameters.Add($"@Source_File{j}", DbType.String).Value = Roms[i].SourceFile;
                    // Game
                    sqlCmd.Parameters.Add($"@Game_Id{j}", DbType.UInt32).Value = Roms[i].Game_Id;

                    sqlCmd.Parameters.Add($"@Year{j}", DbType.String).Value = Roms[i].Year;
                    sqlCmd.Parameters.Add($"@Unwanted{j}", DbType.Boolean).Value = Roms[i].Unwanted;
                    //sqlCmd.Parameters.Add($"@Manufacturer{j}", DbType.String).Value = Roms[i].Manufacturer;
                    sqlCmd.Parameters.Add($"@Manufacturer_Id{j}", DbType.UInt32).Value = Roms[i].Manufacturer.ID;
                    sqlCmd.Parameters.Add($"@Machine_Id{j}", DbType.UInt32).Value = Roms[i].Machine_Id;
                    sqlCmd.Parameters.Add($"@IsParent{j}", DbType.Boolean).Value = Roms[i].IsParent;
                    sqlCmd.Parameters.Add($"@Clone_Of{j}", DbType.UInt32).Value = Roms[i].Clone_Of;
                    sqlCmd.Parameters.Add($"@IsPinball{j}", DbType.Boolean).Value = Roms[i].IsPinball;
                    Trace.WriteLine(Roms[i].Archive_Name);

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Roms.Count);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="biosCollec"></param>
        internal void Insert_BiosInTemp(IList<CT_Bios> biosCollec)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de Bios");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);
        }
        #endregion





        /*
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gamePos"></param>
    public void Insert_GamePos<T>(T gamePos) where T : ICT_GamePos
    {
        Debug.WriteLine($"Insertion en Bibliothèque du jeu: {gamePos.GameFi}");

        string sql = $"INSERT INTO '{Settings.Default.T_GamesPos}'" +
            $" ('GameFi', 'Plateforme', 'Best_Version', 'En_Cours', 'Fini')" +
            $" VALUES" +
            $" ( @GameFi, @Plateforme, @Best_Version, @En_Cours, @Fini)";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);

        sqlCmd.Parameters.Add("@GameFi", DbType.UInt32).Value = gamePos.GameFi;
        sqlCmd.Parameters.Add("@Plateforme", DbType.UInt16).Value = gamePos.Plateforme;
        sqlCmd.Parameters.Add("@Best_Version",DbType.Boolean).Value = gamePos.Best_Version;
        sqlCmd.Parameters.Add("@En_Cours",DbType.Boolean).Value = gamePos.En_Cours;
        sqlCmd.Parameters.Add("@Fini", DbType.Boolean).Value = gamePos.Fini;

        ExecNQ(sqlCmd);

    }
    */


        /*
    /// <summary>
    /// Ajoute une donnée Problème à la base
    /// </summary>
    /// <param name="ctGenre"></param>
    public void Insert_DataProblem(ICT_Problem ctProblem)
    {
        Debug.WriteLine($"Insertion de la donnée Problème: {ctProblem.Problem}");

        string sql = $"INSERT INTO [{Settings.Default.T_ProblemType}] ([Problem]) VALUES (@Problem)";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
        sqlCmd.Parameters.Add("@Problem", DbType.String).Value = ctProblem.Problem;

        ExecNQ(sqlCmd);
    }*/


        /*

    /// <summary>
    /// Ajoute une donnée Release à la base
    /// </summary>
    /// <param name="ct_Release"></param>
    public void Insert_Release(ICT_Release ct_Release)
    {
        Debug.WriteLine($"Insertion de la Date de Release: {ct_Release.Date.ToString()}");

        string sql = $"INSERT INTO [{Settings.Default.T_Releases}] ([Titre], [Zone], [Date]) VALUES (@Titre, @Zone, @Date) ";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);

        sqlCmd.Parameters.Add("@Titre", DbType.UInt32).Value = ct_Release.Titre;
        sqlCmd.Parameters.Add("@Zone", DbType.UInt16).Value = ct_Release.Zone;
        sqlCmd.Parameters.Add("@Date", DbType.Date).Value = ct_Release.Date;

        ExecNQ(sqlCmd);
    }*/

        /*

    /// <summary>
    /// Ajoute une Région à la base
    /// </summary>
    /// <param name=""></param>
    public void Insert_Region(ICT_Region cT_Region)
    {
        Debug.WriteLine($"Insertion de la donnée Problème: {cT_Region.Region}");

        string sql = $"INSERT INTO [{Settings.Default.T_Regions}] ([Region]) VALUES (@Region)";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
        sqlCmd.Parameters.Add("@Region", DbType.String).Value = cT_Region.Region;

        ExecNQ(sqlCmd);
    }*/


        /*

    /// <summary>
    /// Ajoute une Saga à la base
    /// </summary>
    /// <param name="ctSaga"></param>
    public void Insert_Saga(ICT_Saga ctSaga)
    {
        Debug.WriteLine($"Insertion de la saga: {ctSaga.Saga}");

        string sql = $"INSERT INTO [{Settings.Default.T_Sagas}] ([Saga]) VALUES (@Saga)";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
        sqlCmd.Parameters.Add("@Saga", DbType.String).Value = ctSaga.Saga;

        ExecNQ(sqlCmd);
    }
    */


        /*
    /// <summary>
    /// Ajoute un Support à la base
    /// </summary>
    /// <param name="ctSupport"></param>
    public void Insert_Support(ICT_SupportType ctSupport)
    {
        Debug.WriteLine($"Insertion du support: {ctSupport.Support}");

        string sql = $"INSERT INTO [{Settings.Default.T_Supports_Type}] ([Support]) VALUES (@Support)";

        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
        sqlCmd.Parameters.Add("@Support", DbType.String).Value = ctSupport.Support;

        ExecNQ(sqlCmd);
    }
    */

        /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cT_TitreAlt"></param>
    public void Insert_TitreAlternatif(ICT_TitreAlt cT_TitreAlt)
    {
        Debug.WriteLine($"Insertion du suppoer: {cT_TitreAlt.Titre_Alternatif}");

        string sql = $"INSERT INTO [{Settings.Default.T_TitresAlt}] ([Titre], [TitreAlt]) VALUES (@Titre, @TitreAlt)";
        SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
        sqlCmd.Parameters.Add("@Titre", DbType.UInt32).Value = cT_TitreAlt.Titre;
        sqlCmd.Parameters.Add("@TitreAlt", DbType.String).Value = cT_TitreAlt.Titre_Alternatif;

        ExecNQ(sqlCmd);
    }*/








    }
}
