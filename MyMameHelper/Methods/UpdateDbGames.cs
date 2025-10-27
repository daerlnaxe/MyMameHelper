using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMameHelper.Methods
{
    public class UpdateDbGames<T> where T: iCT_Games
    {
        private ObservableCollection<T> _Games;

        public void Update_GamesTable(ObservableCollection<T> games)
        {
            _Games = games;

            if (MessageBox.Show("Update this games ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(Update_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();
            }

            MessageBox.Show("Database updated");
        }

        private void Update_DoWork(ProgressWindow sender, DoWorkEventArgs e)
        {
            using (SQLite_Req sqReq = new SQLite_Req())
            {
                sqReq.UpdateProgress += ((x, y) => sender.SetProgress(y));
                sqReq.Update_Games<T>(_Games);
            }
        }
    }
}
