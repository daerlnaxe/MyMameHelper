using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMameHelper.SQLite

{
    public class SqlCond
    {
        public Linker? Link { get; set; }
        public string Colonne { get; set; }
        public string Valeur { get; set; }


        public eWhere Operateur { get; set; }

        public string Get_Operateur()
        {
            switch (Operateur)
            {
                case eWhere.Equal: return "=";
                case eWhere.Not_Equal: return "!=";
                case eWhere.Like: return " LIKE ";
                case eWhere.Not_Like: return " NOT LIKE ";
                case eWhere.Is: return " IS ";
                case eWhere.Is_Not: return " IS Not ";
                default: return null;
            }
        }

        internal string Get_Linker()
        {

            switch (Link)
            {
                case Linker.And: return "AND ";
                default: return "";
            }
        }


        public SqlCond(string colonne, eWhere operateur, string valeur = null)
        {
            Colonne = colonne;
            Operateur = operateur;
            Valeur = valeur;
        }

        public SqlCond(string colonne, eWhere operateur, uint? valeur)
        {
            Colonne = colonne;
            Operateur = operateur;
            Valeur = valeur.ToString();
        }
        public SqlCond(Linker link, string colonne, eWhere operateur, string valeur)
        {
            Link = link;
            Colonne = colonne;
            Operateur = operateur;
            Valeur = valeur.ToString();
        }
        public SqlCond(Linker link, string colonne, eWhere operateur, uint? valeur)
        {
            Link = link;
            Colonne = colonne;
            Operateur = operateur;
            Valeur = valeur.ToString();
        }

        public SqlCond() { }


    }

    public enum Linker
    {
        And,
        Or,
        Not,
        Not_And,
        Not_Or
    }

}
