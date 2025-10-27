using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMameHelper.Parsers
{
    static class MameExportParser
    {
        public static List<CT_Game> Try_TxtParse(string file)
        {
            List<CT_Game> Games = new List<CT_Game>();
            try
            {
                using (StreamReader stream = new StreamReader(file))
                {
                    string[] lines = System.IO.File.ReadAllLines(file);
                    lines = lines.Skip(1).ToArray();
                    foreach (string line in lines)
                    {
                        string[] gameInfo = line.Split('"');

                        if (gameInfo.Length < 2)
                            throw new Exception("Format non valide");

                        Games.Add(new CT_Game(gameInfo[0].Trim(), gameInfo[1].Trim()));
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }


            return Games;
        }
    }
}
