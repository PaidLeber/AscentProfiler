using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AscentProfiler
{

        class ActionFactory
        {
                List<Action> actionProducts = new List<Action>();

                Match regexGrouping;

                string actionRegex = @"^\t*\w+\s+(?:(\w+)\s+)?(?:(\d+)\s+)?(?:""[\w\s]+"")?\s*$";

                int currentIndex;

                internal ActionFactory()
                {
                        actionProducts.Add(new ActionGroup( currentIndex, true, Convert.ToInt16(regexGrouping.Groups[2].Value) ));
                }

                void CreateAction(ActionType action, int currentindex, int tabstackcount, string commandline, int linenumber)
                {
                        currentIndex = currentindex;

                        regexGrouping = Regex.Match(commandline, actionRegex);

                        if (regexGrouping.Success)
                        {
                                if (GetTabCount(commandline) - tabstackcount == 0)
                                {



                                }
                                else
                                {
                                        Log.Script(LogType.Error, "Unchained Action. Check Tab Structure.", "Line #" + linenumber + ": Command: " + commandline);                //Create loading error in flightlog window
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

                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, @"^(\t)+\w+").Groups[1].Captures.Count;

                }



        }
}
