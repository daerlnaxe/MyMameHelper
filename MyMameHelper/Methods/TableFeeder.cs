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
                strMachine = strMachine.Substring(0, strMachine.Length - extension.Length);



                string strConstruct = srcFile.Substring(0, srcFile.IndexOf("/"));



                CT_Machine machine = new CT_Machine()
                {
                    Nom = strMachine

                };

                // Atari: 1
                if (strConstruct.Equals("atari"))
                {
                    machine.IDConstructeur = 1;
                    machines.Add(machine);
                }
                // Capcom : 2
                else if (strConstruct.Equals("capcom"))
                {
                    machine.IDConstructeur = 2;
                    machines.Add(machine);
                }
                // Data East : 3
                else if (strConstruct.Equals("data east"))
                {
                    machine.IDConstructeur = 3;

                    machines.Add(machine);
                }
                // Konami : 4
                else if (strConstruct.Equals("konami"))
                {
                    machine.IDConstructeur = 4;

                    machines.Add(machine);
                }
                // Irem : 5
                else if (strConstruct.Equals("irem"))
                {
                    machine.IDConstructeur = 5;

                    machines.Add(machine);
                }
                // Midway : 6
                else if (strConstruct.Equals("midway"))
                {
                    machine.IDConstructeur = 6;

                    machines.Add(machine);
                }
                // Namco : 7
                else if (strConstruct.Equals("namco"))
                {
                    machine.IDConstructeur = 7;

                    machines.Add(machine);
                }
                // Sega : 8
                else if (strConstruct.Equals("sega"))
                {
                    machine.IDConstructeur = 8;

                    /*if (
                        strMachine.StartsWith("system1") ||
                        strMachine.StartsWith("system16") ||
                        strMachine.StartsWith("model1") ||
                        strMachine.StartsWith("model2") ||
                        strMachine.StartsWith("model3") ||
                        strMachine.StartsWith("naomi") ||
                        strMachine.StartsWith("naomi") ||
                        strMachine.StartsWith("saturn") ||
                        strMachine.StartsWith("segag80")
                        )
                    {*/
                    machines.Add(machine);
                    /*}
                    else
                    {
                        Debug.WriteLine($"Pass: sega {machine.Nom}");
                        machine = null;
                    }*/




                }
                else
                {
                    Debug.WriteLine($"Pass {srcFile}");
                    if (occur > 10)
                        Debug.WriteLine($"Occurences: {srcFile}: {occur}");

                }

            }


            //Ajout à la base
            using (SQLite_Op sqOP = new SQLite_Op())
            {
                for (int i = 0; i < machines.Count; i++)
                {
                    CT_Machine machine = machines[i];
                    sqOP.Insert_Machine(machine);


                }
            }

            //return machine;
        }



    }
}
