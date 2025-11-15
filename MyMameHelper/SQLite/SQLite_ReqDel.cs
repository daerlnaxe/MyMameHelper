
using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PProp = MyMameHelper.Properties.Settings;


namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_Op
    {

        public void Delete_Game(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Games}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);

        }
        public void Delete_Rom(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Roms}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);            
        }

        public void Delete_RomById(uint Id)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Roms} WHERE ";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);            

            ExecNQ(command);

            // Suppression de l'inclusion dans le jeu.
            string sqlSel = $"SELECT FROM {PProp.Default.T_Games} WHERE [Roms]LIKE'%|{Id}|%'";


        }

        public void Delete_Companie(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Developers}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);

        }

        public void Delete_Constructor(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Manufacturers}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);

        }

        public void Delete_Genre(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Genres}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);

        }


        public void Delete_Machine(SqlCond[] conditions)
        {
            string sql = $"DELETE FROM {PProp.Default.T_Machines}";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, conditions);

            ExecNQ(command);

        }


        /// <summary>
        /// Méthode générique pour effacer
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conditions"></param>
        private void Delete_This(Obj_Select objSelect)
        {
            string sql = $"DELETE FROM [{objSelect.Table}] ";
            SQLiteCommand command = new SQLiteCommand(sql, SQLiteConn);

            Condition_TreatMt(command, objSelect.Conditions);

            Debug.WriteLine($"Lancement de: {command.CommandText}");

            ExecNQ(command);
        }


    }
}
