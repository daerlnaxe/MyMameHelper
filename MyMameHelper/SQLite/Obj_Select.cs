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
        public SqlOrder Orders { get; set; }
        public uint? Limit { get; set; }

        public string Requete { get; private set; }

        public Obj_Select(string table = null, bool all = false, string[] colonnes = null, SqlCond[] conditions = null, string[] groups = null, SqlOrder orders = null, uint? limit = null)
        {
            Table = table;
            All = all;
            Colonnes = colonnes;
            Conditions = conditions;
            Groups = groups;
            Orders = orders;
            Limit = limit;
        }

        public Obj_Select(string table, string[] colonnes, SqlOrder orders=null)
        {
            Table = table;
            Colonnes = colonnes;
            Orders = orders;
        }

        /*
        public Obj_Select(string table, bool all, SqlOrder orders=null)
        {
            Table = table;
            All = all;
            Orders = orders;
        }*/

        public Obj_Select(bool all, SqlOrder orders)
        {
            All = all;
            Orders = orders;
        }

        public Obj_Select(string[] colonnes, SqlOrder orders)
        {
            Colonnes = colonnes;
            Orders = orders;
        }

        public void Add(params SqlCond[] conds)
        {
            Conditions = conds;
        }
    }

    public enum ReqType
    {
        Select
    }
}
