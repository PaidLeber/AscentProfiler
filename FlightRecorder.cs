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
                        FlightLog.Add("TEST8");
                        FlightLog.Add("TEST9");
                        FlightLog.Add("TEST10");
                        FlightLog.Add("TEST11");
                }

                internal void AddLog(string log)
                {
                        var transferlog = timeStamp(module.vessel.missionTime) + " - " + log;
                        FlightLog.Add(transferlog);
                        Log.Level(LogType.Verbose, transferlog);

                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("T+{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }



        }
}
