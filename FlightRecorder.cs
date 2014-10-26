using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightRecorder
        {

                List<string> Log = new List<string>();
                SortedDictionary<float, int> index = new SortedDictionary<float,int>();
                List<List<double>> telemetry = new List<List<double>>();
                AscentProAPGCSModule module;

                internal FlightRecorder(AscentProAPGCSModule module)
                {
                        this.module = module;
                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }
                
                internal void Report(string log)
                {

                        Log.Add( timeStamp(module.vessel.missionTime) + " - " + log);

                }
        }
}
