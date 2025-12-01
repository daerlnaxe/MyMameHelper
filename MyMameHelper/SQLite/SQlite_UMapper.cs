using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite
{
    public sealed partial class SQLite_OP
    {
        /// <summary>
        /// Remappe machine_id pour les roms
        /// </summary>
        /// <param name="romsGrouped">Doit être un résultat groupé par source_file</param>
        internal void Map_RomMachine(List<CT_Machine> machines, List<string> sourceFiles)
        {
            Debug.WriteLine($"Update Machine_ID des Roms");
            SQLiteCommand sqlCmd = new SQLiteCommand(SQLiteConn);




            foreach (var srcFile in sourceFiles)
            {
                // Machine spécifique
                var machine = machines.FirstOrDefault((x) => x.Nom.Equals(srcFile));

                /* // Par constructeur
                 if (machine == null)
                 {
                     string search = srcFile.Substring(0, srcFile.IndexOf('/'));
                     machine = machines.FirstOrDefault((x) => x.Nom.Equals(search));
                 }*/
                sqlCmd.CommandText = $"UPDATE [{tRom}] SET [Machine_Id]=@Machine_Id WHERE source_file='{srcFile}'"; ;

                // Met à null si on ne retrouve pas la correspondance
                if (machine == null)
                {
                    sqlCmd.Parameters.Add($"@Machine_Id", DbType.UInt32).Value = null;
                }                
                else
                {
                    // Pour gagner du temps
                    machines.Remove(machine);

                    // Commande SQL
                    sqlCmd.Parameters.Add($"@Machine_Id", DbType.UInt32).Value = machine.ID;
                }

                // Update
                ExecNQ(sqlCmd);
            }



            /*
            sqlCmd.Parameters.Add($"@Game_Name", DbType.String).Value = gameCont.Game_Name;
            sqlCmd.Parameters.Add($"@Description", DbType.String).Value = gameCont.Description;
            sqlCmd.Parameters.Add($"@Unwanted", DbType.Boolean).Value = gameCont.Unwanted;
            sqlCmd.Parameters.Add($"@Genre_Id", DbType.UInt32).Value = gameCont.Genre_Id;
            sqlCmd.Parameters.Add($"@Rate", DbType.UInt32).Value = gameCont.Rate;
            sqlCmd.Parameters.Add($"@IsMahjong", DbType.Boolean).Value = gameCont.IsMahjong;
            sqlCmd.Parameters.Add($"@IsQuizz", DbType.Boolean).Value = gameCont.IsQuizz;*/
            //sqlCmd.Parameters.Add($"@Developer_Id", DbType.UInt32).Value = gameCont.Developer_Id; Levé pour le moment

            // condition
            //sqlCmd.Parameters.Add($"@ID", DbType.UInt32).Value = gameCont.ID;

        }
    }
}
