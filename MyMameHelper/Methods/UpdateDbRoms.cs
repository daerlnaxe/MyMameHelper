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
    public class UpdateDbRoms<T> where T: iCT_Rom
    {
        private ObservableCollection<T> _Roms;

        public void Update_GamesTable(ObservableCollection<T> roms)
        {
            _Roms = roms;

            if (MessageBox.Show("Update this roms ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                sqReq.Update_Roms<T>(_Roms);
            }
        }
    }
}
