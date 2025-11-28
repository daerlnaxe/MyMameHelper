using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.Models
{
    internal class AsyncWorkBool: IAsyncProgressWork
    {
        public delegate bool AsyncBoolAction(AsyncWindowProgressG window);
        public AsyncBoolAction go { get; set; }


        public List<object> Arguments { get; set; } = new List<object>();

        //public AsyncBoolAction go { get; set; }
        public IAsyncResult BeginGo(AsyncWindowProgressG window, AsyncCallback callback, object state)
        {
            if (go != null)
            {
                return go?.BeginInvoke(window, callback, state);
            }
            return null;
        }

        public object EndGo(IAsyncResult ar)
        {
            return go.EndInvoke(ar);   // <- renvoie bool, mais boxé en object
        }
  

 
    }
}
