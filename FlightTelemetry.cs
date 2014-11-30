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

                bool isSensorsDataReadyToTransmit = false;
                bool isSensorsEnabled = false;
                SensorPackage sensorsSuite;
                Dictionary<SensorType, double[]> sensorsOnBoard = new Dictionary<SensorType, double[]>();
                int sensorstep = 0;
                

                internal FlightTelemetry(AscentProAPGCSModule module)
                {
                        this.module = module;
                        sensorsSuite = new SensorPackage(module);

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
                //

                internal void OnUpdate()
                {
                        if (!isSensorsEnabled || module.vessel.missionTime == 0)
                                {return;}

                        foreach (SensorType sensor in sensorsOnBoard.Keys )
                        {
                                sensorsOnBoard[sensor][sensorstep] = sensorsSuite.GetSensorData(sensor);
                                
                        }

                        sensorstep++;
                
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
