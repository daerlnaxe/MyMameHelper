
using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PProp = MyMameHelper.Properties.Settings;
/*
    Version objet de la connexion sql
     
     */


namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_Req
    {

        #region insertion unique
        public bool Insert_Companie(CT_Constructeur ctC)
        {
            Debug.WriteLine($"Insertion de la companie: {ctC.Nom}");

            string sql = $"INSERT INTO [{PProp.Default.T_Developers}] ([Nom]) VALUES (@Nom)";
            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctC.Nom;

            return ExecNQ(sqlCmd);
        }

        /// <summary>
        /// Ajoute un constructeur à la base
        /// </summary>
        /// <param name="compagnie"></param>
        public bool Insert_Constructeur(CT_Constructeur ctC)
        {
            Debug.WriteLine($"Insertion du constructeur: {ctC.Nom}");

            string sql = $"INSERT INTO [{PProp.Default.T_Constructeurs}] ([Nom]) VALUES (@Nom)";
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


        internal void Insert_Genre(CT_Genre cT_Genre)
        {
            Debug.WriteLine($"Insertion du genre: {cT_Genre.Nom}");

            string sql = $"INSERT INTO [{PProp.Default.T_Genres}] ([Nom]) VALUES (@Nom)";
            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = cT_Genre.Nom;

            ExecNQ(sqlCmd);
        }

        public void Insert_Machine(CT_Machine ctM)
        {
            Debug.WriteLine($"Insertion de la machine: {ctM.Nom}");

            string sql = $"INSERT INTO [{PProp.Default.T_Machines}] " +
                            $"([Nom], [Constructeur], [Year], [AllowCPath]) " +
                            $"VALUES " +
                            $"(@Nom, @Constructor, @Year, @AllowCPath)";

            SQLiteCommand sqlCmd = new SQLiteCommand(sql, SQLiteConn);
            sqlCmd.Parameters.Add("@Nom", DbType.String).Value = ctM.Nom;
            sqlCmd.Parameters.Add("@Constructor", DbType.UInt32).Value = ctM.IDConstructeur;
            sqlCmd.Parameters.Add("@Year", DbType.UInt32).Value = ctM.Year;
            sqlCmd.Parameters.Add("@AllowCPath", DbType.Boolean).Value = ctM.AllowCPath;

            ExecNQ(sqlCmd);
        }
        #endregion

        #region insertion de collection

        public void Insert_CollecInGames<T>(ObservableCollection<T> Games) where T : iCT_Games
        {
            uint max = 100;
            Debug.WriteLine($"Insertion de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Games.Count; i++)
            {
                T game = Games[i];

                sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_Games}] " +
                                        $"([Game_Name],  [Description], [Roms], [Unwanted],[Machine], [Genre], [IsMahjong], [IsQuizz], [Rate]) " +
                                        $"VALUES ";

                for (int j = 0; j < max; j++)
                {
                    if (i == Games.Count)
                        break;
                    if (j != 0)
                        sqlCmd.CommandText += ", ";

                    sqlCmd.CommandText += $"(@Game_Name{j}, @Description{j}, @Roms{j}, @Unwanted{j},@Machine{j}, @Genre{j}, @IsMahjong{j}, @IsQuizz{j}, @Rate{j})";
                    // parametres
                    sqlCmd.Parameters.Add($"@Game_Name{j}", DbType.String).Value = Games[i].Game_Name;
                    sqlCmd.Parameters.Add($"@Description{j}", DbType.String).Value = Games[i].Description;
                    sqlCmd.Parameters.Add($"@Unwanted{j}", DbType.Boolean).Value = Games[i].Unwanted;
                    sqlCmd.Parameters.Add($"@Machine{j}", DbType.Int32).Value = Games[i].Machine;
                    sqlCmd.Parameters.Add($"@Genre{j}", DbType.Int32).Value = Games[i].Genre;
                    sqlCmd.Parameters.Add($"@Rate{j}", DbType.Int32).Value = Games[i].Rate;
                    sqlCmd.Parameters.Add($"@IsMahjong{j}", DbType.Boolean).Value = Games[i].IsMahjong;
                    sqlCmd.Parameters.Add($"@IsQuizz{j}", DbType.Boolean).Value = Games[i].IsQuizz;
                    // roms
                    string romsString = string.Empty;
                    foreach (CT_Rom rom in Games[i].Roms)
                    {
                        if (rom != Games[i].Roms[0])
                            romsString += "|";

                        romsString += rom.ID;
                    }

                    sqlCmd.Parameters.Add($"@Roms{j}", DbType.String).Value = romsString;

                    // a surveiller si bug
                    if (j < max - 1)
                        i++;
                }

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Games.Count);
            }

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


        //    

        /*
        }
        */
        /// <summary>
        /// Insère une collection de devs
        /// </summary>
        /// <param name="developers"></param>
        public void Insert_Devs(ObservableCollection<CT_Constructeur> developers, bool ignore)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de developpeurs");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < developers.Count; i++)
            {
                CT_Constructeur dev = developers[i];
                //  string vals = null;

                // Add ignore if asked
                string sqlIgnore = "";
                if (ignore)
                    sqlIgnore = "OR IGNORE";


                // 
                sqlCmd.CommandText = $"Insert {sqlIgnore} INTO  [{PProp.Default.T_Developers}] (" +
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


        /// <summary>
        /// Insère une collection de manufacturers
        /// </summary>
        /// <param name="developers"></param>
        public void Insert_Manus(ObservableCollection<CT_Constructeur> developers)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de developpeurs");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < developers.Count; i++)
            {
                CT_Constructeur dev = developers[i];
                //  string vals = null;
                sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_Constructeurs}] (" +
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


        /// <summary>
        /// Insère une collection de roms dans la table temp
        /// </summary>
        /// <param name="Roms"></param>
        public void Insert_RomsInTemp(ObservableCollection<RawMameRom> Roms)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de roms brutes");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);
            

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < Roms.Count; i++)
            {
                RawMameRom rom = Roms[i];
                //  string vals = null;
                sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_TempRoms}] (" +
                                        "[Name], " +
                                        "[Source_File], " +
                                        "[Rom_Of], " +
                                        "[Clone_Of], " +
                                        "[Sample_Of], " +
                                        "[Is_Bios], " +
                                        "[Is_Mechanical], " +
                                        "[Description], " +
                                        "[Year], " +
                                        "[Manufacturer] " +
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
                                          $"@Manufacturer{j}" +
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

                    if (j < max - 1)
                        i++;
                }

                //Trace.WriteLine($"Requete: {sqlCmd.CommandText}");

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Roms.Count);
                Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds}");
            }
            Debug.WriteLine($"{sw.ElapsedMilliseconds}");
        }


        public void Insert_Roms<T>(List<T> Roms) where T : iCT_Rom
        {
            ushort max = 50;

            Debug.WriteLine($"Insertion de la collection de roms brutes");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Roms.Count; i++)
            {
                T rom = Roms[i];
                //  string vals = null;
                sqlCmd.CommandText = $"Insert INTO [{PProp.Default.T_Roms}] (" +
                                        "[Archive_Name], " +
                                        "[Description], " +
                                        "[Unwanted]," +
                                        "[Year], " +
                                        "[Manufacturer], " +
                                        "[IsParent], " +
                                        "[Clone_Of]" +
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
                                          $"@Unwanted{j}, " +
                                          $"@Year{j}, " +
                                          $"@Manufacturer{j}, " +
                                          $"@IsParent{j}, " +
                                          $"@Clone_Of{j} " +
                                          $")";

                    sqlCmd.Parameters.Add($"@Archive_Name{j}", DbType.String).Value = Roms[i].Archive_Name;
                    sqlCmd.Parameters.Add($"@Description{j}", DbType.String).Value = Roms[i].Description;
                    sqlCmd.Parameters.Add($"@Year{j}", DbType.String).Value = Roms[i].Year;
                    sqlCmd.Parameters.Add($"@Unwanted{j}", DbType.Boolean).Value = Roms[i].Unwanted;
                    sqlCmd.Parameters.Add($"@Manufacturer{j}", DbType.String).Value = Roms[i].Manufacturer;
                    sqlCmd.Parameters.Add($"@IsParent{j}", DbType.Boolean).Value = Roms[i].IsParent;
                    sqlCmd.Parameters.Add($"@Clone_Of{j}", DbType.UInt32).Value = Roms[i].Clone_Of;
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

        internal void Insert_BiosInTemp(MyObservableCollection<CT_Bios> biosCollec)
        {
            uint max = 50;
            Debug.WriteLine($"Insertion de la collection de Bios");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);
        }
        #endregion

        #region Update

        public void Update_Games<T>(ObservableCollection<T> Games) where T : iCT_Games
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Games.Count; i++)
            {
                T game = Games[i];
                //  string vals = null;

                sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Games}]" +
                                        $"SET " +
                                        $"[Game_Name]=@Game_Name, " +
                                        $"[Description]=@Description, " +
                                        $"[Machine]=@Machine, " +
                                        $"[Genre]=@Genre, " +
                                        $"[Rate]=@Rate, " +
                                        $"[Unwanted]=@Unwanted, " +
                                        $"[IsMahjong]=@IsMahjong, " +
                                        $"[IsQuizz]=@IsQuizz " +
                                        //$"[Developer]=@Developer " +
                                        $"WHERE ID=@ID";

                sqlCmd.Parameters.Add($"@Game_Name", DbType.String).Value = Games[i].Game_Name;
                sqlCmd.Parameters.Add($"@Description", DbType.String).Value = Games[i].Description;
                sqlCmd.Parameters.Add($"@Machine", DbType.UInt32).Value = Games[i].Machine;
                sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = Games[i].Unwanted;
                sqlCmd.Parameters.Add($"@Genre", DbType.UInt32).Value = Games[i].Genre;
                sqlCmd.Parameters.Add($"@Rate", DbType.UInt32).Value = Games[i].Rate;
                sqlCmd.Parameters.Add($"@IsMahjong", DbType.Boolean).Value = Games[i].IsMahjong;
                sqlCmd.Parameters.Add($"@IsQuizz", DbType.Boolean).Value = Games[i].IsQuizz;
                //sqlCmd.Parameters.Add($"@Developer", DbType.UInt32).Value = Games[i].Developer;

                // condition
                sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = Games[i].ID;

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Games.Count);

            }
        }

        internal void Update_Game(CT_Game gameCont)
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Games}]" +
                                    $"SET " +
                                    $"[Game_Name]=@Game_Name, " +
                                    $"[Description]=@Description, " +
                                    $"[Machine]=@Machine, " +
                                    $"[Genre]=@Genre, " +
                                    $"[Rate]=@Rate, " +
                                    $"[Unwanted]=@Unwanted, " +
                                    $"[IsMahjong]=@IsMahjong, " +
                                    $"[IsQuizz]=@IsQuizz " +
                                    //$"[Developer]=@Developer " +
                                    $"WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Game_Name", DbType.String).Value = gameCont.Game_Name;
            sqlCmd.Parameters.Add($"@Description", DbType.String).Value = gameCont.Description;
            sqlCmd.Parameters.Add($"@Machine", DbType.UInt32).Value = gameCont.Machine;
            sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = gameCont.Unwanted;
            sqlCmd.Parameters.Add($"@Genre", DbType.UInt32).Value = gameCont.Genre;
            sqlCmd.Parameters.Add($"@Rate", DbType.UInt32).Value = gameCont.Rate;
            sqlCmd.Parameters.Add($"@IsMahjong", DbType.Boolean).Value = gameCont.IsMahjong;
            sqlCmd.Parameters.Add($"@IsQuizz", DbType.Boolean).Value = gameCont.IsQuizz;
            //sqlCmd.Parameters.Add($"@Developer", DbType.UInt32).Value = Games[i].Developer;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = gameCont.ID;

            ExecNQ(sqlCmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> clone et isparent ont été levés</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="Roms"></param>
        public void Update_Roms<T>(ObservableCollection<T> Roms) where T : iCT_Rom
        {
            Debug.WriteLine($"Update de la collection");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            for (int i = 0; i < Roms.Count; i++)
            {
                T game = Roms[i];
                //  string vals = null;

                sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Roms}]" +
                                        $"SET " +
                                        $"[Archive_Name]=@Archive_Name, " +
                                        $"[Description]=@Description, " +
                                        $"[Year]=@Year, " +
                                        $"[Manufacturer]=@Manufacturer, " +
                                        $"[Unwanted]=@Unwanted " +
                                        //
                                        $"WHERE ID=@ID";

                sqlCmd.Parameters.Add($"@Archive_Name", DbType.String).Value = Roms[i].Archive_Name;
                sqlCmd.Parameters.Add($"@Description", DbType.String).Value = Roms[i].Description;
                sqlCmd.Parameters.Add($"@Year", DbType.String).Value = Roms[i].Year;
                sqlCmd.Parameters.Add($"@Manufacturer", DbType.UInt32).Value = Roms[i].Manufacturer;
                sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = Roms[i].Unwanted;

                // condition
                sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = Roms[i].ID;

                ExecNQ(sqlCmd);
                UpdateProgress?.Invoke(this, i * 100 / Roms.Count);
            }
        }

        internal void Update_Company(CT_Constructeur ctComp)
        {
            Debug.WriteLine($"Update de {ctComp.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Developers}] SET [Nom]=@Nom WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = ctComp.Nom;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = ctComp.ID;

            ExecNQ(sqlCmd);
        }

        internal void Update_Constructeur(CT_Constructeur ctConst)
        {
            Debug.WriteLine($"Update de {ctConst.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Constructeurs}] SET [Nom]=@Nom WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = ctConst.Nom;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = ctConst.ID;

            ExecNQ(sqlCmd);
        }

        internal void Update_Genre(CT_Genre ctGenre)
        {
            Debug.WriteLine($"Update de {ctGenre.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Genres}] SET [Nom]=@Nom WHERE ID=@ID";

            sqlCmd.Parameters.Add($"@Nom", DbType.String).Value = ctGenre.Nom;

            // condition
            sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = ctGenre.ID;

            ExecNQ(sqlCmd);
        }

        public void Update_Machine(CT_Machine Machine)
        {
            Debug.WriteLine($"Update de {Machine.Nom}");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);

            sqlCmd.CommandText = $"UPDATE [{PProp.Default.T_Machines}] " +
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

        #region UNIQUE
        internal bool Flush_TempRoms()
        {
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);
            sqlCmd.CommandText = $"DELETE FROM [{PProp.Default.T_TempRoms}];";
            sqlCmd.CommandText += $"UPDATE sqlite_sequence SET [seq]=@seq WHERE [name]=@name;";

            sqlCmd.Parameters.AddWithValue("@seq", 0);
            sqlCmd.Parameters.AddWithValue("@name", PProp.Default.T_TempRoms);

            return ExecNQ(sqlCmd);

        }
        #endregion
    }
}
