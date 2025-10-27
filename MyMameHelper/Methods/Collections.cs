using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Methods
{
    internal static class Collections
    {
       internal static ObservableCollection<T> Return_Missing<T>(ObservableCollection<T> source, ObservableCollection<T> toFilter)
        {
            ObservableCollection<T> tempo = new ObservableCollection<T>(toFilter);
            return tempo;
        }

    }
}
