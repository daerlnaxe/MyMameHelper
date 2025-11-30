using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace MyMameHelper.Methods
{

    internal static class TableFeeder
    {
        /// <summary>
        /// Construit les machines en fonction de source_file des roms temporaires de M.A.M.E
        /// </summary>
        internal static void Machine(Dictionary<string, object[]> sourceFiles)
        {
            //var machine = new List<CT_Machine>();
            List<CT_Machine> machines = new List<CT_Machine>();
            List<CT_Machine> machinesSimple = new List<CT_Machine>();
            Dictionary<string, List<string>> notAccepted = new Dictionary<string, List<string>>();


            /*for (int i = 0; i < notAccepted.Count; i++)
                f.WriteLine(notAccepted[i]);
            */
            string prevConstructor = "";
            uint otherID = 1000;
            uint keepID = 1000;

            List<string> skelettons = new List<string>();
            List<string> machinesASous = new List<string>();
            List<string> materielElectronique = new List<string>();
            List<string> pass = new List<string>();


            foreach (var kvp in sourceFiles)
            {
                var srcFile = kvp.Key;
                bool isdevice = (bool)kvp.Value[0];
                int occur = Convert.ToUInt16(kvp.Value[1]);


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



                CT_Machine machine = new CT_Machine()
                {
                    Nom = $"{strConstruct} - {strMachine}"
                };

                //KeyValuePair<string, List<string>> foo = notAccepted.FirstOrDefault(x => x.Key.Equals(strConstruct));

                // Amiga: 9
                if (strConstruct.Equals("amiga"))
                {
                    machine.IDConstructeur = 9;
                    if (
                        strMachine.StartsWith("amiga") ||
                        strMachine.StartsWith("arsystems")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Amstrad: 10
                else if (strConstruct.Equals("amstrad"))
                {
                    machine.IDConstructeur = 10;
                    if (
                        strMachine.StartsWith("amstrad")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Atari: 1
                else if (strConstruct.Equals("atari"))
                {

                    machine.IDConstructeur = 1;

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
                        machines.Add(machine);
                        continue;
                    }

                }
                // Atlus: 11
                else if (strConstruct.Equals("atlus"))
                {
                    machine.IDConstructeur = 11;
                    if (
                        strMachine.StartsWith("cave")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Capcom : 2
                else if (strConstruct.Equals("capcom"))
                {
                    machine.IDConstructeur = 2;

                    if (
                        strMachine.StartsWith("cps1") ||
                        strMachine.StartsWith("cps2") ||
                        strMachine.StartsWith("cps3")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }

                }
                // Cave: 12
                else if (strConstruct.Equals("cave"))
                {
                    machine.IDConstructeur = 11;
                    if (
                        strMachine.StartsWith("cv1k")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }/*                    
                    // Cinematronics: 
                    else if (strConstruct.Equals("cinematronics"))
                    {
                        machine.IDConstructeur = ;
                        if (
                            strMachine.StartsWith("cv1k")
                            )
                        {
                            machines.Add(machine);
                            continue;
                        }
                    }*/
                // Data East : 3
                else if (strConstruct.Equals("data east"))
                {
                    machine.IDConstructeur = 3;

                    if (
                        strMachine.StartsWith("dec0") ||
                        strMachine.StartsWith("dec8") ||
                        strMachine.StartsWith("deco32") ||
                        strMachine.StartsWith("deco_mlc") ||
                        strMachine.StartsWith("simpl156") ||
                        strMachine.StartsWith("decocass")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }

                }
                // Dooyong: 13
                else if (strConstruct.Equals("dooyong"))
                {
                    machine.IDConstructeur = 13;
                    if (
                        strMachine.StartsWith("dooyong")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Emusy: 14
                else if (strConstruct.Equals("emusys"))
                {
                    machine.IDConstructeur = 14;
                    if (
                        strMachine.StartsWith("emu2") ||
                        strMachine.StartsWith("emu3") ||
                        strMachine.StartsWith("emu68k")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Eolith: 15
                else if (strConstruct.Equals("eolith"))
                {
                    machine.IDConstructeur = 15;
                    if (
                        strMachine.StartsWith("eolith") /*||
                            strMachine.StartsWith("emu3") ||
                            strMachine.StartsWith("emu68k")*/
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Exidy: 16
                else if (strConstruct.Equals("exidy"))
                {
                    machine.IDConstructeur = 16;
                    if (
                        strMachine.StartsWith("exidy") ||
                        strMachine.StartsWith("exidy440") /*||
                            strMachine.StartsWith("emu68k")*/
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Gaelco: 17
                else if (strConstruct.Equals("gaelco"))
                {
                    machine.IDConstructeur = 17;
                    if (
                        strMachine.StartsWith("gaelco") ||
                        strMachine.StartsWith("gaelco2") ||
                        strMachine.StartsWith("gaelco3d")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Gottlieb: 18
                else if (strConstruct.Equals("gottlieb"))
                {
                    machine.IDConstructeur = 18;
                    if (
                        strMachine.StartsWith("gottlieb")/* ||
                            strMachine.StartsWith("exidy440") /*||
                            strMachine.StartsWith("emu68k")*/
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Gottlieb: 18
                else if (strConstruct.Equals("handheld"))
                {
                    machine.IDConstructeur = 18;
                    if (
                        strMachine.StartsWith("hh_sm510") ||
                        strMachine.StartsWith("hh_tms1k") ||
                        strMachine.StartsWith("hh_hmcs40")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }
                }
                // Konami : 4
                else if (strConstruct.Equals("konami"))
                {
                    machine.IDConstructeur = 4;

                }
                // Irem : 5
                else if (strConstruct.Equals("irem"))
                {
                    machine.IDConstructeur = 5;


                }
                // Midway : 6
                else if (strConstruct.Equals("midway"))
                {
                    machine.IDConstructeur = 6;


                }
                // Namco : 7
                else if (strConstruct.Equals("namco"))
                {
                    machine.IDConstructeur = 7;

                }
                // Sega : 8
                else if (strConstruct.Equals("sega"))
                {
                    machine.IDConstructeur = 8;

                    if (
                        strMachine.StartsWith("system1") ||
                        strMachine.StartsWith("system16") ||
                        strMachine.StartsWith("model1") ||
                        strMachine.StartsWith("model2") ||
                        strMachine.StartsWith("model3") ||
                        strMachine.StartsWith("naomi") ||
                        strMachine.StartsWith("naomi2") ||
                        //strMachine.StartsWith("saturn") ||    // on dirait que c'est un bios
                        strMachine.StartsWith("segag80")
                        )
                    {
                        machines.Add(machine);
                        continue;
                    }

                }

                // On ajoute juste le nom du constructeur ça peut servir
                else
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        machine.ID = otherID;
                        machine.Nom = strConstruct;
                        machinesSimple.Add(machine);
                        otherID++;
                    }
                }



                // Subdivision
                string rLine = $"{strConstruct} - {strMachine} ({occur})";

                if (strConstruct.StartsWith("skeleton"))
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        skelettons.Add($"============== {strConstruct} - Squelettes, pas une machine  ==============");
                    }
                    skelettons.Add(rLine);
                }
                else if (
                    strConstruct.Equals("aristocrat")|| //&& strMachine.Equals("aristmk5") ) ||
                    strConstruct.Equals("astrocorp") ||
                    strConstruct.Equals("barcrest") ||
                    strConstruct.Equals("bfm") ||
                    strConstruct.Equals("bmc") ||
                    strConstruct.Equals("dynax") ||
                    strConstruct.Equals("excellent") ||
                    strConstruct.Equals("falco") ||
                    strConstruct.Equals("gridcomp")


                    )
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        machinesASous.Add($"============== {strConstruct} - Machines à sous  ==============");
                    }
                    machinesASous.Add(rLine);


                }
                else if (
                    strConstruct.Equals("brother") ||
                    strConstruct.Equals("canon") ||
                    strConstruct.Equals("elektor") ||
                    strConstruct.Equals("epson") ||
                    strConstruct.Equals("ensoniq") ||
                    strConstruct.Equals("ericsson") ||
                    strConstruct.Equals("excalibur") ||
                    strConstruct.Equals("facid") ||
                    strConstruct.Equals("fairchild") ||
                    strConstruct.Equals("fairlight") //||
                    )
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        materielElectronique.Add($"============== {strConstruct} - Materiel Electronique ==============");
                    }
                    materielElectronique.Add(rLine);


                }
                else
                {
                    if (!prevConstructor.Equals(strConstruct))
                    {
                        pass.Add($"============== {strConstruct} ==============");
                    }
                    pass.Add(rLine);
                    if (occur > 10)
                        pass.Add($">>>>----------------------------------------- {occur} occurences");

                }

                prevConstructor = strConstruct;



                machine = null;
            }


            using (System.IO.StreamWriter f = new System.IO.StreamWriter("passed.log"))
            {
                f.WriteLine("************************ Pass ************************");
                foreach (string line in pass)
                    f.WriteLine(line);

                f.WriteLine("************************ Skelettons ************************");
                foreach (string line in skelettons)
                    f.WriteLine(line);

          
                f.WriteLine("************************ Materiel Electronique ************************");
                foreach (string line in materielElectronique)
                    f.WriteLine(line);


                f.WriteLine("************************ Machine à sous ************************");
                foreach (string line in machinesASous)
                    f.WriteLine(line);
            }

            //Ajout à la base
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                for (int i = 0; i < machines.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machines[i].Nom}");
                    CT_Machine machine = machines[i];
                    sqOP.Insert_Machine(machine, false, false);


                }
                for (int i = 0; i < machinesSimple.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machinesSimple[i].ID} {machinesSimple[i].Nom}");
                    CT_Machine machine = machinesSimple[i];
                    sqOP.Insert_Machine(machine, false, true);


                }
            }




            //return machine;



        }

    }
}
