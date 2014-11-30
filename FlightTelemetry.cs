using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightTelemetry
        {

                AscentProAPGCSModule module;

                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;
                internal bool missionLogEnabled = true;

                bool sensorsDataReadyToTransmit = false;
                bool sensorsEnabled = false;
                SensorPackage sensorspackage;
                Dictionary<SensorType, double[]> sensorsData = new Dictionary<SensorType, double[]>();
                

                internal FlightTelemetry(AscentProAPGCSModule module)
                {
                        this.module = module;
                        sensorspackage = new SensorPackage(module);

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
