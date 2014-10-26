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
                List<List<double>> data = new List<List<double>>();
                Vessel vessel;

                internal FlightRecorder(Vessel v)
                {
                        this.vessel = v;
                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }
                
                void Report(string log)
                {

                        Log.Add( timeStamp(vessel.missionTime) + " - " + log);

                }
        }
}
