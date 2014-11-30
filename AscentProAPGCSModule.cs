using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        public class AscentProAPGCSModule : PartModule
        {
                internal FlightProfile flightProfile;
                internal FlightTelemetry flightTelemetry;

                //On RX Sequence of New Profile
                private List<object[]> listRXReceiverMessage = new List<object[]>();
                private bool isNewProfile = false;
                private double profileTransmissionTime = 0;
                private int profileMessageSequence = 0;


                private bool isConnectedtoKSC                                                                           //If RT loaded, get RT value, if no RT, always return true;
                {
                        get
                        {
                                if (AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech) && vessel.GetCrewCount() == 0)                  
                                {
                                        return RemoteTech.API.HasConnectionToKSC(vessel.id);
                                }
                                else
                                {
                                        return true;
                                }
                        }

                }

                public AscentProAPGCSModule()
                {
                        InitNewProfileSequence();
                        flightTelemetry = new FlightTelemetry(this); 
                        
                }

                private float lastUpdate = 0.0f;
                private float lastFixedUpdate = 0.0f;
                private float logInterval = 5.0f;

                internal void Test()
                {
                        Log.Level(LogType.Verbose, "AscentProAPGCSModule TEST1");
                }





                void Transmit(FlightTelemetry flightTelemetry)
                {
                        if (!isConnectedtoKSC)
                                return;


                        if (flightTelemetry.isMissionLogEnabled && flightTelemetry.missionLog.Count > flightTelemetry.lastMissionLogTransmitCount)              // Send Mission Logs
                                if (AscentProfiler.telemetryReceiver.ReceiveMissionLog(TransitTimeUT(), flightTelemetry.missionLog))
                                        flightTelemetry.lastMissionLogTransmitCount = flightTelemetry.missionLog.Count;


                        if (flightTelemetry.isSensorsDataReadyToTransmit)
                        {
                                if (AscentProfiler.telemetryReceiver.ReceiveTelemetryData(TransitTimeUT(), flightTelemetry.sensorsOnBoard))
                                {

                                }
                        }

                                

                }

                double TransitTimeUT()
                {
                        if (!AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
                                { return 0; }
                        Debug.Log("UT: " + Planetarium.GetUniversalTime());
                        Debug.Log("Signal Delay: " + RemoteTech.API.GetSignalDelayToKSC(vessel.id));
                        return Planetarium.GetUniversalTime() + RemoteTech.API.GetSignalDelayToKSC(vessel.id);
                
                }

                /*
* Called every frame
*/
                public override void OnUpdate()
                {
                        if ((Time.time - lastUpdate) > logInterval)
                        {
                                lastUpdate = Time.time;
                                Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                                    + "][" + Time.time.ToString("0.0000") + "]: OnUpdate");
                        }

                        if (flightProfile != null && flightProfile.isEnabled)
                        {
                                flightProfile.OnUpdate();
                        }

                        flightTelemetry.OnUpdate();
                        Transmit(flightTelemetry);

                }

                internal bool RXProfile(FlightProfile newprofile)
                {
                        Debug.Log("RX Profile successful");
                        //RXProfileReceiverSequence(newprofile);

                        flightProfile = newprofile;
                        flightProfile.AssignToModule(this);
                        flightProfile.isEnabled = true;
                        return isNewProfile = true;
                }

                void RXProfileReceiverSequence(FlightProfile newprofile)
                {
                        
                        profileMessageSequence = 0;
                        profileTransmissionTime = 0;


                                if (profileTransmissionTime == 0)
                                { profileTransmissionTime = Planetarium.GetUniversalTime(); }


                                if (profileMessageSequence == 5)
                                {
                                        isNewProfile = false;
                                }


                                var messagearray = listRXReceiverMessage[profileMessageSequence];

                                //Debug.Log("UT: " + Planetarium.GetUniversalTime() + " PTT: " + profileTransmissionTime + " PSD: " + profileSequenceDelay + " RTSD: " + RemoteTech.API.GetSignalDelayToKSC(vessel.id) + " PROFILE MESSAGESEQ: " + profileMessageSequence);
                                if (Planetarium.GetUniversalTime() > profileTransmissionTime + Convert.ToDouble(messagearray[1]) + RemoteTech.API.GetSignalDelayToKSC(vessel.id))
                                {
                                        if (profileMessageSequence == 3)
                                        {
                                                flightProfile = null;
                                                flightProfile.AssignToModule(this);
                                                flightProfile.isEnabled = true;
                                                Log.Level(LogType.Verbose, "Profile Loaded");
                                                Log.Level(LogType.Verbose, "this module enabled: " + this.isEnabled);
                                        }

                                        ScreenMessages.PostScreenMessage(new ScreenMessage(Convert.ToString(messagearray[2]), (float)messagearray[0], ScreenMessageStyle.UPPER_LEFT));
                                        profileMessageSequence++;

                                }

                }
                

                void InitNewProfileSequence()
                {

                        listRXReceiverMessage.Add(new object[] { 2.0f, 0, "RX: APGCS Telecommand Receiver Version " + AscentProfiler.version + " Ready" });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 2, "RX: Reconfiguration packets received from frame" });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 2, "RX: Checksum verification in progress, please standby " });
                        listRXReceiverMessage.Add(new object[] { 8.0f, 8, "RX: Reconfiguration successful: Profile loaded" });
                        listRXReceiverMessage.Add(new object[] { 12.0f, 2, "RX: APGCS Telecommand Receiver Version " + AscentProfiler.version + " Ready" });
                
                }

                /*
                * Called after the scene is loaded.
                */
                public override void OnAwake()
                {


                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnAwake: " + this.name);
                }

                /*
                 * Called after OnAwake.
                 */
                public override void OnStart(PartModule.StartState state)
                {

                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnStart: " + state);
                }

                /*
                 * Called when the part is activated/enabled. This usually occurs either when the craft
                 * is launched or when the stage containing the part is activated.
                 * You can activate your part manually by calling part.force_activate().
                 */
                public override void OnActive()
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnActive");
                }



                /*
                * Called at a fixed time interval determined by the physics time step.
                */
                public override void OnFixedUpdate()
                {


                        if ((Time.time - lastFixedUpdate) > logInterval)
                        {
                                lastFixedUpdate = Time.time;
                                Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                                    + "][" + Time.time.ToString("0.0000") + "]: OnFixedUpdate");
                        }
                }

                /*
                 * KSP adds the return value to the information box in the VAB/SPH.
                 */
                public override string GetInfo()
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: GetInfo");
                        return "\nContains the TAC Example - Simple Part Module\n";
                }

                /*
                 * Called when the part is deactivated. Usually because it was destroyed.
                 */
                public override void OnInactive()
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnInactive");
                }

                /*
                 * Called when the game is loading the part information. It comes from: the part's cfg file,
                 * the .craft file, the persistence file, or the quicksave file.
                 */
                public override void OnLoad(ConfigNode node)
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnLoad: " + node);
                }

                /*
                 * Called when the game is saving the part information.
                 */
                public override void OnSave(ConfigNode node)
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnSave: " + node);
                }


        }


}
