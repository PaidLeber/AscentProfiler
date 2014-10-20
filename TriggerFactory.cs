using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
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
                        regexDict.Add("timer", @"^\t*\w+\s*(?:(?:(\d{1,2}):)?(\d{1,2}):)?(\d{1,2})$");
                        regexDict.Add("clock", @"([0-9]?[0-9])+");
                        // countdown regex      \t*\w+\s+(Y\d{1,4}\s*,\s*D\d{1,3}\s*,\s*(([0-9]?[0-9]):([0-9]?[0-9]):([0-9]?[0-9]))|(T-([0-9]?[0-9]):([0-9]?[0-9]):([0-9]?[0-9]))|(UT\d+)|(\d+s)\s*$

                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, ASCENDING, FROMMAXVAL, DBLVALUE); });
                        triggerProduct.Add(TriggerType.COUNTDOWN, () => { return new Countdown(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, STRVALUE); });

                        triggerRegex.Add(TriggerType.ASCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.DESCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.ALTITUDE, regexDict["oneParamFromMaxValRegex"]);
                        triggerRegex.Add(TriggerType.COUNTDOWN, regexDict["timer"]);
                
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
                                ASCENDING = true;
                                return -1;
                        }
                        else if (trigger == TriggerType.DESCENT)
                        {
                                ASCENDING = false;
                                return -1;
                        }

                        Log.Level(LogType.Verbose, "Checking Command Syntax: " + commandLine +" : "+ triggerRegex[trigger]);

                        //Check command line for valid syntax, if true then parse it
                        Match triggerParse = Regex.Match(commandLine, triggerRegex[trigger]);


                        if (triggerParse.Success)
                        {
                                if(trigger == TriggerType.COUNTDOWN)
                                {
                                        Log.Level(LogType.Verbose, "whole value: " + triggerParse.Groups[0].Value + " g1:" + triggerParse.Groups[1].Value + " g2:" + triggerParse.Groups[2].Value + " g3:" + triggerParse.Groups[3].Value);

                                        Match countdownparse = Regex.Match(commandLine, triggerParse.Groups[0].Value);

                                        Log.Level(LogType.Verbose, "countdown parse list");
                                        Log.Level(LogType.Verbose, countdownparse.Groups[0].Value);
                                        Log.Level(LogType.Verbose, countdownparse.Groups[1].Value);
                                        Log.Level(LogType.Verbose, countdownparse.Groups[2].Value);
                                        

                                }
                                        
                                if (!String.IsNullOrEmpty(triggerParse.Groups[2].Value))
                                {
                                        Log.Level(LogType.Verbose, "triggerParse.Groups[2].Value Is not null: " + triggerParse.Groups[2].Value);
                                        // Check command line for optional trigger switches if so enable
                                        switch ((TriggerModifier)Enum.Parse(typeof(TriggerModifier), triggerParse.Groups[2].Value))
                                        {
                                                case TriggerModifier.FROMMAXVAL:
                                                        FROMMAXVAL = true;
                                                        break;


                                        }                          
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

                        /*Populate Trigger Classes*/
                        Log.Level(LogType.Verbose, "Populating trigger variables.");
                        TRIGGERTYPE = trigger;
                        Log.Level(LogType.Verbose, "converting to double: "+ triggerParse.Groups[1].Value);

                        if (!string.IsNullOrEmpty(triggerParse.Groups[1].Value))
                        {
                                DBLVALUE = Convert.ToDouble(triggerParse.Groups[1].Value);
                        }
                        
                        
                        DESCRIPTION = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trigger.ToString());

                        Log.Level(LogType.Verbose, "NEW TRIGGER VARIABLES: " + " index: " + TRIGGERINDEX + " type: " + TRIGGERTYPE + " desc: " + DESCRIPTION + " ascentmode: " + ASCENDING + " strvalue: "+ STRVALUE + " dblvalue: " + DBLVALUE + " frommaxval: " + FROMMAXVAL);

                        AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, triggerProduct[trigger]());

                        Log.Level(LogType.Verbose, "CURRENT INDEX: " + currentIndex);
                        Log.Level(LogType.Verbose, "TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        
                        return currentIndex;
                }


                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, regexDict["tabcount"]).Groups[1].Captures.Count;
                        
                }

        }
}
