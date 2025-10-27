using MyMameHelper.ContTable;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyMameHelper.Commands
{
    public class SearchCommand : ICommand, INotifyPropertyChanged
    {
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if(value!= _IsActive)
                {

                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsActive;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
