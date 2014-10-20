using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace AscentProfiler
{
        class ProfileLoader
        {
                TriggerFactory triggerFactory = new TriggerFactory();
                Dictionary<string, string> profiles = new Dictionary<string, string>();


                int triggerIndex = -1;

                public ProfileLoader()
                {
                        List<string> files = new List<string>(Directory.GetFiles(AscentProfiler.profilesDir, "*.profile"));

                        foreach (string file in files)
                        {
                                profiles.Add(Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllText(file));
                        }
                }

                public Dictionary<string, string> GetProfiles()
                {
                        return profiles;
                }

                public string LoadProfile(string profile)
                {
                        int profileStart = 0;
                        int profileEnd = 0;

                        AscentProfiler.ActiveProfile.Reset();

                        List<string> profileLines = new List<string>(profiles[profile].ToUpper().Split(new string[] { "\n", "\r" }, StringSplitOptions.None));

                        Log.Level(LogType.Info, "Loading Profile: " + profile);
                        Log.Level(LogType.Verbose, profiles[profile].ToUpper());

                        int lineCounter = 0;
                        foreach (string line in profileLines)
                        {
                                //Debug.Log("regex start and stop");
                                lineCounter++;
                                if (Regex.IsMatch(line, triggerFactory.regexDict["START"]))
                                {
                                        Log.Level(LogType.Verbose, "GSCRIPT START: #" + line);
                                        profileStart = lineCounter;
                                }

                                if (Regex.IsMatch(line, triggerFactory.regexDict["END"]))
                                {
                                        Log.Level(LogType.Verbose, "GSCRIPT END: #" + line);
                                        profileEnd = lineCounter;
                                }
                        }

                        if(profileStart > profileEnd)
                        {
                                profileLines.Reverse();
                        }
                        else if(profileStart == 0)
                        {
                                Log.Script(LogType.Error, profile, "START not found.");
                                // Add error log checking here then return bool
                                //return false;
                        }
                        else if (profileEnd == 0)
                        {
                                Log.Script(LogType.Error, profile, "END not found.");
                        }

                        lineCounter = 0;
                        foreach (string line in profileLines)
                        {
                                
                                lineCounter++;
                                foreach (TriggerType trigger in (TriggerType[])Enum.GetValues(typeof(TriggerType)))
                                {
                                        //Debug.Log("start trigger check: " + trigger.ToString());
                                        if (Regex.IsMatch(line, triggerFactory.regexDict["CMDBEGIN"] + trigger.ToString() + triggerFactory.regexDict["CMDEND"] ))
                                        {

                                                Log.Level(LogType.Verbose, "Creating trigger line #" + lineCounter + ": " + line);

                                                int indexer = triggerFactory.CreateTrigger(trigger, line, lineCounter);

                                                if (indexer != -1) // return of -1 is a trigger switch of some sort, not an actual trigger
                                                {
                                                        triggerIndex = indexer;
                                                }

                                        }

                                        
                                }


                        }


                        return string.Join("\n", profileLines.ToArray());

                }



        }
}
