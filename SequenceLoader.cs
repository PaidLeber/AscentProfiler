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


                int triggerIndex = 0;

                internal SequenceLoader()
                {
                        List<string> files = new List<string>(Directory.GetFiles(AscentProfiler.profilesDir, "*.seq"));

                        foreach (string file in files)
                        {
                                sequences.Add(Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllText(file));
                        }
                }

                internal Dictionary<string, string> GetProfiles()
                {
                        return sequences;
                }

                internal bool LoadSequence(string sequence)
                {
                        int sequenceStart = 0;
                        int sequenceEnd = 0;

                        List<string> sequenceLines = new List<string>(sequences[sequence].ToUpper().Split(new string[] { "\n", "\r" }, StringSplitOptions.None));

                        Log.Level(LogType.Info, "Loading Profile: " + sequence);
                        Log.Level(LogType.Verbose, sequences[sequence].ToUpper());

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
                        else if(sequenceStart == 0)
                        {
                                Log.Script(LogType.Error, sequence + " START not found.");
                                // Add error log checking here then return bool
                                //return false;
                        }
                        else if (sequenceEnd == 0)
                        {
                                Log.Script(LogType.Error, sequence+ " END not found.");
                        }

                        lineCounter = 0;
                        foreach (string line in sequenceLines)
                        {
                                
                                lineCounter++;

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



                        return TXAscentProAPGCSModule(sequence, triggerFactory.GetNewFlightSequence(actionFactory.GetNewActionList()));

                }
                

                bool TXAscentProAPGCSModule(string sequence, SequenceEngine newsequence)
                {
                        AscentProAPGCSModule APGCSmodule = AscentProfiler.currentVessel.Parts.SelectMany(p => p.Modules.OfType<AscentProAPGCSModule>()).FirstOrDefault();

                        TXRemoteTechNetwork(sequence, APGCSmodule);
                        
                        return APGCSmodule.RXNewSequence(newsequence);

                }

                void TXRemoteTechNetwork(string sequence, AscentProAPGCSModule module)
                {
                        System.Random rng = new System.Random();
                        int port = rng.Next(4000, 20000);
                        string vessel_ip = (AscentProfiler.currentVessel.vesselName.ToLower() + "." + AscentProfiler.currentVessel.vesselType.ToString().ToLower() + ".dsn").Replace(" ", "_");
                        
                        ScreenMessages.PostScreenMessage(new ScreenMessage("Transmitting sequence to " + AscentProfiler.currentVessel.vesselType.ToString() + ". Please standby...", 3.0f, ScreenMessageStyle.UPPER_RIGHT));

                        //module.flightTelemetry.AddLog("Transmitting command sequence to " + AscentProfiler.currentVessel.vesselType.ToString() + ". Please standby...");
                        module.flightTelemetry.AddLog("$nc -uv -w "+ Math.Ceiling(RemoteTech.API.GetSignalDelayToKSC(module.vessel.id)) + " " + vessel_ip + " " + port + " < " + sequence + ".seq");
                        
                        if (module.isConnectedtoKSC)
                        {
                                module.flightTelemetry.AddLog("nc: Connection to " + vessel_ip + " "+ port + " port [icp/*] succeeded!");
                        }
                        else
                        {
                                module.flightTelemetry.AddLog("nc: connect to " + vessel_ip + " " + port + " (icp) failed: Timeout (Check connection)");
                        }

                }

                bool IsRegexCommandMatch(string line, string commandType)
                {
                        return Regex.IsMatch(line, @"^\t*" + commandType + @"\s*.*$");

                }


        }
}
