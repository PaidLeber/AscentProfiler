using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AscentProfiler
{

        class TriggerFactory
        {
                List<Trigger> NewTriggerSequence = new List<Trigger>();

                internal Dictionary<string, string> regexDict = new Dictionary<string, string>();
                Dictionary<TriggerType, string> triggerRegex = new Dictionary<TriggerType, String>();
                Dictionary<TriggerType, Func<Trigger>> triggerProducts = new Dictionary<TriggerType, Func<Trigger>>();


                Stack<int> tabCountStack = new Stack<int>();                                                            // LIFO stack to convert, track and chain tabs (\t) to trigger indexes.

                //values that populate prototrigger
                Match regexGrouping;
                bool scriptAscentMode = true;
                int currentIndex = 0;
                int linkedIndex;
                TriggerType currentTrigger;

                internal TriggerFactory()
                {
                        regexDict.Add("oneParamFromMaxValRegex", @"^\t*\w+\s+(\d+)\s*(\w+)?\s*$");
                        regexDict.Add("oneWordRegex", @"^\w+\s*");
                        regexDict.Add("tabcount", @"^(\t)+\w+");
                        regexDict.Add("countdown", @"^\t*\w+\s+(?:Y(\d{1,4})\s*,\s*D(\d{1,3})\s*,\s*|T-)(?:(?:(?:(\d{1,2}):)?(?:(\d{1,2}):)?)?(\d{1,2}))\s*$");
                        regexDict.Add("attitude", @"^\t*\w+\s+(\w+)(?:\s+(-?(?:[0-9]{1,3})))?(?:,(-?(?:[0-8][0-9]?|90)),(-?(?:[0-8][0-9]?|90)))?");

                        /*Many to one Relationship*/
                        triggerRegex.Add(TriggerType.ASCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.DESCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.ALTITUDE, regexDict["oneParamFromMaxValRegex"]);
                        triggerRegex.Add(TriggerType.COUNTDOWN, regexDict["countdown"]);
                        triggerRegex.Add(TriggerType.ATTITUDE, regexDict["attitude"]);

                        triggerProducts.Add(TriggerType.ALTITUDE, () => { return new Altitude(currentIndex, linkedIndex, currentTrigger, UpperFirstChar(currentTrigger.ToString()), scriptAscentMode, SetModifier(TriggerModifier.FROMMAXVAL, regexGrouping.Groups[2].Value.ToString()), Convert.ToDouble(regexGrouping.Groups[1].Value.ToString())); });


                        triggerProducts.Add(TriggerType.ATTITUDE, () => { return new Attitude(currentIndex, linkedIndex, currentTrigger, ParseEnum<AttitudeType>(regexGrouping.Groups[1].Value), SetAttitude(regexGrouping.Groups[2].Value.ToString(), regexGrouping.Groups[3].Value.ToString(), regexGrouping.Groups[4].Value.ToString())); });

                }

                internal int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {
                        Log.Level(LogType.Verbose, "Validating Syntax: " + commandLine + " : " + triggerRegex[trigger]);

                        currentTrigger = trigger;

                        regexGrouping = Regex.Match(commandLine, triggerRegex[trigger]);                           //Check command line for valid syntax, if true then parse it

                        if (regexGrouping.Success)
                        {

                                if (SetTriggerMode(trigger)) { return -1; }                                              // return -1 if a trigger has no index. i.e. a bit flipper.
                                
                                currentIndex++;                                                                          

                                linkedIndex = GetParentIndex(trigger, commandLine, lineNumber, currentIndex);
                                Log.Level(LogType.Verbose, "CURRENT INDEX: " + currentIndex);
                                Debug.Log(trigger.ToString());
                                Debug.Log("GET PARENT INDEX: LINKED INDEX = " + linkedIndex);

                                NewTriggerSequence.Add(triggerProducts[trigger]());

                                
                                //Log.Level(LogType.Verbose, "TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                                
                        }
                        else
                        {
                                Log.Script(LogType.Error,"Unable to parse command line. Check Syntax. "+ "Line #" + lineNumber + ": Command: " + commandLine );
                        }
                        

                        return currentIndex;
                }

                internal SequenceEngine GetNewFlightSequence(List<Action> newactionlist)
                {
                        Log.Level(LogType.Verbose, "GetNewTriggerGuardian: creating trigger guardian");
                        return new SequenceEngine(NewTriggerSequence, newactionlist);
                }

                bool SetTriggerMode(TriggerType trigger)
                {
                        switch (trigger)
                        {

                                case TriggerType.ASCENT:

                                        scriptAscentMode = true;
                                        return true;

                                case TriggerType.DESCENT:

                                        scriptAscentMode = false;
                                        return true;

                                default:
                                        return false;
                        }
                }

                Vector3 SetAttitude(string h, string p, string r)
                {

                        try
                        {
                                float heading = string.IsNullOrEmpty(h) ? 0f : Convert.ToSingle(h);
                                float pitch   = string.IsNullOrEmpty(p) ? 0f : Convert.ToSingle(p);
                                float roll    = string.IsNullOrEmpty(r) ? 0f : Convert.ToSingle(r);

                                Debug.Log("HEADING: "+heading+" PITCH: "+pitch+" ROLL: "+ roll);
                                return new Vector3(heading, pitch, roll);
                                
                        }
                        catch
                        {
                                Log.Script(LogType.Error, "Attitude value is not a number in a valid format. Line #");
                        }

                        return new Vector3(0,0,0);
    
                }

                bool SetModifier(TriggerModifier modifier, string regexgroup)
                {
                        if(String.IsNullOrEmpty(regexgroup))
                        {
                                return false;
                        }

                        if ( modifier == (TriggerModifier)Enum.Parse(typeof(TriggerModifier), regexgroup) )
                        {
                                return true;
                        }
                        
                        return false;
                }

                int GetParentIndex(TriggerType trigger, string commandline, int linenumber, int currentindex)
                {
                        // If tabcount == 0; first level trigger, has no parent index
                        // If (tabcount - tabCountStack.Count) == 0, next level trigger; Peek index value then put it in the new trigger's index and push new trigger on stack
                        // If tabcount < tabCountStack.Count; lower level trigger; pop triggers off stack until current trigger is on top of it's corresponding chained trigger
                        // If (tabcount - tabCountStack.Count) > 0: tab error; Catch unchained trigger and throw error

                        int tabcount = GetTabCount(commandline);
                        int linkedindex = 0;

                        Log.Level(LogType.Verbose, "Tab Count \t: " + tabcount);

                        if (tabcount == 0)
                        {
                                linkedindex = 0;                                               // This is an unchained (root) trigger.
                                tabCountStack.Clear();
                                tabCountStack.Push(currentindex);
                        }
                        else if ((tabcount - tabCountStack.Count) == 0)
                        {
                                linkedindex = tabCountStack.Peek();
                                tabCountStack.Push(currentindex);
                        }
                        else if (tabcount < tabCountStack.Count)
                        {
                                for (int i = tabCountStack.Count; i > tabcount; i--)
                                {
                                        tabCountStack.Pop();
                                }

                                linkedindex = tabCountStack.Peek();
                                tabCountStack.Push(currentindex);
                        }
                        else if ((tabcount - tabCountStack.Count) > 0)
                        {
                                Log.Script(LogType.Error, "Unchained Trigger: Check Tab Structure. Line #" + linenumber + ": Command: " + commandline);                //Create loading error in flightlog window
                        }

                        return linkedindex;

                }


                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, regexDict["tabcount"]).Groups[1].Captures.Count;

                }

                internal int GetTabStackCount()
                {
                        return tabCountStack.Count;

                }

                T ParseEnum<T>(string value)
                {
                        try
                        {
                                return (T)Enum.Parse(typeof(T), value, true);
                        }
                        catch
                        {
                                Log.Script(LogType.Error, "\"" + value + "\"" + " is not a valid trigger parameter for " + typeof(T));
                        }

                        return (T)Enum.Parse(typeof(T), value, true);

                }

                static string UpperFirstChar(string s)
                {
                        return char.ToUpper(s[0]) + s.Substring(1).ToLower();
                }
        }
}
