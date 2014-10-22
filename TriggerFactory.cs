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
                internal Dictionary<string, string> regexDict = new Dictionary<string, string>();
                Dictionary<TriggerType, string> triggerRegex = new Dictionary<TriggerType, String>();
                Dictionary<TriggerType, Func<Trigger>> triggerProduct = new Dictionary<TriggerType, Func<Trigger>>();

                // Use a LIFO stack to convert, track and chain tabs (\t) to trigger indexes.
                Stack<int> tabCountStack = new Stack<int>();

                //variables used for trigger contructors
                int TRIGGERINDEX;
                TriggerType TRIGGERTYPE;
                string DESCRIPTION;
                double DBLVALUE;
                bool ASCENDING = true;
                bool FROMMAXVAL;
                string STRVALUE;

                int currentIndex = 0;

                internal TriggerFactory()
                {
                        regexDict.Add("START", @"^START\s*$");
                        regexDict.Add("END", @"^END\s*$");
                        regexDict.Add("CMDBEGIN", @"^\t*");
                        regexDict.Add("CMDEND", @"\s*.*$");
                        regexDict.Add("oneParamFromMaxValRegex", @"^\t*\w+\s+(\d+)\s*(\w*)\s*$");
                        regexDict.Add("oneWordRegex", @"^\w+\s*");
                        regexDict.Add("tabcount", @"^(\t)+\w+");
                        regexDict.Add("countdown", @"^\t*\w+\s+(?:Y(\d{1,4})\s*,\s*D(\d{1,3})\s*,\s*|T-)(?:(?:(?:(\d{1,2}):)?(?:(\d{1,2}):)?)?(\d{1,2}))\s*$");
                        

                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, ASCENDING, FROMMAXVAL, DBLVALUE); });
                        triggerProduct.Add(TriggerType.COUNTDOWN, () => { return new Countdown(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, DBLVALUE); });

                        triggerRegex.Add(TriggerType.ASCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.DESCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.ALTITUDE, regexDict["oneParamFromMaxValRegex"]);
                        triggerRegex.Add(TriggerType.COUNTDOWN, regexDict["countdown"]);
                
                }
                
                void ClearTriggerValues()
                {
                    
                        TRIGGERTYPE = TriggerType.None;
                        DESCRIPTION = null;
                        FROMMAXVAL = false;
                        DBLVALUE = 0;
                        STRVALUE = "";
                
                }

                internal int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {
                        ClearTriggerValues();
                        
                        /*Check for non trigger commands*/

                        // Check for trigger mode switches
                        if (trigger == TriggerType.ASCENT)
                        {

                                return -1;
                        }
                        else if (trigger == TriggerType.DESCENT)
                        {
                                ASCENDING = false;
                                return -1;
                        }

                        Log.Level(LogType.Verbose, "Validating Syntax: " + commandLine +" : "+ triggerRegex[trigger]);

                        //Check command line for valid syntax, if true then parse it
                        Match regexGrouping = Regex.Match(commandLine, triggerRegex[trigger]);

                        if (regexGrouping.Success)
                        {
                                if(!SetTriggerValues(trigger, regexGrouping))
                                {
                                        return -1;  // return -1 if a trigger has no index. i.e. a bit flipper.
                                }
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Line #" + lineNumber + ": Command: " + commandLine + ":", "Unable to parse command line. Check Syntax.");
                        }


                        /*Create Trigger Classes*/
                        // If tabcount == 0; first level trigger
                        // If (tabcount - tabCountStack.Count) == 0, next level trigger; then Peek index value put it in the new trigger's index and push new trigger on stack
                        // If tabcount < tabCountStack.Count; lower level trigger; pop triggers off stack until current trigger is pushed on top of it's corresponding chained trigger
                        // If (tabcount - tabCountStack.Count) > 0: tab error; Catch unchained trigger and throw error

                        currentIndex++;

                        int tabcount = GetTabCount(commandLine);

                        Log.Level(LogType.Verbose, "Checking Tab Structures: " + trigger.ToString());
                        Log.Level(LogType.Verbose, "tabcount: " + tabcount);

                        if (tabcount == 0)
                        {
                                TRIGGERINDEX = 0; // This is an unchained (root) trigger.
                                tabCountStack.Clear();
                                tabCountStack.Push(currentIndex);
                        }
                        else if ((tabcount - tabCountStack.Count) == 0)
                        {
                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else if (tabcount < tabCountStack.Count)
                        {
                                for (int i = tabCountStack.Count; i > tabcount; i--)
                                {
                                        tabCountStack.Pop();
                                }

                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else if ((tabcount - tabCountStack.Count) > 0)
                        {
                                //Create loading error in flightlog window
                                Log.Script(LogType.Error, "Line #" + lineNumber + ": Command: " + commandLine + ":", "Check Tab Structure: Unchained trigger.");
                        }
                        
                        
                        

                        AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, triggerProduct[trigger]());

                        Log.Level(LogType.Verbose, "CURRENT INDEX: " + currentIndex);
                        Log.Level(LogType.Verbose, "TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        
                        return currentIndex;
                }




                bool SetTriggerValues(TriggerType trigger, Match triggergroups)
                {

                        Log.Level(LogType.Verbose, "whole value: " + triggergroups.Groups[0].Value
                        + " g1:" + triggergroups.Groups[1].Value
                        + " g2:" + triggergroups.Groups[2].Value
                        + " g3:" + triggergroups.Groups[3].Value);

                        switch (trigger)
                        {       

                                case TriggerType.ASCENT:
                                        
                                        ASCENDING = true;
                                        return false;

                                case TriggerType.DESCENT:

                                        ASCENDING = false;
                                        return false;

                                case TriggerType.ALTITUDE:

                                        TRIGGERTYPE = trigger;
                                        DESCRIPTION = UpperFirstChar(trigger.ToString());
                                        DBLVALUE = Convert.ToDouble(triggergroups.Groups[1].Value);

                                        SetTriggerModifier(triggergroups.Groups[2].Value);



                                        break;

                                case TriggerType.COUNTDOWN:
                                        //pull values and multiply to get total time in seconds
                                        break;


                        }

                        Log.Level(LogType.Verbose, "NEW TRIGGER VARIABLES: " + " index: " + TRIGGERINDEX + " type: " + TRIGGERTYPE + " desc: " + DESCRIPTION + " ascentmode: " + ASCENDING + " strvalue: " + STRVALUE + " dblvalue: " + DBLVALUE + " frommaxval: " + FROMMAXVAL);

                        return false;
                }


                bool SetTriggerModifier(string modifier)
                {
                        if (!String.IsNullOrEmpty(modifier))
                        {
                                Log.Level(LogType.Verbose, "triggerParse.Groups[2].Value Is not null: " + modifier);
                                // Check command line for optional trigger switches if so enable
                                switch ((TriggerModifier)Enum.Parse(typeof(TriggerModifier), modifier))
                                {
                                        case TriggerModifier.FROMMAXVAL:
                                                FROMMAXVAL = true;
                                                break;


                                }
                        }

                        return false;
                }

                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, regexDict["tabcount"]).Groups[1].Captures.Count;

                }

                static string UpperFirstChar(string s)
                {
                        // Check for empty string.
                        if (string.IsNullOrEmpty(s))
                        {
                                return string.Empty;
                        }
                        // Return char and concat substring.
                        return char.ToUpper(s[0]) + s.Substring(1);
                }
        }
}
