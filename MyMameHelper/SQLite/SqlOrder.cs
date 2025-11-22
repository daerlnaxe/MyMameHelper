using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite
{
    public class SqlOrder
    {
        /// <summary>
        /// 
        /// </summary>
        public string field;

        /// <summary>
        /// 
        /// </summary>
        public Collate collate;
        
        /// <summary>
        /// Sens
        /// </summary>
        public Sens sens;

        public SqlOrder(string field)
        {
            this.field = field;
        }

        public SqlOrder(Sens sens, string field)
        {
            this.field = field;
            this.sens = sens;
        }

        public SqlOrder(Collate coll, string field)
        {
            this.field = field;
            this.sens = Sens.Asc;
            this.collate = coll;
        }


        public SqlOrder(string field, Collate coll , Sens sens = Sens.Asc)
        {
            this.field = field;
            this.sens = sens;
            this.collate = coll;
        }
    }

    public enum Sens
    {
        Asc,
        Desc
    }

    /// <summary>
    /// Options de comparaison
    /// </summary>
    public enum Collate
    {                      
        None,   // Pas de collation particulière (utilise le comportement par défaut)
        NOCASE, // Comparaison insensible à la casse (majuscules = minuscules)
        BINARY  // Comparaison insensible à la casse (majuscules = minuscules)
    }
}
