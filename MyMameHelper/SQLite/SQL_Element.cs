using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite
{
    internal class SQL_Element
    {
        internal Type type;
        internal string column;
        internal object value;
  

        public SQL_Element(Type t, string c, object v)
        {
            this.type = t;
            this.column = c;
            this.value = v;
        }
    }
}
