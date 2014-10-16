using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace AscentProfiler
{
        public class ProfileLoader
        {
                private TriggerFactory triggerFactory = new TriggerFactory();
                private Dictionary<string, string> profiles = new Dictionary<string, string>();
                private List<string> activeProfile = new List<string>();

                private int triggerIndex = -1;

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

                        int lineCounter = 0;
                        foreach (string line in profileLines)
                        {
                                lineCounter++;
                                if (Regex.IsMatch(line, @"^START\s*.*"))
                                {
                                        profileStart = lineCounter;
                                }

                                if (Regex.IsMatch(line, @"^END\s*.*"))
                                {
                                        profileEnd = lineCounter;
                                }
                        }

                        if(profileStart > profileEnd)
                        {
                                profileLines.Reverse();
                        }
                        else if(profileStart == 0 || profileEnd == 0)
                        {
                                // Add error log checking here then return bool
                                //return false;
                        }

                        lineCounter = 0;
                        foreach (string line in profileLines)
                        {
                                lineCounter++;
                                foreach (TriggerType trigger in (TriggerType[])Enum.GetValues(typeof(TriggerType)))
                                {
                                        if (Regex.IsMatch(line, @"^\t*" + trigger.ToString() + @".*"))
                                        {
                                                Debug.Log("Creating Trigger line #"+ lineCounter +": "+line);
                                                triggerFactory.CreateTrigger(trigger, line, lineCounter);
                                        }
                                        
                                }


                        }


                        return string.Join("\n", profileLines.ToArray());


                        /*
	                foreach (string line in lines)
	                {
                                foreach (string trigger in triggers)
                                {
                                        if (line.ToUpper().Contains(trigger))
                                        { 
                                                
                                        }
                                        else if (line.ToUpper().Contains(trigger))
                                        {
                                        
                                        }
                                }
                                // change everything .ToUpper
                                // check .contains(ASCENT) for ascent/descent commmand (forget it, i'll send it to triggerfactory and flip the bit there!!!"
                                // when you send a trigger line, it needs to return an int index
	                }
                         */


                        

                        //return false;
                }

                private string RemoveEmptyLines(string lines)
                {
                        // Remove empty line with or without a newline/carriage return character 
                        return Regex.Replace(lines, @"(^\s*$\n|\r)|(^\s*$)", String.Empty, RegexOptions.Multiline);
                               
                }
                private string RemoveCommentLines(string lines)
                {
                        // Remove comment line with a newline/carriage return character or (|) no return character 
                        return Regex.Replace(lines, @"(^\s*#.*$n|\r)|(^\s*#.*$)", String.Empty, RegexOptions.Multiline).TrimEnd();
                }


        }
}
