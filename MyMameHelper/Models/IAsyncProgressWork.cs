using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Models
{
    /// <summary>
    /// Pour les fenêtres de travail asynchrone
    /// </summary>
    internal interface IAsyncProgressWork
    {
        IAsyncResult BeginGo(AsyncWindowProgressG window, AsyncCallback callback, object state);

        List<object> Arguments { get; set; }

        object EndGo(IAsyncResult result);

    }
}
