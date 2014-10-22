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

                Stack<int> tabCountStack = new Stack<int>();                                                            // LIFO stack to convert, track and chain tabs (\t) to trigger indexes.

                bool scriptAscentMode = true;
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
                        
                        triggerRegex.Add(TriggerType.ASCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.DESCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.ALTITUDE, regexDict["oneParamFromMaxValRegex"]);
                        triggerRegex.Add(TriggerType.COUNTDOWN, regexDict["countdown"]);
                
                }
              
                internal int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {

                        Log.Level(LogType.Verbose, "Validating Syntax: " + commandLine +" : "+ triggerRegex[trigger]);

                        TriggerInput directive = new TriggerInput();                                                     //Struct of values that is passed to new trigger class constructor

                        Match regexGrouping = Regex.Match(commandLine, triggerRegex[trigger]);                           //Check command line for valid syntax, if true then parse it

                        if (regexGrouping.Success)
                        {

                                if (SetTriggerMode(trigger)) { return -1; }                                              // return -1 if a trigger has no index. i.e. a bit flipper.
                                
                                currentIndex++;                                                                          

                                int linkedIndex = GetParentIndex(trigger, commandLine, lineNumber, currentIndex);        

                                if(SetTriggerValues(trigger, regexGrouping, linkedIndex, directive))
                                {
                                        
                                        AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, (Trigger)Activator.CreateInstance(Type.GetType(trigger.ToString()), directive) );
                                        Log.Level(LogType.Verbose, "New Trigger: " + trigger + ": ");
                                        Log.Level(LogType.Verbose, "CURRENT INDEX: " + currentIndex);
                                        Log.Level(LogType.Verbose, "TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                                }
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Line #" + lineNumber + ": Command: " + commandLine + ":", "Unable to parse command line. Check Syntax.");
                        }
                        
                        return currentIndex;
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


                bool SetTriggerValues(TriggerType trigger, Match triggergroups, int linkedindex, TriggerInput directive)
                {

                        Log.Level(LogType.Verbose, "whole value: " + triggergroups.Groups[0].Value
                        + " g1:" + triggergroups.Groups[1].Value
                        + " g2:" + triggergroups.Groups[2].Value
                        + " g3:" + triggergroups.Groups[3].Value);

                        switch (trigger)
                        {       

                                case TriggerType.ALTITUDE:

                                        directive.index         = linkedindex;
                                        directive.type          = trigger;
                                        directive.description   = UpperFirstChar(trigger.ToString());
                                        directive.value         = Convert.ToDouble(triggergroups.Groups[1].Value);
                                        directive.ascentMode    = scriptAscentMode;
                                        directive.fromaxval     = SetModifier(TriggerModifier.FROMMAXVAL, triggergroups.Groups[2].Value);

                                        return true;

                                case TriggerType.COUNTDOWN:
                                        //pull values and multiply to get total time in seconds
                                        break;

                                default:
                                        return false;

                        }

                    

                        return false;
                }


                bool SetModifier(TriggerModifier modifier, string regexgroup)
                {

                        if ( modifier == (TriggerModifier)Enum.Parse(typeof(TriggerModifier), regexgroup) && !String.IsNullOrEmpty(regexgroup) )
                        {
                                return true;
                        }
                        
                        return false;
                }

                int GetParentIndex(TriggerType trigger, string commandline, int linenumber, int currentindex)
                {
                        // If tabcount == 0; first level trigger
                        // If (tabcount - tabCountStack.Count) == 0, next level trigger; then Peek index value put it in the new trigger's index and push new trigger on stack
                        // If tabcount < tabCountStack.Count; lower level trigger; pop triggers off stack until current trigger is pushed on top of it's corresponding chained trigger
                        // If (tabcount - tabCountStack.Count) > 0: tab error; Catch unchained trigger and throw error

                        int tabcount = GetTabCount(commandline);
                        int linkedindex = 0;

                        Log.Level(LogType.Verbose, "Checking Tab Structures: " + trigger.ToString());
                        Log.Level(LogType.Verbose, "tabcount: " + tabcount);

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
                                Log.Script(LogType.Error, "Line #" + linenumber + ": Command: " + commandline + ":", "Check Tab Structure: Unchained trigger.");                //Create loading error in flightlog window
                        }

                        return linkedindex;

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
