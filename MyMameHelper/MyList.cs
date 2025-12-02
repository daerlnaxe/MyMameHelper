using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper
{
    internal class MyList<T>: List<T>
    {
        private List<RawMameRom> resultats;

        public MyList()
        {

        }


        public MyList(IList<T> resultats)
        {
            this.AddRange( resultats);
        }



        public List<T> ChangeContent
        {
            set
            {
                this.Clear();
                foreach (T element in value)
                    this.Add(element);
            }

        }


        public void AddRange(IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (T element in collection)
                this.Add(element);

        }


        internal void RemoveRange(IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");


            foreach (T element in collection)
                this.Remove(element);

            

        }
    }
}
