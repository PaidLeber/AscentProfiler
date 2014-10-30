using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightRecorder
        {

                AscentProAPGCSModule module;

                internal List<string> MissionLog = new List<string>();
                internal int lastFlightLogTransmitCount = 0;
                internal bool logEnabled = true;


                SortedDictionary<float, int> index = new SortedDictionary<float,int>();
                List<List<double>> telemetry = new List<List<double>>();


                internal FlightRecorder(AscentProAPGCSModule module)
                {
                        this.module = module;
                        MissionLog.Add("TEST0");
                        MissionLog.Add("TEST1");
                        MissionLog.Add("TEST2");
                        MissionLog.Add("TEST3");
                        MissionLog.Add("TEST4");
                        MissionLog.Add("TEST5");
                        MissionLog.Add("TEST6");
                        MissionLog.Add("TEST7");
                        MissionLog.Add("TEST8");
                        MissionLog.Add("TEST9");
                        MissionLog.Add("TEST10");
                        MissionLog.Add("TEST11");
                }

                internal void AddLog(string log)
                {
                        var transferlog = timeStamp(module.vessel.missionTime) + " - " + log;
                        MissionLog.Add(transferlog);
                        Log.Level(LogType.Verbose, transferlog);

                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("T+{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }



        }
}
