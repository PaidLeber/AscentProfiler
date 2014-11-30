using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class FlightTelemetry
        {

                AscentProAPGCSModule module;
                SensorPackage sensorsSuite;

                internal bool isMissionLogEnabled = true;
                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;

                internal bool isSensorsEnabled = false;
                internal Dictionary<SensorType, double[]> sensorsOnBoard = new Dictionary<SensorType, double[]>();
                internal bool isSensorsDataReadyToTransmit = false;
                int sensorstep = 0;
                

                internal FlightTelemetry(AscentProAPGCSModule module)
                {
                        this.module = module;
                        sensorsSuite = new SensorPackage(module);

                }
                
                internal bool AddSensor(SensorType sensor)
                {

                        sensorsOnBoard.Add(sensor, new double[]{});
                        return true;
                }

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
                        if (!isMissionLogEnabled)
                                { return; }

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
