using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
                internal Dictionary<SensorType, List<double>> sensorsOnBoard = new Dictionary<SensorType, List<double>>();
                internal bool isSensorsDataReadyToTransmit = false;
                int sensorstep = 0;
                int sensorrate = 1; //per second
                double sensorlast;
                

                internal FlightTelemetry(AscentProAPGCSModule module)
                {
                        this.module = module;
                        sensorsSuite = new SensorPackage(module);

                }
                
                internal bool AddSensor(SensorType sensor)
                {

                        sensorsOnBoard.Add(sensor, new List<double>{});
                        return true;
                }

                internal void OnUpdate()
                {
                        if (!isSensorsEnabled || module.vessel.missionTime == 0)
                                {return;}

                        if (Planetarium.GetUniversalTime() > sensorlast + sensorrate)
                        {
                                foreach (SensorType sensor in sensorsOnBoard.Keys)
                                {
                                        sensorsOnBoard[sensor].Add(sensorsSuite.GetSensorData(sensor));

                                        Debug.Log( sensor.ToString() +": COUNT: "+  sensorsOnBoard[sensor][sensorsOnBoard[sensor].Count - 1]);


                                }
                                
                                sensorlast = Planetarium.GetUniversalTime();
                                sensorstep++;
                        }



                
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
