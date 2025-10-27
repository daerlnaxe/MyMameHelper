
using MyMameHelper.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_Req : IDisposable
    {
        internal const string MOT_DE_PASSE = @"jw9s4X#7~S4#4P-y65_Sk-k@GmWG}y3r~V7e476-:DC-4VxgpB";
        public delegate void SendIntValue(object sender, int value);
        public event SendIntValue UpdateProgress;

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
        public SQLite_Req()
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



        #region commun
        /// <summary>
        /// Définit les colonnes à sélectionner
        /// </summary>
        /// <param name="objSelect"></param>
        /// <returns></returns>
        public string Cols_TreatMt(Obj_Select objSelect)
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

            return cols2Sel;
        }

        /// <summary>
        /// Ajoute une gestion du groupage sous forme de requête paramétrée
        /// </summary>
        /// <param name="command"></param>
        /// <param name="groups"></param>
        public void Group_TreatMt(SQLiteCommand command, string[] groups = null)
        {
            if (groups == null)
                return;

            command.CommandText += " GROUP BY ";

            // Traitement des group
            int i = 0;
            foreach (string group in groups)
            {
                if (i > 0) command.CommandText += ", ";
                command.CommandText += $"[{group}]";
                i++;
            }
        }


        /// <summary>
        /// Ajoute des conditions sous requête paramétrée à la requête
        /// </summary>
        /// <param name="command"></param>
        /// <param name="conditions"></param>
        public void Condition_TreatMt(SQLiteCommand command, SqlCond[] conditions)
        {
            // Ajout du mot clé where
            if (conditions == null)
                return;

            command.CommandText += " WHERE";

            // Traitement des conditions
            //int i = 0;
            for (int i = 0; i < conditions.Length; i++)
            {
                SqlCond cond = conditions[i];

                // if (i > 0) command.CommandText += ", ";
                //IEnumerable<string> munch = cond.Colonne.Split('.').Select(x => x = $"[{x}]");
                string[] munch = cond.Colonne.Split('.');
                munch[0] = $"[{munch[0]}]";

                cond.Valeur = this.FilterParameter(cond.Valeur);
                command.CommandText += $" {cond.Get_Linker()} {String.Join(".", munch)}{cond.Get_Operateur()}'{cond.Valeur}'";
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="orderCom"></param>
        public void Order_TreatMt(SQLiteCommand command, SqlOrder orderCom)
        {
            if (orderCom == null)
                return;

            command.CommandText += " ORDER BY";

            // traitement de l'ordre
            int i = 0;
            foreach (string order in orderCom.orders)
            {
                if (i > 0) command.CommandText += ", ";
                command.CommandText += $" [{order}]";
                i++;
            }

            if (orderCom.collate != Collate.None) command.CommandText += $" COLLATE {orderCom.collate}";

            command.CommandText += $" {orderCom.sens}";
        }


        /// <summary>
        /// Execute la requête avec gestion erreurs
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        internal bool ExecNQ(SQLiteCommand sqlCmd)
        {
            try
            {
                sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException sqlExc)
            {
                MessageBox.Show($"{sqlExc.Message}\n{sqlCmd.CommandText}", "Sqlite Exception");
                Console.WriteLine(sqlExc);
                return false;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return false;
            }

        }


        private string FilterParameter(string value)
        {
            string result = string.Empty;
            result = value.Replace(";", "") ;

            return result;
        }
        #endregion

        /// <summary>
        /// Renvoie un int ou bien un null selon l'objet donné
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal int? GetNullableInteger(object value)
        {
            if (value == DBNull.Value) return null;

            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Vérifie la présence d'une table;
        /// </summary>
        /// <param name="table_name"></param>
        /// <returns></returns>
        internal bool Check_Table(string table_name)
        {
            string sql = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{table_name}'";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();

            return reader.HasRows;
        }

        internal bool Check_Column(string Table_Name, string column)
        {
            string sql = $"pragma table_info({Table_Name})";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var colonne = reader.GetFieldValue<string>(1);
                if (colonne.Equals(column))
                    return true;
            }

            return false;
        }


        public void Dispose()
        {
            Debug.WriteLine("SQlite_Test: Dispose");
            if (SQLiteConn != null)
            {
                SQLiteConn.Close();
                Debug.WriteLine("Etat de la connexion: " + SQLiteConn.State);
                SQLiteConn.Dispose();
            }
        }


    }
}
