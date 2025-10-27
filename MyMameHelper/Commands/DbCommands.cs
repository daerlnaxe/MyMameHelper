using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyMameHelper.Commands
{
    public static class DbCommands
    {
        public static readonly RoutedUICommand SaveToVracCmd = new RoutedUICommand("Save to dbBuffer", "SaveToVracCmd", typeof(DbCommands));


        internal static void SaveToVrac()
        {
            /*
            if (MessageBox.Show("Save to 'En Vrac' table ? ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProgressWindow progressW = new ProgressWindow();
                progressW.DoWork += new ProgressWindow.DoWorkEventHandler(Save_DoWork);

                progressW.Total = 100;
                progressW.ShowDialog();
            }*/
        }
    }
}
