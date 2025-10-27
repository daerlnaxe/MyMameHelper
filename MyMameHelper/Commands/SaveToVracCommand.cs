using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyMameHelper.Commands
{
    public class SaveToVracCommand : ICommand, INotifyPropertyChanged
    {
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _IsActive { get; set; }
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if(value!=_IsActive)
                {
                    _IsActive = value;
                    CanExecuteChanged.Invoke(this, null);
                }
            }
        }       
        

        public bool CanExecute(object parameter)
        {
            return IsActive;
        }

        public void Execute(object parameter)
        {
        

            
        }



    }
}
