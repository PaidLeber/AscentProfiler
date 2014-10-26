using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightRecorder
        {

                List<string> Log;
                SortedDictionary<float, int> index;
                List<List<double>> data;
                Vessel vessel;

                internal FlightRecorder(Vessel v)
                {
                        this.vessel = v;
                }

                string output;

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }
                
                void LogOutput(string log)
                {

                        output = timeStamp(FlightGlobals.ActiveVessel.missionTime) + " - " + log + "\n" + output;

                }
        }
}
