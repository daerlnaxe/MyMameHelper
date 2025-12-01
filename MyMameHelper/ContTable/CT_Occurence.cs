using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.ContTable
{
    internal class CT_Occurence<T>
    {
        internal T Objet;
        internal uint Occurences;

        public CT_Occurence(T objet, uint occurences)
        {
            Objet = objet;
            Occurences = occurences;
        }


    }
}
