﻿using System;
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
                ActionFactory actionFactory = new ActionFactory();
                Dictionary<string, string> profiles = new Dictionary<string, string>();


                int triggerIndex = 0;

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
                                lineCounter++;
                                if (Regex.IsMatch(line, @"^START\s*$"))
                                {
                                        Log.Level(LogType.Verbose, "GSCRIPT START: #" + line);
                                        profileStart = lineCounter;
                                }

                                if (Regex.IsMatch(line, @"^END\s*$"))
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
                                        if ( IsRegexCommandMatch(line, trigger.ToString()) )
                                        {
                                                Log.Level(LogType.Verbose, "Creating trigger line #" + lineCounter + ": " + line);

                                                int currentindex = triggerFactory.CreateTrigger(trigger, line, lineCounter);

                                                if (currentindex != -1) // return of -1 is a trigger switch of some sort, not an actual trigger
                                                {
                                                        triggerIndex = currentindex;
                                                }

                                        }   

                                }

                                foreach (ActionType action in (ActionType[])Enum.GetValues(typeof(ActionType)))
                                {
                                        if (IsRegexCommandMatch(line, action.ToString()))
                                        {
                                                actionFactory.CreateAction(action, line, lineCounter);


                                        }


                                }


                        }


                        return string.Join("\n", profileLines.ToArray());

                }

                bool IsRegexCommandMatch(string line, string commandType)
                {
                        return Regex.IsMatch(line, @"^\t*" + commandType + @"\s*.*$");

                }


        }
}
