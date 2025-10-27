using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite
{
    public class SqlOrder
    {
        public string[] orders;
        public Collate collate;
        public Sens sens;

        public SqlOrder(params string[] orders)
        {
            this.orders = orders;
        }

        public SqlOrder(Sens sens, params string[] orders)
        {
            this.orders = orders;
            this.sens = sens;
        }

        public SqlOrder(Collate coll, params string[] orders)
        {
            this.orders = orders;
            this.sens = Sens.Asc;
            this.collate = coll;
        }


        public SqlOrder(Collate coll , Sens sens = Sens.Asc, params string[] orders)
        {
            this.orders = orders;
            this.sens = sens;
            this.collate = coll;
        }
    }

    public enum Sens
    {
        Asc,
        Desc
    }

    public enum Collate
    {
        None,
        NOCASE,
        BINARY
    }
}
