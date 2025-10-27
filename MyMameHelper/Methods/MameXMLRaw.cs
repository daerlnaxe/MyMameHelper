using MyMameHelper.ContTable;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace MyMameHelper.Methods
{
    public class MameXMLRaw
    {
        private string _Filename;

        private AsyncLoading aLoad;

        private uint _Line;

        private  List<RawMameRom> _RomsCollec { get; set; } = new List<RawMameRom>();

        public List<RawMameRom> TryToParse(string filename)
        {
            _Filename = filename;
            _Line = 0;

            aLoad = new AsyncLoading();
            aLoad.go += new AsyncLoading.AsyncAction(AsyncParse);
            aLoad.ShowDialog();
            return _RomsCollec;
        }

        private void AsyncParse(AsyncLoading windows)
        {

            XmlTextReader textReader = new XmlTextReader(_Filename);
                        
            while (textReader.Read())
            {
                if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "machine")
                {
                    RawMameRom mRaw = new RawMameRom();
                    _RomsCollec.Add(mRaw);

                    mRaw.Name = textReader.GetAttribute("name");
                    mRaw.Source_File = textReader.GetAttribute("sourcefile");

                    mRaw.Is_Mechanical = textReader.GetAttribute("ismechanical") == "yes" ? true : false;
                    mRaw.Is_Bios = textReader.GetAttribute("isbios") == "yes" ? true : false;

                    mRaw.Clone_Of = textReader.GetAttribute("cloneof");
                    mRaw.Rom_Of = textReader.GetAttribute("romof");
                    mRaw.Sample_Of = textReader.GetAttribute("sampleof");

                    GetRomsInfos(textReader, mRaw);
                }

                //  Informer l'utilisateur
                _Line++;
                if (_Line % 1000 == 0)
                    aLoad.AsyncInform($"{_Line} lines readed");
            }

            //return roms;

            //MessageBox.Show($"Erreur:\n{exc.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
            // return null;

        }


        private void GetRomsInfos(XmlReader textReader, RawMameRom mRaw)
        {
            XmlReader subReader = textReader.ReadSubtree();
            // Children
            while (subReader.Read())
            {
                if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "description")
                    mRaw.Description = GetLeafContent(subReader);

                if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "year")
                    mRaw.Year = GetLeafContent(subReader);

                if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "manufacturer")
                    mRaw.Manufacturer = GetLeafContent(subReader);

                //  Informer l'utilisateur
                _Line++;
                if (_Line % 1000 == 0)
                    aLoad.AsyncInform($"{_Line} lines readed");
            }
        }

        /// <summary>
        /// Récupère le contenu d'une feuille >something<
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string GetLeafContent(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                    return reader.Value;
            }
            return null;
        }
    }
}
