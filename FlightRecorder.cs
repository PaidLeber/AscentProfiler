using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightRecorder
        {

                AscentProAPGCSModule module;

                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;
                internal bool missionLogEnabled = true;


                bool telemetryReadyToTransmit = false;
                Dictionary<SensorType, double[]> telemetryData = new Dictionary<SensorType, double[]>();               
                

                internal FlightRecorder(AscentProAPGCSModule module)
                {
                        this.module = module;
                        missionLog.Add("TEST0");
                        missionLog.Add("TEST1");
                        missionLog.Add("TEST2");
                        missionLog.Add("TEST3");
                        missionLog.Add("TEST4");
                        missionLog.Add("TEST5");
                        missionLog.Add("TEST6");
                        missionLog.Add("TEST7");
                        missionLog.Add("TEST8");
                        missionLog.Add("TEST9");
                        missionLog.Add("TEST10");
                        missionLog.Add("TEST11");
                }

                internal void AddLog(string log)
                {
                        var transferlog = timeStamp(module.vessel.missionTime) + " - " + log;
                        missionLog.Add(transferlog);
                        Log.Level(LogType.Verbose, transferlog);

                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("T+{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                        
                }



        }
}
