using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite
{
    public class Obj_Select
    {
        public string Table { get; set; }
        public bool All { get; set; }
        public string[] Colonnes { get; set; }
        public SqlCond[] Conditions { get; set; }
        public string[] Groups { get; set; }
        public SqlOrder[] Orders { get; set; }
        public uint? Limit { get; set; }

        public string Requete { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table">Table de la base</param>
        /// <param name="all">Tous les champs</param>
        /// <param name="colonnes"></param>
        /// <param name="conditions"></param>
        /// <param name="groups"></param>
        /// <param name="orders"></param>
        /// <param name="limit">Limite des résultats</param>
        /// <example>
        /// order: new []{}
        /// </example>
        public Obj_Select(string table = null, bool all = false, string[] colonnes = null, string[] groups = null, uint? limit = null)
        {
            Table = table;
            All = all;
            Colonnes = colonnes;
            //Conditions = conditions;
            Groups = groups;
           // Orders = orders;
            Limit = limit;
        }

        public Obj_Select(string table, string[] colonnes)
        {
            Table = table;
            Colonnes = colonnes;
         //   Orders = orders;
        }

        /*
        public Obj_Select(string table, bool all, SqlOrder orders=null)
        {
            Table = table;
            All = all;
            Orders = orders;
        }*/

        public Obj_Select(bool all/*, params SqlOrder[] orders*/)
        {
            All = all;
            //Orders = orders;
        }

        public Obj_Select(string[] colonnes/*, params SqlOrder[] orders*/)
        {
            Colonnes = colonnes;
            //Orders = orders;
        }

        public void AddConds(params SqlCond[] conds)
        {
            Conditions = conds;
        }

        internal void AddOrders(params SqlOrder[] sqlOrders)
        {
            this.Orders = sqlOrders;
        }
    }

    public enum ReqType
    {
        Select
    }
}
