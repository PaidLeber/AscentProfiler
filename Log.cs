using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        internal enum LogLevel
        { 
                INFO,
                WARNING,
                ERROR,
                VERBOSE
        
        }

        internal static class Log
        {
                internal bool debug = true;

                internal void Debug()
                {
                        if (!debug) { return; }




                }

                internal void Flight()
                { 
                
                }


        }
}
