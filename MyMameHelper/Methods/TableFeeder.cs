using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace MyMameHelper.Methods
{

    internal static class TableFeeder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Synthesizer, sampler,
        /// </remarks>
        /// 
        static List<string> _Audio = new List<string>()
        {
            "akai/akaivx600",
            "casio/fz1",
            "ensoniq/esq1",
            "kawai/k1",
            "kawai/k4",
            "kawai/k5",
            "kawai/sx240",
            "korg/korgds8",
            "korg/korgdss1",
            "korg/korgdw8k",
            "korg/korgtriton",
            "korg/korgz3",
            "korg/microkorg",
            "korg/poly61",
            "korg/poly800",
            "korg/polysix",
            "novation/basssta",
            "philips/philipsbo",
            "roland/alphajuno",
            "roland/juno106",
            "roland/juno6",
            "roland/roland_d10",
            "roland/roland_d50",
            "roland/roland_d70",
            "roland/roland_jd800",
            "roland/roland_jv80",
            "roland/roland_jx3p",
            "roland/roland_jx8p",
            "roland/roland_s10",
            "roland/roland_s50",
            "yamaha/yman1x",
            "yamaha/ymdx100",
            "yamaha/ymdx11",
            "yamaha/ymdx7",
            "yamaha/ymdx9",
            "yamaha/ymsy35",
        };

        static List<string> _Echec = new List<string>()
        {
            "cxg"
        };


        static List<string> _Bellfruits = new List<string>()
        {
            "barcrest/mpu1",
"barcrest/mpu4avan",
"barcrest/mpu4bwb",
"barcrest/mpu4crystal",
"barcrest/mpu4empire",
"barcrest/mpu4mod2sw",
"barcrest/mpu4mod4oki",
"barcrest/mpu4union",
"barcrest/mpu4unsorted",
"barcrest/mpu4vid",
"barcrest/mpu5sw",
            "bfm/bfm_ad5sw",
            "bfm/bfm_blackbox",
            "bfm/bfm_sc1",
            "bfm/bfm_sc2",
            "bfm/bfm_sc4",
            "bfm/bfm_sc5sw",
            "bfm/bfm_swp",
            "bfm/bfmsys83",
            "bfm/bfmsys85",
            "cirsa/missbamby",
            "funworld/supercrd",
            "igs/goldstar",
            "igs/igs_m027",
            "igs/igs_m027xa",
            // all
            "jpm/jpmimpctsw",
            "jpm/jpmmps",
            "jpm/jpms80",
            "jpm/jpmsys5sw",
            "jpm/pluto5",
            "konami/konmedal68k",
            "maygay/maygay1bsw",
            "maygay/maygayep",
            "misc/39in1",
            "misc/amaticmg",
            "misc/astrafr",
            "misc/atronic",
            "misc/blitz68k",
            "misc/cromptons",
            "misc/dfruit",
            "misc/ecoinfr",
            "misc/fresh",
            "misc/hazelgr",
            "misc/jungleyo",
            "misc/mpu12wbk",
            "misc/multfish",
            "misc/multfish_boot",
            "misc/sfbonus",
            "nichibutsu/jangou",
            "pc/fruitpc",
            "playmark/sderby",
            "recfranco/rfslotsmcs48",
            "shared/fruitsamples",
            "shared/sec",
            "sigma/sigmab52",

        };

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Bellfruit, Super Fruit
        /// </remarks>
        static List<string> _MoneyConst = new List<string>()
            {
                "adp",
                "adc",
                "adds",
                "adp",
                "amirix",
                "appliedconcepts",
                "aristocrat",
                "astrocorp",
                "barcrest",
                "bfm",
                "bmc",
                "bordun",
                "dynax",
                "excellent",
                "falco",
                "gridcomp",
                "igt",
                "jpm",
                "kiwako",
                "hegenerglaser",
                "maygay",
                "merit",
                "midcoin",
                "nmk",
                "subsino",
                "subsino2",
                "tigertel",
                "wing",
                "yeno",
                "zvt"
            };
        public static List<string> MoneyConst => _MoneyConst;


        private static List<string> _Periphs = new List<string>()
        {
            "brother",
            "canon"
        }
        ;

        /// <summary>
        /// Ordinateurs professionnels sans jeux
        /// </summary>
        private static List<string> _ProCompConst = new List<string>()
        {
            "act/*",
            "altos/*",
            "ampro/*",
            "att/*",
            "banctec/*",
            "ccs/*",
            "burroughs/*",
            "dms/dms5000",
            "esprit/esp250c",
            "esprit/executive10",
            "learsiegler/*",
            "ericsson/alfaskop41xx",
            "ericsson/e9161",
            "ericsson/eispc",
            "facit/f4431",
            "facit/facit4440"
        };



        /// <summary>
        /// Construit les machines en fonction de source_file des roms temporaires de M.A.M.E
        /// </summary>
        internal static Dictionary<string, List<CT_Machine>> Machine(List<CT_Occurence<RawMameRom>> groupedResultats)
        {
            //var machine = new List<CT_Machine>();
            //var machines = new Dictionary<string, List<CT_Machine>>();
            //List<CT_Machine> machinesSimple = new List<CT_Machine>();
            Dictionary<string, List<string>> notAccepted = new Dictionary<string, List<string>>();


            /*for (int i = 0; i < notAccepted.Count; i++)
                f.WriteLine(notAccepted[i]);
            */
            string prevConstructor = "";
            uint otherID = 1000;
            uint keepID = 1000;

            List<CT_Machine> isKnowedSystem = new List<CT_Machine>();
            List<CT_Machine> onlyConstructs = new List<CT_Machine>();

            List<string> skelettons = new List<string>();
            List<string> pinball = new List<string>();

            List<string> machinesASous = new List<string>();
            List<string> materielElectronique = new List<string>();
            List<string> mahjong = new List<string>();
            List<string> pass_notid = new List<string>();
            List<string> pass_id = new List<string>();
            List<string> mechanics = new List<string>();


            foreach (var grRes in groupedResultats)
            {
                var srcFile = grRes.Objet.Source_File;
                bool isdevice = grRes.Objet.Is_Device;
                uint occur = grRes.Occurences;


                if (isdevice)
                {
                    Debug.WriteLine($"Pass : {srcFile} est de type device");
                    continue;
                }


                // Extension du sourceFile
                string extension = srcFile.Substring(srcFile.LastIndexOf('.') + 1);

                // Machine
                string strMachine = srcFile.Substring(srcFile.IndexOf('/') + 1);
                strMachine = strMachine.Substring(0, strMachine.Length - extension.Length - 1);


                string strConstruct = srcFile.Substring(0, srcFile.IndexOf("/"));
                //Debug.WriteLine(strConstruct);


                CT_Machine machine = new CT_Machine()
                {
                    Nom = $"{strConstruct} - {strMachine}"
                };

                // Subdivision
                string rLine = $"{strConstruct} - {strMachine} ({occur})";
                // On ajoute le nom du constructeur pour tous les autres cas // (sauf pinball)
                if (onlyConstructs.FirstOrDefault(x => x.Nom.Equals(strConstruct)) == null)
                {
                    machine.ID = otherID;
                    machine.Nom = strConstruct;

                    //if (!strConstruct.Equals("pinball"))
                    onlyConstructs.Add(machine);

                    otherID++;
                }


                // Squelettes, non fonctionnel
                if (strConstruct.StartsWith("skeleton"))
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        skelettons.Add($"============== {strConstruct} - Squelettes, pas une machine  ==============");
                    }
                    skelettons.Add(rLine);

                    prevConstructor = strConstruct;

                    continue;

                }

                var res = IsKnowedSystem(strConstruct, ref machine, strMachine);
                // knowed
                if (res == 1)
                {
                    isKnowedSystem.Add(machine);

                    // On passe
                    continue;
                }
                // Jeu probable
                else if (res == 0)
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        pass_id.Add($"============== {strConstruct} ==============");
                    }

                    pass_id.Add(rLine);
                    if (occur > 50)
                        pass_id.Add($">>>>----------------------------------------- {occur} occurences");

                    prevConstructor = strConstruct;

                    //machine.Nom = strConstruct;
                    continue;
                }

                // Jeux d'argent

                if (IsMoneyGame(strConstruct, ref machine, strMachine))
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        machinesASous.Add($"============== {strConstruct} - Machines à sous  ==============");
                    }
                    machinesASous.Add(rLine);
                    prevConstructor = strConstruct;

                }

                // Roms mécaniques 
                else if (grRes.Objet.Is_Mechanical)
                {
                    // pinball
                    if (strConstruct.Equals("pinball"))
                        pinball.Add(rLine);
                    else
                    {

                    }
                    // systèmes

                    // autres

                    prevConstructor = strConstruct;

                }
                else
                {

                }
                /*


#region Matériel Electronique
else if (
    strConstruct.Equals("apple") ||
    strConstruct.Equals("agat") ||
    (strConstruct.Equals("alpha") && strMachine.StartsWith("alpha")) ||
    strConstruct.Equals("ausnz") ||
    strConstruct.Equals("beehive") ||
                dms/dms86.cpp
dms/zsbc3.cpp
                drc/zrt80.cpp
                epson/hx20.cpp
epson/px4.cpp
epson/px8.cpp
epson/qx10.cpp
                mupid/mdisk.cpp
                mupid/mupid2.cpp
    strConstruct.Equals("excalibur") ||




    strConstruct.Equals("commodore") ||

    //strConstruct.Equals("dec") ||
    
    strConstruct.Equals("elektor") ||
    
    strConstruct.Equals("fairchild") ||
    strConstruct.Equals("fairlight") ||
    strConstruct.Equals("force") ||
    strConstruct.Equals("fujitsu") ||
    strConstruct.Equals("grundy") ||
    strConstruct.Equals("hds") ||
    strConstruct.Equals("heathzenith") ||
    strConstruct.Equals("hp") ||
    strConstruct.Equals("hitachi") ||
    strConstruct.Equals("ibm") ||
    strConstruct.Equals("informer") ||
    strConstruct.Equals("intel") ||
    strConstruct.Equals("kawai") ||
    strConstruct.Equals("korg") ||
    
    strConstruct.Equals("kyber") ||
    
    strConstruct.Equals("liberty") ||
    strConstruct.Equals("linn") ||
    strConstruct.Equals("makerbot") ||
    strConstruct.Equals("matic") ||
    strConstruct.Equals("matsushita") ||
    strConstruct.Equals("mattel") ||
    strConstruct.Equals("mc") ||
    strConstruct.Equals("mera") ||
    strConstruct.Equals("microterm") ||
    strConstruct.Equals("mips") ||
    strConstruct.Equals("mits") ||
    //
    (strConstruct.Equals("misc")
         && (
             strMachine.Equals("adi_vt52") ||
             strMachine.Equals("mcm70") ||
             strMachine.Equals("nabupc") ||
             strMachine.Equals("vocalizer") ||
             strMachine.Equals("z80ne")
             )
    ) ||
    strConstruct.Equals("mitsubishi") ||
    strConstruct.Equals("moog") ||
    strConstruct.Equals("morrow") ||
    strConstruct.Equals("motorola") ||
    strConstruct.Equals("multitech") ||
   
    strConstruct.Equals("nakajima") ||
    strConstruct.Equals("natsemi") ||
    strConstruct.Equals("ncd") ||
    strConstruct.Equals("nec") ||
    strConstruct.Equals("netronics") ||
    strConstruct.Equals("novag") ||
    strConstruct.Equals("saitek") ||
    strConstruct.Equals("sealy") ||
    strConstruct.Equals("sequential") ||
    strConstruct.Equals("siemens") ||
    strConstruct.Equals("slicer") ||
    strConstruct.Equals("sord") ||
    strConstruct.Equals("stm") ||
    strConstruct.Equals("sun") ||
    strConstruct.Equals("suna") ||
    strConstruct.Equals("ta") ||
    strConstruct.Equals("tab") ||
    strConstruct.Equals("tektronix") ||
    strConstruct.Equals("telercas") ||
    strConstruct.Equals("televideo") ||
    strConstruct.Equals("tesla") ||
    strConstruct.Equals("ti") ||
    strConstruct.Equals("toshiba") ||
    strConstruct.Equals("trainer") ||
    strConstruct.Equals("trs") ||
    strConstruct.Equals("wyse") ||
    strConstruct.Equals("xerox") ||
    )
{
    if (!prevConstructor.Equals(strConstruct))
    {
        materielElectronique.Add($"============== {strConstruct} - Materiel Electronique ==============");
    }
    materielElectronique.Add(rLine);
}
#endregion Matériel Electronique*/
                /*
                #region Pinball
                else if (
                    strConstruct.Equals("meadows") //||
                    )
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        mechanics.Add($"============== {strConstruct} - Pinball ==============");
                    }
                    mechanics.Add(rLine);

                }
                #endregion Pinball
                */


                /*
                else
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        pass_notid.Add($"============== {strConstruct} ==============");
                    }
                    pass_notid.Add(rLine);
                    if (occur > 50)
                        pass_notid.Add($">>>>----------------------------------------- {occur} occurences");

                }*/




                machine = null;
            }


            using (System.IO.StreamWriter f = new System.IO.StreamWriter("passed_notID.log"))
            {
                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Pass Not ID                           *");
                f.WriteLine("****************************************************************");
                foreach (string line in pass_notid)
                    f.WriteLine(line);
            }

            using (System.IO.StreamWriter f = new System.IO.StreamWriter("passed_ID.log"))
            {
                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Pass ID                               *");
                f.WriteLine("****************************************************************");
                foreach (string line in pass_id)
                    f.WriteLine(line);

                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Skelettons                            *");
                f.WriteLine("****************************************************************");
                foreach (string line in skelettons)
                    f.WriteLine(line);


                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Materiel Electronique                 *");
                f.WriteLine("****************************************************************");
                foreach (string line in materielElectronique)
                    f.WriteLine(line);


                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Machine à sous                        *");
                f.WriteLine("****************************************************************");
                foreach (string line in machinesASous)
                    f.WriteLine(line);

                f.WriteLine("\n****************************************************************");
                f.WriteLine("*                        Mechanics                             *");
                f.WriteLine("****************************************************************");
                foreach (string line in mechanics)
                    f.WriteLine(line);
            }


            return new Dictionary<string, List<CT_Machine>>() { { "identified", isKnowedSystem } };
            /*
            //Ajout à la base
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                sqOP.Insert_Machines(machines, false, false);
                sqOP.Insert_Machines(machinesSimple, false, true);

              /*  for (int i = 0; i < machines.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machines[i].Nom}");
                    CT_Machine machine = machines[i];
                    


                }
                for (int i = 0; i < machinesSimple.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machinesSimple[i].ID} {machinesSimple[i].Nom}");
                    CT_Machine machine = machinesSimple[i];
                    sqOP.Insert_Machine(machine, false, true);


                }*/
            /*}*/




            //return machine;



        }



        private static List<String> Platform = new List<string> {
                   "alliedleisure",
                    "alpha", // microordinateur avec quelques portages de jeux
                    "ces",
                    "ceres",
                    "chess",
                    "chessking",
                    "cinematronics",
                    "cybiko",
                    "dg",
                    "edevices",
                    "efo",
                    "f32",
                    "funworld",
                    "fuuki",
                    "galaxian",
                    "gamepark",
                    "gametron",
                    "homebrew",
                    "homelab",
                    "husky",
                    "ice",
                    "itech",
                    "kaneko",
                    "metro",
                    "microkey",
                    "miltonbradley",
                    "microsoft",
                    "midw8080",
                    "modelracing",
                    "msx",
                    "nichibutsu",
                    "nix",
                    "nokia",
                    "pinball", // Simulateurs de pinball
                    "psikyo",
                    "rare",
                    "sanritsu",
                    "technos",
                    "tvgames",
                    "vsystem",
                    "yachiyo",
                    "yungsung",
                    "vtech",
                    "zaccaria"
        };

        /// <summary>
        /// Système sélectionnés par mes soins
        /// </summary>
        /// <param name="strConstruct"></param>
        /// <param name="machine"></param>
        /// <param name="strMachine"></param>
        /// <returns></returns>
        private static short IsKnowedSystem(string strConstruct, ref CT_Machine machine, string strMachine)
        {
            // Amiga: 1
            if (strConstruct.Equals("amiga"))
            {
                machine.IDConstructeur = 1;
                if (strMachine.StartsWith("amiga") || strMachine.StartsWith("arsystems"))
                {
                    return 1;
                }
                return 0;
            }
            // Amstrad: 2
            else if (strConstruct.Equals("amstrad"))
            {
                machine.IDConstructeur = 2;
                if (strMachine.StartsWith("amstrad"))
                {
                    return 1;
                }
                return 0;
            }
            // Atari: 3
            else if (strConstruct.Equals("atari"))
            {
                machine.IDConstructeur = 3;

                if (
                    strMachine.StartsWith("atari400") ||
                    strMachine.StartsWith("atarist") ||
                    strMachine.StartsWith("atarittl") ||
                    strMachine.StartsWith("atarisy1") ||
                    strMachine.StartsWith("atarisy2") ||
                    strMachine.StartsWith("atarisy4") ||
                    strMachine.StartsWith("atarig1") ||
                    strMachine.StartsWith("atarig42") ||
                    strMachine.StartsWith("atarigt") ||
                    strMachine.StartsWith("atarigx2") ||
                    strMachine.StartsWith("jaguar") ||
                    strMachine.StartsWith("lynx") ||
                    strMachine.StartsWith("mediagx")
                    )
                {
                    return 1;
                }

                return 0;

            }
            // Atlus: 4
            else if (strConstruct.Equals("atlus"))
            {
                machine.IDConstructeur = 4;
                if (strMachine.StartsWith("cave"))
                {
                    return 1;
                }
                return 0;
            }
            // Capcom : 5
            else if (strConstruct.Equals("capcom"))
            {
                machine.IDConstructeur = 5;

                if (strMachine.StartsWith("cps1") ||
                    strMachine.StartsWith("cps2") ||
                    strMachine.StartsWith("cps3")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Casio : 6
            else if (strConstruct.Equals("casio"))
            {
                machine.IDConstructeur = 6;

                if (strMachine.StartsWith("pickytlk"))
                {
                    return 1;
                }
                return 0;
            }
            // Cave: 7
            else if (strConstruct.Equals("cave"))
            {
                machine.IDConstructeur = 7;
                if (strMachine.StartsWith("cv1k"))
                {
                    return 1;
                }
                return 0;
            }
            // Commodore: 8
            else if (strConstruct.Equals("commodore"))
            {
                machine.IDConstructeur = 8;
                if (strMachine.StartsWith("c64dtv"))
                {
                    return 1;
                }
                return 0;
            }
            // Data East : 9
            else if (strConstruct.Equals("dataeast"))
            {
                machine.IDConstructeur = 9;

                if (
                    strMachine.StartsWith("dec0") ||
                    strMachine.StartsWith("dec8") ||
                    strMachine.StartsWith("deco32") ||
                    strMachine.StartsWith("deco_mlc") ||
                    strMachine.StartsWith("simpl156") ||
                    strMachine.StartsWith("decocass")
                    )
                {
                    return 1;
                }
                return 0;

            }
            // Dooyong: 10
            else if (strConstruct.Equals("dooyong"))
            {
                machine.IDConstructeur = 10;
                if (
                    strMachine.StartsWith("dooyong")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Emusy: 11
            else if (strConstruct.Equals("emusys"))
            {
                machine.IDConstructeur = 11;
                if (
                    strMachine.StartsWith("emu2") ||
                    strMachine.StartsWith("emu3") ||
                    strMachine.StartsWith("emu68k")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Eolith: 12
            else if (strConstruct.Equals("eolith"))
            {
                machine.IDConstructeur = 12;
                if (
                    strMachine.StartsWith("eolith") /*||
                            strMachine.StartsWith("emu3") ||
                            strMachine.StartsWith("emu68k")*/
                    )
                {
                    return 1;
                }

                return 0;
            }
            // Exidy: 13
            else if (strConstruct.Equals("exidy"))
            {
                machine.IDConstructeur = 13;
                if (
                    strMachine.StartsWith("exidy") ||
                    strMachine.StartsWith("exidy440") /*||
                            strMachine.StartsWith("emu68k")*/
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Gaelco: 14
            else if (strConstruct.Equals("gaelco"))
            {
                machine.IDConstructeur = 14;
                if (
                    strMachine.StartsWith("gaelco") ||
                    strMachine.StartsWith("gaelco2") ||
                    strMachine.StartsWith("gaelco3d")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Gottlieb: 15
            else if (strConstruct.Equals("gottlieb"))
            {
                machine.IDConstructeur = 15;
                if (
                    strMachine.StartsWith("gottlieb")/* ||
                            strMachine.StartsWith("exidy440") /*||
                            strMachine.StartsWith("emu68k")*/
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Gottlieb: 16
            else if (strConstruct.Equals("handheld"))
            {
                machine.IDConstructeur = 16;
                if (
                    strMachine.StartsWith("hh_sm510") ||
                    strMachine.StartsWith("hh_tms1k") ||
                    strMachine.StartsWith("hh_hmcs40")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // IGS :17
            else if (strConstruct.Equals("igs"))
            {
                machine.IDConstructeur = 17;
                if (
                    strMachine.StartsWith("goldstar") ||
                    strMachine.StartsWith("igs_m027") ||
                    strMachine.StartsWith("pgm") ||
                    strMachine.StartsWith("pgm2") ||
                    strMachine.StartsWith("pgm3")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Irem : 18
            else if (strConstruct.Equals("irem"))
            {
                machine.IDConstructeur = 18;
                if (

                    strMachine.StartsWith("m10") ||
                    strMachine.StartsWith("m5") ||
                    strMachine.StartsWith("m6") ||
                    strMachine.StartsWith("m7") ||
                    strMachine.StartsWith("m8") ||
                    strMachine.StartsWith("m9") ||
                    strMachine.StartsWith("m107")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Itech: 19
            else if (strConstruct.Equals("itech"))
            {
                machine.IDConstructeur = 19;
                if (

                    strMachine.StartsWith("iteagle") ||
                    strMachine.StartsWith("itech32") ||
                    strMachine.StartsWith("itech8")
                )
                {
                    return 1;

                }
                return 0;
            }
            // Jalleco: 20
            else if (strConstruct.Equals("jaleco"))
            {
                machine.IDConstructeur = 20;
                if (

                    strMachine.StartsWith("megasys1") ||
                    strMachine.StartsWith("ms32") ||
                    strMachine.StartsWith("tetris2p")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Kaneko: 21
            else if (strConstruct.Equals("kaneko"))
            {
                machine.IDConstructeur = 21;
                if (

                    strMachine.StartsWith("kaneko16") ||
                    strMachine.StartsWith("kaneko16")// ||
                                                     //strMachine.StartsWith("tetris2p")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Konami: 22
            else if (strConstruct.Equals("konami"))
            {
                machine.IDConstructeur = 22;
                if (

                    strMachine.StartsWith("hornet") ||
                    strMachine.StartsWith("ksys573") ||// Genre de playstation
                    strMachine.StartsWith("Nemesis") ||
                    strMachine.StartsWith("NWK-TR")  //||
                                                     //strMachine.StartsWith("tetris2p")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Midway - midw8080: 23
            else if (strConstruct.Equals("midw8080"))
            {
                machine.IDConstructeur = 23;
                if (
                    strMachine.StartsWith("8080bw") ||
                    strMachine.StartsWith("mw8080bw")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Midway : 24
            else if (strConstruct.Equals("midway"))
            {
                machine.IDConstructeur = 24;
                if (
                    strMachine.StartsWith("mcr") ||
                    strMachine.StartsWith("mcr3") ||
                    strMachine.StartsWith("midtunit") ||
                    strMachine.StartsWith("midvunit") ||
                    strMachine.StartsWith("midyunit") ||
                    strMachine.StartsWith("williams")
                )
                {
                    return 1;
                }
                return 0;
            }
            // Namco : 25
            else if (strConstruct.Equals("namco"))
            {
                machine.IDConstructeur = 25;

                if (
                    strMachine.StartsWith("namco1") ||
                    strMachine.StartsWith("namcos1") ||
                    strMachine.StartsWith("namco2") ||
                    strMachine.StartsWith("namcos2") ||
                    strMachine.StartsWith("namcops2")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Neogeo : 26
            else if (strConstruct.Equals("neogeo"))
            {
                machine.IDConstructeur = 26;

                if (
                    strMachine.StartsWith("midas") ||
                    strMachine.StartsWith("neogeo") ||
                    strMachine.StartsWith("neopcb")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Nintendo : 27
            else if (strConstruct.Equals("nintendo"))
            {
                machine.IDConstructeur = 27;

                if (
                    strMachine.StartsWith("aleck64") || // nintendo 64
                    strMachine.StartsWith("nes") ||
                    strMachine.StartsWith("gamecube") ||
                    strMachine.StartsWith("multigam") ||
                    strMachine.StartsWith("playch10") ||
                    strMachine.StartsWith("snesb") ||
                    strMachine.StartsWith("vsnes")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // nmk : 28
            else if (strConstruct.Equals("nmk"))
            {
                machine.IDConstructeur = 28;

                if (
                    strMachine.StartsWith("nmk16")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Sega : 29
            else if (strConstruct.Equals("sega"))
            {
                machine.IDConstructeur = 29;

                if (
                    strMachine.StartsWith("chihiro") ||
                    strMachine.StartsWith("dc_atomiswave") ||
                    strMachine.StartsWith("lindberghd") ||
                    strMachine.StartsWith("megaplay") ||    // Megadrive avec chrono
                    strMachine.StartsWith("megatech") ||    // z80
                    strMachine.StartsWith("model1") ||
                    strMachine.StartsWith("model2") ||
                    strMachine.StartsWith("model3") ||
                    strMachine.StartsWith("naomi") ||
                    strMachine.StartsWith("naomi2") ||
                    //strMachine.StartsWith("saturn") ||    // on dirait que c'est un bios
                    strMachine.StartsWith("segac2") ||  // Gamegear, Master System
                    strMachine.StartsWith("segag80") ||
                    strMachine.StartsWith("segas16") ||
                    strMachine.StartsWith("segas18") ||
                    strMachine.StartsWith("segas24") ||
                    strMachine.StartsWith("segas32") ||
                    strMachine.StartsWith("segasp") ||
                    strMachine.StartsWith("segaxbd") ||
                    strMachine.StartsWith("sg1000a") ||
                    strMachine.StartsWith("stv") ||         // Titans
                    strMachine.StartsWith("triforce") ||    // Gamecube en collaboration
                    strMachine.StartsWith("turbo") ||
                    strMachine.StartsWith("vicdual") ||

                    strMachine.StartsWith("system1") ||
                    strMachine.StartsWith("system16")
                    )
                {
                    return 1;
                }
                return 0;

            }
            // Seibu: 30
            else if (strConstruct.Equals("seibu"))      // Toki ...
            {
                machine.IDConstructeur = 30;
                if (
                    strMachine.StartsWith("seibuspi")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Seta: 31
            else if (strConstruct.Equals("seta"))
            {
                machine.IDConstructeur = 31;
                if (
                    strMachine.StartsWith("seta") ||
                    strMachine.StartsWith("seta2") ||
                    strMachine.StartsWith("simple_st0016") ||
                    strMachine.StartsWith("ssv") //||
                    )
                {
                    return 1;
                }
                return 0;
            }
            // sfrj: 32 - Jeux de Mahjong ou de Casino
            else if (strConstruct.Equals("sfrj"))
            {
                machine.IDConstructeur = 32;
                return 0;
            }
            // sgi: 33 - Basés sur Silicon graphics. Cruis'n usa etc..
            else if (strConstruct.Equals("sgi"))
            {
                machine.IDConstructeur = 33;
                return 0;
            }
            // Sharp: 34 
            else if (strConstruct.Equals("sharp"))
            {
                machine.IDConstructeur = 34;
                return 0;
            }
            // Sigma: 35 
            else if (strConstruct.Equals("sigma"))
            {
                machine.IDConstructeur = 35;

                if (
                    strMachine.StartsWith("sigma21") ||
                    strMachine.StartsWith("sigmab31") ||
                    strMachine.StartsWith("sigmab52") ||
                    strMachine.StartsWith("sigmab88") ||
                    strMachine.StartsWith("sigmab98") //||
                    )
                {
                    return 1;
                }
                return 0;
            }
            // SNK: 36
            else if (strConstruct.Equals("snk"))
            {
                machine.IDConstructeur = 36;

                if (
                    strMachine.StartsWith("snk") ||
                    strMachine.StartsWith("snk6502") ||
                    strMachine.StartsWith("snk68") //||

                    )
                {
                    return 1;
                }
                return 0;
            }
            // SNK: 37
            else if (strConstruct.Equals("sony"))
            {
                machine.IDConstructeur = 37;

                if (
                    strMachine.StartsWith("taitogn") ||
                    strMachine.StartsWith("zn")// ||

                    )
                {
                    return 1;
                }
                return 0;
            }
            // Taito: 38
            else if (strConstruct.Equals("taito"))
            {
                machine.IDConstructeur = 38;

                if (
                    strMachine.StartsWith("taito") ||
                    strMachine.StartsWith("tnzs") ||
                    strMachine.StartsWith("tsamurai") ||
                    strMachine.StartsWith("undrfire") ||
                    strMachine.StartsWith("volfied") ||
                    strMachine.StartsWith("wgp") ||
                    strMachine.StartsWith("wyvernf0")
                    )
                {
                    return 1;
                }
                return 0;
            }
            // Techmo: 39
            else if (strConstruct.Equals("tecmo"))
            {
                machine.IDConstructeur = 39;

                if (
                    strMachine.StartsWith("tecmo") ||
                    strMachine.StartsWith("wc90")// ||

                    )
                {
                    return 1;
                }
                return 0;
            }
            // Toaplan: 40
            else if (strConstruct.Equals("toaplan"))
            {
                machine.IDConstructeur = 40;

                if (
                    strMachine.StartsWith("toaplan")// ||

                    )
                {
                    return 1;
                }
                return 0;
            }

            //
            return -1;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="strConstruct"></param>
        /// <param name=""></param>
        /// <param name="strMachine"></param>
        /// <returns></returns>
        private static bool IsMoneyGame(string strConstruct, ref CT_Machine machine, string strMachine)
        {
            // equals
            for (int i = 0; i < _MoneyConst.Count; i++)
            {
                if (strConstruct.Equals(_MoneyConst[i]))
                    return true;
            }


            // Cas particuliers

            // acorn 
            if (strConstruct.Equals("acorn") && strMachine.Equals("aristmk5"))
                return true;
            // fidelity
            if (strConstruct.Equals("fidelity"))
            {
                switch (strMachine)
                {
                    case "bridgeb":
                        return true;
                    case "card":
                        return true;
                    case "desdis":
                        return true;
                    case "sc6":
                        return true;
                    case "sc8":
                        return true;
                    case "vcc":
                        return true;
                    case "vsc":
                        return true;
                    default:
                        return false;
                }
            }

            // misc
            if (strConstruct.Equals("misc"))
            {
                switch (strMachine)
                {
                    case "aces":
                        return true;
                    case "acefruit":
                        return true;
                    case "amaticmg":
                        return true;
                    case "ampoker2":
                        return true;
                    case "atrornic":
                        return true;
                    case "astrafr":
                        return true;
                    case "bingoman":
                        return true;
                    case "calomega":
                        return true;
                    case "clpoker":
                        return true;
                    case "coinmstr":
                        return true;
                    case "gamtor":
                        return true;
                    case "gambl186":
                        return true;
                    case "gei":
                        return true;
                    case "goldnpkr":
                        return true;
                    case "hitpoker":
                        return true;
                    case "ltcasino":
                        return true;
                    case "majorpkr":
                        return true;
                    case "mgavegas":
                        return true;
                    case "murogmbl":
                        return true;

                }
                if (
                    strMachine.StartsWith("ecoinf") ||
                    strMachine.StartsWith("itgambl") ||
                    strMachine.StartsWith("jack") ||
                    strMachine.StartsWith("multfish") ||
                    strMachine.StartsWith("norautp") ||
                    strMachine.Contains("poker") ||
                    strMachine.StartsWith("proconn") ||
                    strMachine.StartsWith("sfbonus") ||
                    strMachine.StartsWith("vroulet") ||
                    strMachine.StartsWith("wildpkr")
                    )
                    return true;

                return false;
            }

            // igs: spoker
            if (strConstruct.Equals("igs") && strMachine.Equals("spoker"))
                return true;

            // ussr special gambling                
            if (strConstruct.Equals("ussr") && strMachine.Equals("special_gambl"))
                return true;

            return false;


        }

    }
}
