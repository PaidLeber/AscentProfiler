using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ControlTelemetry
        {

                AscentProAPGCSModule module;
                SensorPackage sensorsSuite;

                internal bool Enabled = false;

                internal bool isMissionLogEnabled = true;
                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;

                internal bool sensorsEnabled = false;
                internal Dictionary<SensorType, List<double>> sensorsOnBoard = new Dictionary<SensorType, List<double>>();
                internal bool isSensorsDataReadyToTransmit = false;
                int sensorstep = 0;
                int sensorrate = 1; //per second
                double sensorlast;

                internal bool isConnectedtoKSC                                                                           //If RT loaded, get RT value, if no RT, always return true;
                {
                        get
                        {
                                if (AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech) && module.vessel.GetCrewCount() == 0)
                                {
                                        return RemoteTech.API.HasConnectionToKSC(module.vessel.id);
                                }
                                else
                                {
                                        return true;
                                }
                        }

                }

                internal ControlTelemetry(AscentProAPGCSModule module)
                {
                        this.module = module;
                        sensorsSuite = new SensorPackage(module);

                }

                internal void Transmit()
                {
                        if (!isConnectedtoKSC)
                                return;


                        if (isMissionLogEnabled && missionLog.Count > lastMissionLogTransmitCount)              // Send Mission Logs
                                if (AscentProfiler.telemetryReceiver.ReceiveMissionLog(TransitTimeUT(), missionLog))
                                        lastMissionLogTransmitCount = missionLog.Count;

                        if (isSensorsDataReadyToTransmit)
                                if (AscentProfiler.telemetryReceiver.ReceiveTelemetryData(TransitTimeUT(), sensorsOnBoard))                     // Send Telemetry Data
                                {
                                        sensorsEnabled = false;
                                        isSensorsDataReadyToTransmit = false;
                                        sensorsOnBoard.Clear();
                                }



                }

                double TransitTimeUT()
                {
                        if (!AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
                        { return 0; }
                        Debug.Log("UT: " + Planetarium.GetUniversalTime());
                        Debug.Log("Signal Delay: " + RemoteTech.API.GetSignalDelayToKSC(module.vessel.id));
                        return Planetarium.GetUniversalTime() + RemoteTech.API.GetSignalDelayToKSC(module.vessel.id);

                }

                internal bool AddSensor(SensorType sensor)
                {

                        sensorsOnBoard.Add(sensor, new List<double>{});
                        return true;
                }

                internal void ReadSensors()
                {
                        if (!sensorsEnabled || module.vessel.missionTime == 0)
                                return;

                        if (Planetarium.GetUniversalTime() > sensorlast + sensorrate)
                        {
                                foreach (SensorType sensor in sensorsOnBoard.Keys)
                                {
                                        sensorsOnBoard[sensor].Add(sensorsSuite.GetSensorData(sensor));

                                        Debug.Log( sensor.ToString() +": COUNT: "+  (sensorsOnBoard[sensor].Count - 1));
                                        Debug.Log(sensor.ToString() + ": VALUE: " + (sensorsOnBoard[sensor][sensorsOnBoard[sensor].Count - 1]));

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
