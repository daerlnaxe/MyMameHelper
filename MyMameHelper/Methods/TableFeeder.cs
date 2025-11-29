using MyMameHelper.ContTable;
using MyMameHelper.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            using (System.IO.StreamWriter f = new System.IO.StreamWriter("passed.log"))
            {
                /*for (int i = 0; i < notAccepted.Count; i++)
                    f.WriteLine(notAccepted[i]);
                */
                string prevConstructor = "";
                uint otherID = 1000;
                uint keepID = 1000;

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
                        Nom = strMachine
                    };

                    //KeyValuePair<string, List<string>> foo = notAccepted.FirstOrDefault(x => x.Key.Equals(strConstruct));
               

                    // Atari: 1
                    if (strConstruct.Equals("atari"))
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
                    // Capcom : 2
                    else if (strConstruct.Equals("capcom"))
                    {
                        machine.IDConstructeur = 2;

                    }
                    // Data East : 3
                    else if (strConstruct.Equals("data east"))
                    {
                        machine.IDConstructeur = 3;
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
                            strMachine.StartsWith("saturn") ||
                            strMachine.StartsWith("segag80")
                            )
                        {
                            machines.Add(machine);
                            continue;
                        }
                        else
                        {

                            // notAccepted.Add(srcFile);
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


                    if (!prevConstructor.Equals(strConstruct))
                    {
                        f.WriteLine($"============== {strConstruct} ==============");
                        prevConstructor = strConstruct;
                    }


                    //notAccepted.Add(srcFile);
                    f.WriteLine($"Pass: {strConstruct} {machine.Nom} ({occur})");

                    machine = null;
                    if (occur > 10)
                        f.WriteLine($">>> Occurences: {srcFile}: {occur}");



                }
            }


            //Ajout à la base
            using (SQLite_OP sqOP = new SQLite_OP())
            {
                for (int i = 0; i < machines.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machines[i].Nom}");
                    CT_Machine machine = machines[i];
                    sqOP.Insert_Machine(machine, false,false);


                }
                for (int i = 0; i < machinesSimple.Count; i++)
                {
                    Debug.WriteLine($"insert {i}: {machinesSimple[i].ID} {machinesSimple[i].Nom}");
                    CT_Machine machine = machines[i];
                    sqOP.Insert_Machine(machine, false, true);


                }
            }




            //return machine;



        }

    }
}
