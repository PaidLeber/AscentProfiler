using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightRecorder
        {

                AscentProAPGCSModule module;

                internal List<string> FlightLog = new List<string>();
                internal int lastFlightLogTransmitCount = 0;
                internal bool logEnabled = true;


                SortedDictionary<float, int> index = new SortedDictionary<float,int>();
                List<List<double>> telemetry = new List<List<double>>();


                internal FlightRecorder(AscentProAPGCSModule module)
                {
                        this.module = module;
                        FlightLog.Add("TEST0");
                        FlightLog.Add("TEST1");
                        FlightLog.Add("TEST2");
                        FlightLog.Add("TEST3");
                        FlightLog.Add("TEST4");
                        FlightLog.Add("TEST5");
                        FlightLog.Add("TEST6");
                        FlightLog.Add("TEST7");
                }

                internal void Log(string log)
                {

                        FlightLog.Add(timeStamp(module.vessel.missionTime) + " - " + log);

                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }



        }
}
