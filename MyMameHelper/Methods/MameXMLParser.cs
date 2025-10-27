using MyMameHelper.ContTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyMameHelper.Methods
{
    public class MameXMLParser
    {
        public MyObservableCollection<CT_Game> GamesCollec { get; set; } = new MyObservableCollection<CT_Game>();
        public MyObservableCollection<CT_Bios> BiosCollec { get; set; } = new MyObservableCollection<CT_Bios>();

        public MyObservableCollection<CT_Mechanical> MecanicsCollec { get; set; } = new MyObservableCollection<CT_Mechanical>();


        /// <summary>
        /// Gestionnaire de roms (handle)
        /// </summary>
        /// <param name="filename"></param>
        private void ParseXML(string filename)
        {
            /*_DocX = new XmlDocument();
            _DocX.Load(filename);*/
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.Read())
            {
                #region Machine
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "machine")
                {
                    if (reader.GetAttribute("isbios") != null)
                        AddToBios(reader);
                    else if (reader.GetAttribute("ismechanical") != null)
                        AddToMechanical(reader);
                    else
                        AddToGames(reader);
                    /*
                    // Source File
                    game.SourceFile = reader.GetAttribute("sourcefile");
                    // Mécanique
                    string isMec = reader.GetAttribute("ismechanical");
                    if (isMec != null)
                        game.IsMechanical = isMec == "yes" ? true : false;
                        */

                    //Lecture attributs Machine
                    for (int attInd = 0; attInd < reader.AttributeCount; attInd++)
                    {
                        reader.MoveToAttribute(attInd);
                        if (reader.Name != "name"
                            && reader.Name != "cloneof"
                            && reader.Name != "ismechanical"
                            && reader.Name != "sourcefile"
                            )
                        {
                            Console.WriteLine(reader.Name);
                            Console.WriteLine(reader.Value);
                        }
                    }
                }
                #endregion Machine

            }
        }

               
        #region AddMethods
        // Ajoute une rom bios à la liste
        private void AddToBios(XmlTextReader reader)
        {
            // On récupère le nom
            string name = reader.GetAttribute("name");

            // Est ce un clone ?
            string cloneof = reader.GetAttribute("cloneof");

            CT_Bios bios = null;
            // si ce n'est pas un clone
            if (cloneof == null)
            {
                // Recherche si existe déjà en base
                bios = BiosCollec.FirstOrDefault(x => x.Bios_Name.Equals(name));

                // Si le jeu n'a pas encore été ajouté
                if (bios == null)
                {
                    bios = new CT_Bios();
                    bios.Bios_Name = name;
                    BiosCollec.Add(bios);
                }

                CT_Rom parent = new CT_Rom(name);
                parent.IsParent = true;
                GetRomInfos(reader.ReadSubtree(), parent);
                bios.Roms.Add(parent);
            }
            // Si c'est un clone
            else
            {
                // On vérifie que le parent existe déjà
                bios = BiosCollec.FirstOrDefault(x => x.Bios_Name.Equals(cloneof));

                // Si le jeu n'existe pas
                if (bios == null)
                {
                    bios = new CT_Bios();
                    bios.Bios_Name = cloneof;
                    BiosCollec.Add(bios);
                }

                CT_Rom clone = new CT_Rom(name);
                GetRomInfos(reader.ReadSubtree(), clone);
                bios.Roms.Add(clone);

            }
        }

        /// <summary>
        /// Jeux
        /// </summary>
        /// <param name="reader"></param>
        private void AddToGames(XmlTextReader reader)
        {
            // On récupère le nom
            string name = reader.GetAttribute("name");

            // Est ce un clone ?
            string cloneof = reader.GetAttribute("cloneof");

            CT_Game game = null;
            // si ce n'est pas un clone
            if (cloneof == null)
            {
                // Recherche si existe déjà en base
                game = GamesCollec.FirstOrDefault(x => x.Game_Name.Equals(name));

                // Si le jeu n'a pas encore été ajouté
                if (game == null)
                {
                    game = new CT_Game();
                    game.Game_Name = name;
                    GamesCollec.Add(game);
                }

                CT_Rom parent = new CT_Rom(name);
                parent.IsParent = true;
                GetRomInfos(reader.ReadSubtree(), parent);
                game.Roms.Insert(0, parent);
            }
            // Si c'est un clone
            else
            {
                // On vérifie que le parent existe déjà
                game = GamesCollec.FirstOrDefault(x => x.Game_Name.Equals(cloneof));

                // Si le jeu n'existe pas
                if (game == null)
                {
                    game = new CT_Game();
                    game.Game_Name = cloneof;
                    GamesCollec.Add(game);
                }

                CT_Rom clone = new CT_Rom(name);
                GetRomInfos(reader.ReadSubtree(), clone);
                game.Roms.Add(clone);

            }
        }

        // Ajoute une rom Mécanique à la liste
        private void AddToMechanical(XmlTextReader reader)
        {
            string name = reader.GetAttribute("name");

            // Est ce un clone ?
            string cloneof = reader.GetAttribute("cloneof");

            CT_Mechanical meca = null;
            // si ce n'est pas un clone
            if (cloneof == null)
            {
                // Recherche si existe déjà en base
                meca = MecanicsCollec.FirstOrDefault(x => x.Meca_Name.Equals(name));

                // Si le jeu n'a pas encore été ajouté
                if (meca == null)
                {
                    meca = new CT_Mechanical();
                    meca.Meca_Name = name;
                    MecanicsCollec.Add(meca);
                }

                RawMameRom parent = new RawMameRom(name);
                parent.IsParent = true;
                GetRomInfos(reader.ReadSubtree(), parent);
                meca.Roms.Insert(0, parent);

            }
            // Si c'est un clone
            else
            {
                // On vérifie que le parent existe déjà
                meca = MecanicsCollec.FirstOrDefault(x => x.Meca_Name.Equals(cloneof));

                // Si le jeu n'existe pas
                if (meca == null)
                {
                    meca = new CT_Mechanical();
                    meca.Meca_Name = cloneof;
                    MecanicsCollec.Add(meca);
                }

                RawMameRom clone = new RawMameRom(name);
                GetRomInfos(reader.ReadSubtree(), clone);
                meca.Roms.Add(clone);
            }

        }

        #endregion


        #region getInfos


        private void GetRomInfos(XmlReader reader, RawMameRom rom)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "description")
                    rom.Description = GetLeafContent(reader);

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "year")
                    rom.Year = GetLeafContent(reader);

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "manufacturer")
                    rom.Manufacturer = GetLeafContent(reader);
            }
        }

        #endregion
        
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
