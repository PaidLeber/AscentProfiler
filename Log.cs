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

                internal static Queue<string> console = new Queue<string>();


                internal static void Console(string value)
                {
                        console.Enqueue(value);
                        if (console.Count > 8)
                        {
                                console.Dequeue();
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

                internal static void Script(LogType lType, string message)
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
