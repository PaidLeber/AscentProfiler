using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AscentProfiler
{

        class ActionFactory
        {
                internal List<Action> actionProducts = new List<Action>();

                Match regexGrouping;

                string actionRegex = @"^\t*\w+\s+(?:(\w+)\s+)?(\d+)\s*$";

                internal ActionFactory()
                { 
                
                }

                public void CreateAction(ActionType action, int currentindex, int tabstackcount, string commandline, int linenumber)
                {
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
                                //(tabcount - tabCountStack.Count) == 0
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Unable to parse command line. Check Syntax.", "Line #" + linenumber + ": Command: " + commandline + ":");
                        }

                }



                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, @"^(\t)+\w+").Groups[1].Captures.Count;

                }



        }
}
