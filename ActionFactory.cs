using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace AscentProfiler
{

        class ActionFactory
        {

                internal Dictionary<string, string> actionDict = new Dictionary<string, string>();
                Dictionary<ActionType, string> actionRegex = new Dictionary<ActionType, String>();

                List<Action> NewActionList = new List<Action>();
                Dictionary<ActionType, Func<Action>> actionProducts = new Dictionary<ActionType, Func<Action>>();
                Match regexGrouping;


                //values that populate proto-action
                int currentIndex;
                ActionType currentAction;

                internal ActionFactory()
                {
                        actionDict.Add("actionRegex", @"^\t*\w+\s+(\w+)\s+(\w+)(?:\s+""([\w\s]+)"")?\s*$");
                        actionDict.Add("sensorsRegex", @"^\t*\w+\s+(\w+)\s*$");

                        actionRegex.Add(ActionType.SENSORS, actionDict["sensorsRegex"]);
                        actionRegex.Add(ActionType.ACTIONGROUP, actionDict["actionRegex"]);

                        actionProducts.Add(ActionType.ACTIONGROUP, () => { return new ActionGroup(currentIndex, currentAction, ParseEnum<ActionModifier>(regexGrouping.Groups[2].Value), ParseEnum<KSPActionGroup>(regexGrouping.Groups[1].Value), regexGrouping.Groups[3].Value.ToString()); });
                        actionProducts.Add(ActionType.SENSORS,     () => { return new Sensors(currentIndex, currentAction, ParseEnum<SensorType>(regexGrouping.Groups[1].Value)); });
                        actionProducts.Add(ActionType.TELEMETRY,   () => { return new Telemetry(currentIndex, currentAction, ParseEnum<TelemetryType>(regexGrouping.Groups[1].Value), ParseEnum<ActionModifier>(regexGrouping.Groups[2].Value)); });
                }

                internal void CreateAction(ActionType action, int currentindex, int tabstackcount, string commandline, int linenumber)
                {
                        regexGrouping = Regex.Match(commandline, actionRegex[action]);

                        if (regexGrouping.Success)
                        {
                                Log.Level(LogType.Verbose, "Action Valid. Success! Groups Next:");
                                Log.Level(LogType.Verbose, regexGrouping.Groups[1].Value);
                                Log.Level(LogType.Verbose, regexGrouping.Groups[2].Value);
                                Log.Level(LogType.Verbose, regexGrouping.Groups[3].Value);

                                if (GetTabCount(commandline) - tabstackcount == 0)
                                {
                                        Log.Level(LogType.Verbose, action.ToString() + " :tabcount: "+GetTabCount(commandline)+ " tabstackcount: "+ tabstackcount);

                                        currentIndex = currentindex;
                                        currentAction = action;

                                        NewActionList.Add(actionProducts[action]());
                                        
                                }
                                else
                                {
                                        Log.Script(LogType.Error, "Unchained Action: Check Tab Structure. Line #" + linenumber + ": Command: " + commandline);                //Create loading error in flightlog window
                                }
                                
                        }
                        else
                        {
                                Log.Script(LogType.Error, "Unable to parse command line. Check Syntax. Line #" + linenumber + ": Command: " + commandline);
                        }

                }

                internal List<Action> GetNewActionList()
                {
                        Log.Level(LogType.Verbose, "GetNewTriggerGuardian: creating action executor");
                        return NewActionList;
                }


                T ParseEnum<T>(string value)
                {
                        try
                        {
                                return (T)Enum.Parse(typeof(T), value, true);
                        }
                        catch
                        {
                                Log.Script(LogType.Error, value + " is not a valid action parameter");
                        }

                        return (T)Enum.Parse(typeof(T), value, true);
                        
                }
                
                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, @"^(\t)+\w+").Groups[1].Captures.Count;

                }



        }
}
