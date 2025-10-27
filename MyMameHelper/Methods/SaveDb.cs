using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMameHelper.Methods
{
    public class SaveDb<T> where T: iCT_Games
    {
        private ObservableCollection<T> _Games;
  
        public void GamesTable(ObservableCollection<T> games)
        {
            _Games = games;

            if (MessageBox.Show("Save to 'Games' table ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(SaveGames_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();
            }
        }

        public void VracTable(ObservableCollection<T> games)
        {
            _Games = games;

            if (MessageBox.Show("Save to 'Vrac' table ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(SaveVrac_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();
            }
        }

        /*
         *  Asynchrone work
         */
        public void SaveGames_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            //get the provided argument as usual
            object myArgument = e.Argument;

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));
                sqReq.Insert_CollecInGames(_Games);
            }
        }

        public void SaveVrac_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {           
            //get the provided argument as usual
            object myArgument = e.Argument;

            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));
               // sqReq.Insert_CollecInVrac(_Games);
            }
        }





    }

}
