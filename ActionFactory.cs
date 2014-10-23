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

                public void CreateAction(ActionType action, int currentindex, int tabstackcount, string commandLine, int lineNumber)
                {
                        regexGrouping = Regex.Match(commandLine, actionRegex);

                        if (regexGrouping.Success)
                        {
                                
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Unable to parse command line. Check Syntax.", "Line #" + lineNumber + ": Command: " + commandLine + ":");
                        }

                }





                public void Reset()
                {
                        //actionlist.Clear();
                }


        }
}
