using MyMameHelper.Container;
using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using MyMameHelper.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace MyMameHelper.Methods
{

    internal static class TableFeeder
    {

        static List<string> _SystemRoms = new List<string>
        {
            "devices/sound/",
            "devices/video/"
        };


        /// <summary>
        /// Tout n'a pas été totalement vérifié mais quasi sûr 
        /// </summary>
        static List<string> _Bellfruits = new List<string>()
        {
            "barcrest/mpu1.cpp",
            "barcrest/mpu4avan.cpp",
            "barcrest/mpu4bwb.cpp",
            "barcrest/mpu4crystal.cpp",
            "barcrest/mpu4empire.cpp",
            "barcrest/mpu4mod2sw.cpp",
            "barcrest/mpu4mod4oki.cpp",
            "barcrest/mpu4union.cpp",
            "barcrest/mpu4unsorted.cpp",
            "barcrest/mpu4vid.cpp",
            "barcrest/mpu5sw.cpp",
            "bfm/bfm_ad5sw.cpp",
            "bfm/bfm_blackbox.cpp",
            "bfm/bfm_sc1.cpp",
            "bfm/bfm_sc2.cpp",
            "bfm/bfm_sc4.cpp",
            "bfm/bfm_sc5sw.cpp",
            "bfm/bfm_swp.cpp",
            "bfm/bfmsys83.cpp",
            "bfm/bfmsys85.cpp",
            "cirsa/missbamby.cpp",
            "funworld/supercrd.cpp",
            "igs/goldstar.cpp",
            "igs/igs_m027.cpp",
            "igs/igs_m027xa.cpp",
            // all
            "jpm/jpmimpctsw.cpp",
            "jpm/jpmmps.cpp",
            "jpm/jpms80.cpp",
            "jpm/jpmsys5sw.cpp",
            "jpm/pluto5.cpp",
            "konami/konmedal68k.cpp",
            "maygay/maygay1bsw.cpp",
            "maygay/maygayep.cpp",
            "misc/39in1.cpp",
            "misc/amaticmg.cpp",
            "misc/astrafr.cpp",
            "misc/atronic.cpp",
            "misc/blitz68k.cpp",
            "misc/cromptons.cpp",
            "misc/dfruit.cpp",
            "misc/ecoinfr.cpp",
            "misc/fresh.cpp",
            "misc/hazelgr.cpp",
            "misc/jungleyo.cpp",
            "misc/mpu12wbk.cpp",
            "misc/multfish.cpp",
            "misc/multfish_boot.cpp",
            "misc/sfbonus.cpp",
            "nichibutsu/jangou.cpp",
            "pc/fruitpc.cpp",
            "playmark/sderby.cpp",
            "recfranco/rfslotsmcs48.cpp",
            "shared/fruitsamples.cpp",
            "shared/sec.cpp",
            "sigma/sigmab52.cpp",

        };



        static List<string> _Echec = new List<string>()
        {
            "chess/compuchess.cpp",
            "chess/conchess.cpp",
            "chess/conic_cchess2.cpp",
            "chess/conic_cchess3.cpp",
            "chess/tasc.cpp",
            "chessking/master.cpp",
            "commodore/chessmate.cpp",
            "cxg/chess2001.cpp",
            "cxg/computachess.cpp",
            "cxg/computachess2.cpp",
            "cxg/pchess.cpp",
            "cxg/professor.cpp",
            "ddr/chessmst.cpp",
            "ddr/chessmstdm.cpp",
            "devices/bus/c64/fcc.cpp",
            "devices/bus/centronics/chessmec.cpp",
            "devices/bus/chanf/rom.cpp",
            "devices/bus/isa/chessmdr.cpp",
            "devices/bus/isa/chessmsr.cpp",
            "devices/bus/isa/finalchs.cpp",
            "devices/bus/vc4000/rom.cpp",
            "devices/machine/chessmachine.cpp",
            "elektor/avrmax.cpp",
            "fidelity/cc1.cpp",
            "fidelity/cc10.cpp",
            "fidelity/cc7.cpp",
            "fidelity/chesster.cpp",
            "fidelity/csc.cpp",
            "fidelity/eldorado.cpp",
            "fidelity/elegance.cpp",
            "fidelity/msc.cpp",
            "fidelity/phantom.cpp",
            "fidelity/sc12.cpp",
            "fidelity/sc6.cpp",
            "fidelity/sc8.cpp",
            "fidelity/sc9.cpp",
            "fidelity/vcc.cpp",
            "fidelity/vsc.cpp",
            "handheld/chessking.cpp",
            "igs/goldstar.cpp",
            "igs/igs_m027.cpp",
            "mattel/chess.cpp",
            "novag/cnchess.cpp",
            "novag/micro.cpp",
            "novag/robotadv.cpp",
            "saitek/chessac.cpp",
            "saitek/chesstrv.cpp",
            "saitek/companion.cpp",
            "saitek/companion2.cpp",
            "saitek/cp2000.cpp",
            "saitek/delta1.cpp",
            "saitek/electrio.cpp",
            "saitek/exechess.cpp",
            "saitek/intchess.cpp",
            "saitek/mark5.cpp",
            "saitek/minichess.cpp",
            "saitek/prschess.cpp",
            "saitek/schess.cpp",
            "saitek/simultano.cpp",
            "saitek/ssystem3.cpp",
            "saitek/tschess.cpp",
            "saitek/turbo16k.cpp",
            "tryom/chess.cpp",

        };

        /*static List<string> _GameSystems = new List<string>()
        {
            "capcom/cps1.cpp",
            "capcom/cps2.cpp"
        };*/

        [Obsolete]
        /// <summary>
        /// non vérifié
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






        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Bellfruit, Super Fruit
        /// </remarks>
        [Obsolete]
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
        [Obsolete]
        public static List<string> MoneyConst => _MoneyConst;

        [Obsolete]
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



        private static string WriteCategory(string category, ref string prevConstructor, ref string strConstruct)
        {
            string line = "";
            if (!prevConstructor.Equals(strConstruct))
            {
                line = $"============== {strConstruct} - {category}  ==============";
                prevConstructor = strConstruct;
            }


            return line;
        }

        /*
        private static void mee()
        {
            CT_Machine machine = new CT_Machine();  
            // On ajoute le nom du constructeur pour tous les autres cas // (sauf pinball)
            if (onlyConstructs.FirstOrDefault(x => x.Nom.Equals(strConstruct)) == null)
            {
                machine.ID = otherID;
                machine.Nom = strConstruct;

                //if (!strConstruct.Equals("pinball"))
                onlyConstructs.Add(machine);

                otherID++;
            }
        }*/

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
            uint systemID = 1000;
            uint otherID = 2000;
            //  uint keepID = 1000;

            // Systèmes connus comme CPS1, Naomi
            List<CT_Machine> knownSystem = new List<CT_Machine>();
            // Casino, Bellfruits...
            List<CT_Machine> moneyMachine = new List<CT_Machine>();
            // Rom pour le son par exemple
            List<CT_Machine> systemRom = new List<CT_Machine>();
            // Les autres on garde juste le constructeur
            List<CT_Machine> onlyConstructs = new List<CT_Machine>();

            // Squelettes - roms non fonctionnelles
            List<string> skelettons = new List<string>();
            List<string> machinesASous = new List<string>();
            List<string> SystemRoms = new List<string>();
            List<string> chessRoms = new List<string>();



            List<string> pinball = new List<string>();
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




                Debug.Write($"Traitement de {srcFile}");




                // Extension du sourceFile
                string extension = srcFile.Substring(srcFile.LastIndexOf('.') + 1);

                // Machine
                string strMachine = srcFile.Substring(srcFile.IndexOf('/') + 1);
                strMachine = strMachine.Substring(0, strMachine.Length - extension.Length - 1);


                string strConstruct = srcFile.Substring(0, srcFile.IndexOf("/"));
                //Debug.WriteLine(strConstruct);


                CT_Machine machine = new CT_Machine()
                {
                    //Nom = $"{strConstruct} - {strMachine}"
                    Nom = srcFile
                };

                // Subdivision
                string rLine = $"{strConstruct} - {strMachine} ({occur})";



                if (isdevice)
                {
                    // Cas de devices utilisés par des roms
                    if (_SystemRoms.FirstOrDefault(x => srcFile.StartsWith(x)) != null)
                    {
                        SystemRoms.Add(WriteCategory("SystemRoms", ref prevConstructor, ref strConstruct));
                        SystemRoms.Add(rLine);

                        machine.ID = systemID;
                        machine.Category = "SystemRoms";
                        systemRom.Add(machine);

                        systemID++;
                        Debug.WriteLine($" - Keep : type device");
                        continue;
                    }

                    Debug.WriteLine($" - Pass : type device");
                    continue;
                }


                // Squelettes - roms non fonctionnelles
                if (strConstruct.StartsWith("skeleton"))
                {
                    skelettons.Add(WriteCategory("Skeleton", ref prevConstructor, ref strConstruct));
                    skelettons.Add(rLine);

                    continue;

                }

                // Bellfruits - Machines à sous 
                if (_Bellfruits.FirstOrDefault(x => x.Equals(srcFile)) != null)
                {
                    machinesASous.Add(WriteCategory("Bellfruits", ref prevConstructor, ref strConstruct));
                    machinesASous.Add(rLine);


                    machine.Category = "Bellfruits";
                    moneyMachine.Add(machine);

                    continue;
                }

                // Jeux d'échec
                if (_Echec.FirstOrDefault(x => x.Equals(srcFile)) != null)
                {
                    chessRoms.Add(WriteCategory("Chess", ref prevConstructor, ref strConstruct));
                    chessRoms.Add(rLine);


                    machine.Category = "Chess";
                    knownSystem.Add(machine);

                    continue;
                }


                // Game Systems
                var res = IsKnowedSystem(strConstruct, ref machine, strMachine, srcFile);
                if (res > 0)
                {
                    chessRoms.Add(WriteCategory("Game Systems", ref prevConstructor, ref strConstruct));
                    chessRoms.Add(rLine);

                    knownSystem.Add(machine);

                    Debug.WriteLine($" - Keep : type Game System");
                    continue;
                }
                else if (res == 0)
                {

                    knownSystem.Add(machine);
                }

                Debug.WriteLine("");
                continue;


                // knowed
                if (res == 1)
                {
                    knownSystem.Add(machine);

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


            return new Dictionary<string, List<CT_Machine>>() {
                { "identified", knownSystem } ,
                { "money", moneyMachine } ,
                { "Constructeurs", onlyConstructs },
                { "SystemRoms",systemRom }
            };
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


        [Obsolete]
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
        /*

                    // Amiga: 1
            if (strConstruct.Equals("amiga"))
            {
                machine.IDConstructeur = 1;
                if (strMachine.StartsWith("amiga") || strMachine.StartsWith("arsystems"))
                {
                    return 1;
                }
                return 0;
            }*/

        public static List<Cont_Constructeur> _KnownSystems = new List<Cont_Constructeur>()
        {
            // Amiga: 1
            new Cont_Constructeur(1, "Amiga")
            {

            },
            // Capcom : 5
            new Cont_Constructeur(5, "Capcom")
            {
                Machines = new List<Cont_Machine>()
                {
                    new Cont_Machine("capcom/cps1.cpp")
                    {
                        Year=1988,
                        FirstVersion=1988,
                        Category="Capcom - CPS1"

                    },
                    new Cont_Machine("capcom/cps2.cpp")
                    {
                        Year=1993,
                        FirstVersion=1993,
                        Category="Capcom - CPS2"

                    },
                    new Cont_Machine("capcom/cps3.cpp")
                    {
                        Year=1996,
                        FirstVersion=1996,
                        Category="Capcom - CPS3"
                    }
                }
            },
            // Neogeo : 26
            new Cont_Constructeur(26, "NeoGeo")
            {
                Machines = new List<Cont_Machine>()
                {
                    new Cont_Machine("neogeo/neogeo.cpp")
                    {
                        //machine.IDConstructeur = 5;
                        Year=1990,
                        FirstVersion=1990,
                        Category="SNK - NeoGeo"

                    },
                    new Cont_Machine("neogeo/neopcb.cpp")
                    {
                        //machine.IDConstructeur = 5;
                        Year = 2003,
                        FirstVersion = 2003,
                        Category = "SNK - NeoPCB"

                    },
                }
            },
            // Sega : 29
            new Cont_Constructeur(29, "Sega")
            {
                Machines = new List<Cont_Machine>()
                {
                    // System 1
                    new Cont_Machine("sega/segas1")
                    {
                        Year=1983,
                        FirstVersion=19836,
                        Category="Sega - System 1"
                    },
                    // System 16
                    new Cont_Machine("sega/segas16a", "sega/segas16b","sega/system16.cpp" ,"sega/segas16b_isgsm.cpp")
                    {
                        Year=1986,
                        FirstVersion=1986,
                        Category="Sega - System 16"
                    },
                    new Cont_Machine("sega/segas16b_isgsm.cpp")
                    {
                        Year=1986,
                        FirstVersion=1986,
                        Category="Sega - System 16"
                    },
                    // System 18
                    new Cont_Machine("sega/segas18.cpp")
                    {
                        Year=1989,
                        FirstVersion=1989,
                        Category="Sega - System 18"
                    },
                }
            }
        };





        /// <summary>
        /// Système sélectionnés par mes soins
        /// </summary>
        /// <param name="strConstruct"></param>
        /// <param name="machine"></param>
        /// <param name="strMachine"></param>
        /// <returns></returns>
        private static short IsKnowedSystem(string strConstruct, ref CT_Machine machine, string strMachine, string machineName)
        {
            if (machineName.Equals("neogeo/midas.cpp"))
            {
                machine.Category = "Andamiro - Clone NeoGeo";
                return 0;
            }



            var mapConst = _KnownSystems.FirstOrDefault(x => x.Constructeur.ToUpper().Equals(strConstruct.ToUpper()));

            if (mapConst != null)
            {

                CT_Machine machineFound = null;


                for (int i = 0; i < mapConst.Machines.Count; i++)
                {
                    machineFound = mapConst.Machines[i].Get_Machine(machineName);
                    if (machineFound != null)
                        return 1;
                }



                /*
                if (machineFound != null)
                {
                    machine = machineFound;
                    return 1;
                }

                machineFound = mapConst.Machines.FirstOrDefault(x => x.Nom.StartsWith($"{strConstruct}/{strMachine}"));
                if (machineFound != null)
                {
                    machineFound.Nom = machineName;
                    machine = machineFound;
                    return 1;
                }

                machine.Category = strConstruct;*/
                return 0;

            }

            return -1;

            //Totalement revoir
            /*new CT_Machine("neogeo/midas.cpp")
            {
                //machine.IDConstructeur = 5;
                Year = 1990,
                FirstVersion = 1990,
                Category = "SNK - Midas"

            },
                  */



            // Amstrad: 2
            if (strConstruct.Equals("amstrad"))
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
                machine.Category = $"Sega - {strMachine.ToUpper()}";

                if (strMachine.StartsWith("segas16") || strMachine.StartsWith("system16"))
                {
                    machine.Category = $"Sega - System 16";
                    machine.Year = 1986;
                    return 1;
                }
                else if (strMachine.StartsWith("segas18"))
                {
                    machine.Category = $"Sega - System 18";
                    machine.Year = 1989;
                    return 1;
                }
                else if (
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

                    strMachine.StartsWith("segas24") ||
                    strMachine.StartsWith("segas32") ||
                    strMachine.StartsWith("segasp") ||
                    strMachine.StartsWith("segaxbd") ||
                    strMachine.StartsWith("sg1000a") ||
                    strMachine.StartsWith("stv") ||         // Titans
                    strMachine.StartsWith("triforce") ||    // Gamecube en collaboration
                    strMachine.StartsWith("turbo") ||
                    strMachine.StartsWith("vicdual") ||

                    strMachine.StartsWith("system1")

                    )
                {
                    return 1;
                }

                machine.Category = null;
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
                machine.Category = $"SNK - {strMachine.ToUpper()}";

                if (strMachine.StartsWith("snk6502"))
                {
                    machine.Year = 1980;
                    return 1;
                }
                else if (strMachine.StartsWith("snk"))
                {
                    machine.Year = 1983;
                    return 1;
                }
                else if (strMachine.StartsWith("snk68"))
                {
                    machine.Year = 1986;
                    return 1;
                }
                else if (strMachine.StartsWith("hng64"))
                {
                    machine.Year = 1996;
                    return 1;
                }

                machine.Category = null;
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
