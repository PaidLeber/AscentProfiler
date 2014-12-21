using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace AscentProfiler
{
        class SequenceLoader
        {
                TriggerFactory triggerFactory = new TriggerFactory();
                ActionFactory actionFactory = new ActionFactory();
                Dictionary<string, string> sequences = new Dictionary<string, string>();
                List<string> directory;


                int triggerIndex = 0;

                internal SequenceLoader(string path)
                {
                        directory = new List<string>(Directory.GetDirectories(path));

                        foreach (string name in directory)
                        {
                                var i = directory.FindIndex(x => x == name);
                                directory[i] = Path.GetFileName(name);
                        }

                        directory.Sort();

                        List<string> files = new List<string>(Directory.GetFiles(path, "*" + AscentProfiler.sequenceExt));
                        
                        files.Sort();
                        
                        foreach (string file in files)
                        {
                                sequences.Add(Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllText(file));
                        }
                        

                }

                internal Dictionary<string, string> GetSequences()
                {
                        return sequences;
                }

                internal List<string> GetDirectoryNames()
                {
                        return directory;
                }

                internal List<string> GetFileNames(string path)
                {
                        List<string> files = new List<string>(Directory.GetFiles(path, "*" + AscentProfiler.sequenceExt));

                        foreach (string file in files)
                        {
                                var i = files.FindIndex(x => x == file);
                                files[i] = Path.GetFileNameWithoutExtension(file);
                        }

                        files.Sort();
                        return files;

                }

                internal Dictionary<string, string> GetFileContents(string path)
                {
                        List<string> files = new List<string>(Directory.GetFiles(AscentProfiler.sequenceDir, "*" + AscentProfiler.sequenceExt));

                        Dictionary<string, string> sequences = new Dictionary<string, string>();

                        foreach (string file in files)
                        {
                                sequences.Add(Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllText(file));
                        }

                        return sequences;
                }




                internal bool LoadSequence(SequenceEngine module, string sequence)
                {


                        List<string> sequenceLines = new List<string>(sequences[sequence].ToUpper().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                                //.Split(new string[] { "\n", "\r" }, StringSplitOptions.None));

                        Log.Level(LogType.Info, "Loading Sequence: " + sequence);
                        Log.Level(LogType.Verbose, sequences[sequence].ToUpper());

                        int sequenceStart = 0;
                        int sequenceEnd = 0;
                        int lineCounter = 0;
                        foreach (string line in sequenceLines)
                        {
                                lineCounter++;
                                if (Regex.IsMatch(line, @"^START\s*$"))
                                {
                                        Log.Level(LogType.Verbose, "GSCRIPT START: #" + line);
                                        
                                        sequenceStart = lineCounter;
                                }

                                if (Regex.IsMatch(line, @"^END\s*$"))
                                {
                                        Log.Level(LogType.Verbose, "GSCRIPT END: #" + line);
                                        
                                        sequenceEnd = lineCounter;
                                }
                        }

                        if(sequenceStart > sequenceEnd)
                        {
                                sequenceLines.Reverse();
                        }
                        
                        if(sequenceStart == 0)
                        {
                                Log.Console("Load Error: " + sequence + AscentProfiler.sequenceExt + " START command not found.");
                                // Add error log checking here then return bool
                                return false;
                        }
                        
                        if (sequenceEnd == 0)
                        {
                                Log.Console("Load Error: " + sequence + AscentProfiler.sequenceExt + " END command not found.");
                                return false;
                        }

                        lineCounter = 0;
                        foreach (string line in sequenceLines)
                        {
                                
                                lineCounter++;
                                Debug.Log(lineCounter+" line: "+line);
                                foreach (TriggerType trigger in (TriggerType[])Enum.GetValues(typeof(TriggerType)))
                                {
                                        if ( IsRegexCommandMatch(line, trigger.ToString()) )
                                        {
                                                Log.Level(LogType.Verbose, "Creating trigger line #" + lineCounter + ": " + line);

                                                int currentindex = triggerFactory.CreateTrigger(trigger, line, lineCounter);

                                                if (currentindex != -1)                                                         // return of -1 is a trigger switch of some sort, not an actual trigger
                                                {
                                                        triggerIndex = currentindex;
                                                }

                                        }   

                                }
                                
                                foreach (ActionType action in (ActionType[])Enum.GetValues(typeof(ActionType)))
                                {
                                        if (IsRegexCommandMatch(line, action.ToString()))
                                        {
                                                actionFactory.CreateAction(action, triggerIndex, triggerFactory.GetTabStackCount(), line, lineCounter);
                                        
                                        }


                                }
                                

                        }

                        if (HighLogic.LoadedScene == GameScenes.EDITOR)
                        {
                                if (module.Sequencer.sequenceBlock.Keys.Contains(sequence))
                                {
                                        Log.Console("Load Error: Sequence already loaded.");
                                        return false;
                                }

                                module.Sequencer.sequenceBlock.Add(sequence, triggerFactory.GetNewSequence(actionFactory.GetNewActionList()));
                                return true;
 

                        }
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                        {
                                

                        }
                        

                        return false;
                                

                }


                bool TXAscentProAEDLModule(string sequence, List<Command> newsequence)
                {
                        SequenceEngine AEDLmodule = AscentProfilerFlight.currentVessel.Parts.SelectMany(p => p.Modules.OfType<SequenceEngine>()).FirstOrDefault();
                       
                        TXRemoteTechNetwork(sequence, AEDLmodule);

                        return true;

                }

                void TXRemoteTechNetwork(string sequence, SequenceEngine module)
                {
                        System.Random rng = new System.Random();
                        int port = rng.Next(4000, 20000);
                        string vessel_ip = (AscentProfilerFlight.currentVessel.vesselName.ToLower() + "." + AscentProfilerFlight.currentVessel.vesselType.ToString().ToLower() + ".dsn").Replace(" ", "_");
                        
                        ScreenMessages.PostScreenMessage(new ScreenMessage("Transmitting sequence to " + AscentProfilerFlight.currentVessel.vesselType.ToString() + ". Please standby...", 3.0f, ScreenMessageStyle.UPPER_RIGHT));

                        //module.flightTelemetry.AddLog("Transmitting command sequence to " + AscentProfiler.currentVessel.vesselType.ToString() + ". Please standby...");
                        /*
                        module.telemetryController.AddLog("$nc -uv -w "+ Math.Ceiling(RemoteTech.API.GetSignalDelayToKSC(module.vessel.id)) + " " + vessel_ip + " " + port + " < " + sequence + ".seq");
                        
                        if (module.isConnectedtoKSC)
                        {
                                module.telemetryController.AddLog("nc: Connection to " + vessel_ip + " "+ port + " port [icp/*] succeeded!");
                        }
                        else
                        {
                                module.telemetryController.AddLog("nc: connect to " + vessel_ip + " " + port + " (icp) failed: Timeout (Check connection)");
                        }
                        */
                }















                bool IsRegexCommandMatch(string line, string commandType)
                {
                        return Regex.IsMatch(line, @"^\t*" + commandType + @"\s*.*$");

                }


        }
}
