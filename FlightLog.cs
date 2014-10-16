using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        public class FlightLog
        {
                public string output;

                public string timeStamp(double secs)
                {
                       TimeSpan t = TimeSpan.FromSeconds( secs );

                       return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                }

                public void Log(string log)
                {

                       output = timeStamp(FlightGlobals.ActiveVessel.missionTime) + " - " + log +"\n" + output;
                
                }



        }
}
