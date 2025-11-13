using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyMameHelper.ContTable
{
    public static class Trans
    {
        /// <summary>
        /// Récupère sous forme de chaine, si la clé existe
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        public static string GetString(string key, Dictionary<string, object> dico)
        {
            return dico.ContainsKey(key) == true ? dico[key].ToString() : null;
        }

        public static string GetString(string key, SQLiteDataReader reader)
        {
            try
            {


                if (reader[key] is DBNull)
                    return null;

                return reader[key].ToString();
            }
            catch
            {
                throw new Exception($"Column {key} missing, Database alteration");
            
            }

        }

        /// <summary>
        /// Récupère sous forme de UInt?, si la clé existe
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        public static uint? GetNullableUInt(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key)) return null;
            if (dico[key] == DBNull.Value) return null;

            return Convert.ToUInt32(dico[key]);
        }

        /// <summary>
        /// Récupère sous forme uint
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        public static uint GetUInt(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key))
                return 0;

            return Convert.ToUInt32(dico[key]);
        }

        public static uint GetUInt(string key, SQLiteDataReader reader)
        {
            if (reader[key] is DBNull)
                return 0;

            return Convert.ToUInt32(reader[key]);
        }


        /// <summary>
        /// Récupère sous forme de UShort?, si la clé existe
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        internal static ushort? GetNullableUShort(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key)) return null;
            if (dico[key] == DBNull.Value) return null;

            return Convert.ToUInt16(dico[key]);
        }

        internal static Color? GetNullableColor(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key)) return null;
            if (dico[key] == DBNull.Value) return null;
            return (Color)ColorConverter.ConvertFromString(dico[key].ToString());
        }

        /// <summary>
        /// Récupère sous forme de UShort
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        internal static ushort GetUShort(string key, Dictionary<string, object> dico)
        {
            return Convert.ToUInt16(dico[key]);
        }

        internal static ushort GetUShort(string key, SQLiteDataReader reader)
        {
            if (reader[key] is DBNull)
                return 0;

            return Convert.ToUInt16(reader[key]);
        }

        /// <summary>
        /// Récupère la valeur booléenne si la clée existe
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        internal static bool? GetNullableBool(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key) || dico[key] == DBNull.Value)
                return null;

            switch (dico[key].ToString())
            {
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    return null;
            }

        }

        internal static bool? GetNullableBool(string key, SQLiteDataReader reader)
        {
            if (reader[key] is DBNull)
                return null;

            switch (reader[key].ToString())
            {
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    return null;
            }

        }

        /// <summary>
        /// Récupère la valeur booléenne
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        internal static bool GetBool(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key) || dico[key] is DBNull)
                return false;

            return Convert.ToBoolean(dico[key]);
        }

        internal static bool GetBoolFalse(string key, Dictionary<string, object> dico)
        {
            if (!dico.ContainsKey(key) || dico[key] is DBNull)
                return false;

            bool? val = dico[key] as bool?;

            return val == true ? true : false;
        }

        internal static bool GetBool(string key, SQLiteDataReader reader)
        {
            if (reader[key] is DBNull)
                return false;

            return Convert.ToBoolean(reader[key]);
        }

        internal static bool? GetBoolFalse(string key, SQLiteDataReader reader)
        {
            if ( reader[key] is DBNull)
                return false;

            bool? val = reader[key] as bool?;

            return val == true ? true : false;
        }


        /*
        internal static string? CapitalizeFirstLetter(string chaine)
        {
            if (String.IsNullOrEmpty(chaine)) return null;

            chaine = Char.ToUpper(chaine[0]) + chaine.Substring(1);

            bool capitalizenext = false;
            for (UInt16 i = 0; i < chaine.Length; i++)
            {                
                if (chaine[i] == ' ') capitalizenext = true;
                if (capitalizenext)
                {
                    chaine[i] = Char.ToUpper(chaine[i]);
                    capitalizenext = false;
                }

            }
        }*/


    }
}
