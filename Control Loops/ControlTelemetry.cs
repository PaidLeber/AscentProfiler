using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ControlTelemetry : ControlModule
        {

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









        }
}
