using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PProp = MyMameHelper.Properties.Settings;

namespace MyMameHelper.Methods
{
    public static class WorkWithDb
    {
        /// <summary>
        /// Compare avec le contenu de la base de donnée
        /// </summary>
        /// <param name="collecGames"></param>
        /// <returns></returns>
        public static bool FilterGames(ObservableCollection<CT_Game> collecGames)
        {
            if (MessageBox.Show("Filter by comparison with DataBase ?\n If not, you couldn't proceed to recording but you could make it manually later to delock the funtionnality",
                                "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                List<CT_Game> gamesSaved;
                using (SQLite_Req sqReq = new SQLite_Req())
                {
                    gamesSaved = sqReq.Get_ListOf_Games(new Obj_Select(all:true));
                }

                Comparison(collecGames, gamesSaved, (x,y)=> x.Equals(y));

                return true;
            }
            return false;
        }

        private static void Comparison<T>(ObservableCollection<T> collec, List<T> db, Func<T,T, bool> comp)
        {

            for (int i = 0; i < db.Count; i++)
            {
                T element2 = db[i];
                T toRem = default(T);
                foreach (T element1 in collec)
                {
                    if (comp(element1, element2))
                        toRem = element1;
                }

                collec.Remove(toRem);
            }
        }
    }
}
