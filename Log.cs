using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        internal enum LogType
        { 
                Info,
                Warning,
                Error,
                Verbose
        
        }
        
        internal static class Log
        {

                internal static List<string> consolebuffer = new List<string>();

                internal static void ConsoleAppendLine(string value)
                {
                        consolebuffer[consolebuffer.Count - 1] = consolebuffer[consolebuffer.Count - 1] + value;

                }

                internal static void Console(string value)
                {
                        if(value == "\n")
                        {
                                consolebuffer.Add("");
                                consolebuffer.Add("");
                                return;
                        }

                        Debug.Log("log value: " + value);
                        Debug.Log("console count: "+consolebuffer.Count+ "index: "+ (consolebuffer.Count - 1));

                        if(consolebuffer.Count != 0)
                        {
                                if (string.IsNullOrEmpty(consolebuffer[consolebuffer.Count - 1]))
                                {
                                        Debug.Log("last item is empty: " + consolebuffer[consolebuffer.Count - 1]);
                                        consolebuffer.RemoveAt(consolebuffer.Count - 1);
                                }
                        }



                        consolebuffer.Add(value);
                       


                }

                internal static void ConsoleUpdate()
                {
                        if (consolebuffer.Count > 7)
                        {
                                consolebuffer.RemoveAt(0);
                        }
                }

                static LogType loglevel = LogType.Verbose;
                static LogType scriptlevel = LogType.Error;


                private static bool _debug;
                static bool debug
                {
                        get
                        {
                                return _debug;
                        }
                        set
                        {
                                _debug = value;
                                if (value)
                                {
                                        loglevel = LogType.Verbose;
                                }
                                else
                                { 
                                        loglevel = LogType.Error;
                                }
                        }
                }



                internal static void Level(LogType lType, string message)
                {
                        if(lType > loglevel)
                        {
                                return;
                        }

                        message = "Ascent Profiler: " + lType + ": " + message;

                        UnityEngine.Debug.Log( message );
                }


                internal static void Script1(LogType lType, string message)
                {
                        if (lType > scriptlevel)
                        {
                                return;
                        }

                        message = "Loading Gscript: "+ lType + ": " + message;

                        switch (lType)
                        { 
                                case LogType.Error:

                                        throw new Exception(message);
                        
                        }


                }


        }
}
