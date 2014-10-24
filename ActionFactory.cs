using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AscentProfiler
{

        class ActionFactory
        {
                Dictionary<ActionType, Func<Action>> actionProducts = new Dictionary<ActionType, Func<Action>>();

                Match regexGrouping;

                string actionRegex = @"^\t*\w+\s+(\w+)\s+(\w+)(?:\s+""([\w\s]+)"")?\s*$";

                //values that populate protoaction
                int currentIndex;
                ActionType currentAction;
                KSPActionGroup currentActionGroupObject;
                ActionModifier currentModifier;
                string currentDescription;

                internal ActionFactory()
                {
                        actionProducts.Add(ActionType.ACTIONGROUP, () => { return new ActionGroup(currentIndex, currentAction, currentActionGroupObject, currentModifier, currentDescription, Convert.ToInt16(regexGrouping.Groups[2].Value)); });

                }

                internal void CreateAction(ActionType action, int currentindex, int tabstackcount, string commandline, int linenumber)
                {
                        regexGrouping = Regex.Match(commandline, actionRegex);

                        if (regexGrouping.Success)
                        {
                                Log.Level(LogType.Verbose, "Action Valid. Success! Groups Next:");
                                Log.Level(LogType.Verbose, regexGrouping.Groups[1].Value);
                                Log.Level(LogType.Verbose, regexGrouping.Groups[2].Value);
                                Log.Level(LogType.Verbose, regexGrouping.Groups[3].Value);
                                if (GetTabCount(commandline) - tabstackcount == 0)
                                {
                                        currentIndex = currentindex;
                                        currentAction = action;


                                        //AscentProfiler.ActiveProfile.actionExecutor.actionlist.Add( actionProducts[action]());

                                }
                                else
                                {
                                        Log.Script(LogType.Error, "Unchained Action: Check Tab Structure.", "Line #" + linenumber + ": Command: " + commandline);                //Create loading error in flightlog window
                                }
                                
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Unable to parse command line. Check Syntax.", "Line #" + linenumber + ": Command: " + commandline + ":");
                        }

                }



                bool SetModifier(ActionModifier modifier, string regexgroup)
                {
                        if (String.IsNullOrEmpty(regexgroup))
                        {
                                return false;
                        }

                        if (modifier == (ActionModifier)Enum.Parse(typeof(ActionModifier), regexgroup))
                        {
                                return true;
                        }

                        return false;
                }
                
                bool IsValidActionParameter(Enum e, string enumstring)
                {
                        var enumList = Enum.GetNames(e.GetType());

                        foreach (var enumvalue in enumList)
                        {
                                if( enumvalue.ToUpper() == enumstring)
                                {
                                        return true;
                                }
                        }
                        return false;
                }
                
                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, @"^(\t)+\w+").Groups[1].Captures.Count;

                }



        }
}
