using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PProp = MyMameHelper.Properties.Settings;


namespace MyMameHelper.Methods
{

    /// <summary>
    /// Logique de centralisation des requêtes d'insertion
    /// </summary>
    internal static class SaveInDB
    {
        internal static bool Insert_Manus(MyObservableCollection<CT_Constructeur> manufacturers)
        {
            AsyncWindowProgress awP = new AsyncWindowProgress();
            awP.Arguments = new List<object>() { manufacturers };
            awP.Message_Value = "Adding Manufacturer(s)";

            //awP.go += new AsyncWindowProgress.AsyncAction(Save_DoWork);
            awP.go += new AsyncWindowProgress.AsyncAction(Insert_AsyncManus);
            awP.ShowDialog();

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                MainWindow.NumberOf_Dev = sqReq.Count(PProp.Default.T_Manufacturers);
                return true;
            }

            return false;
            
        }



        /// <summary>
        /// Ajoute des Manufacturers en base
        /// </summary>
        /// <param name="windows"></param>
        internal static void Insert_AsyncManus(AsyncWindowProgress windows)
        {
            MyObservableCollection<CT_Constructeur> Manufacturers = (MyObservableCollection<CT_Constructeur>)windows.Arguments[0];

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => windows.AsyncUpProgressPercent(y));
                sqReq.Insert_Manus(Manufacturers, true);
                //return true;
            }
            //return false;
        }


    }
}
