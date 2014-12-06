using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        public class AscentProAPGCSModule : PartModule
        {
                private Vessel focusedVessel;

                internal AttitudeController attitude;
                internal SequenceEngine flightSequence;
                internal FlightTelemetry flightTelemetry;

                //On RX Sequence of New Sequence
                private List<object[]> listRXReceiverMessage = new List<object[]>();
                private SequenceEngine newSequence;
                private double sequenceTransmissionTime = 0;
                private int sequenceMessageOrder = 0;


                internal bool isConnectedtoKSC                                                                           //If RT loaded, get RT value, if no RT, always return true;
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
                        InitNewSequenceMessage();
                        flightTelemetry = new FlightTelemetry(this); 
                        
                }

                void Transmit(FlightTelemetry flightTelemetry)
                {
                        if (!isConnectedtoKSC)                                                                                                                  
                                return;

                        
                        if (flightTelemetry.isMissionLogEnabled && flightTelemetry.missionLog.Count > flightTelemetry.lastMissionLogTransmitCount)              // Send Mission Logs
                                if (AscentProfiler.telemetryReceiver.ReceiveMissionLog(TransitTimeUT(), flightTelemetry.missionLog))
                                        flightTelemetry.lastMissionLogTransmitCount = flightTelemetry.missionLog.Count;

                        if (flightTelemetry.isSensorsDataReadyToTransmit)
                                if (AscentProfiler.telemetryReceiver.ReceiveTelemetryData(TransitTimeUT(), flightTelemetry.sensorsOnBoard))                     // Send Telemetry Data
                                {
                                        flightTelemetry.isSensorsEnabled = false;
                                        flightTelemetry.isSensorsDataReadyToTransmit = false;
                                        flightTelemetry.sensorsOnBoard.Clear();
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

                bool IsFocusedVessel()
                {
                        if (vessel == focusedVessel)
                                return true;

                        if (focusedVessel != null) focusedVessel.OnFlyByWire -= ActiveController;


                        return false;
                }

                void ActiveController(FlightCtrlState s)
                { 
                
                }

                /*
* Called every frame
*/

                private float lastUpdate = 0.0f;
                private float lastFixedUpdate = 0.0f;
                private float logInterval = 5.0f;

                public override void OnUpdate()
                {
                        if ((Time.time - lastUpdate) > logInterval)
                        {
                                lastUpdate = Time.time;
                                Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                                    + "][" + Time.time.ToString("0.0000") + "]: OnUpdate");
                        }

                        if(newSequence != null)
                        {
                                RXSequenceReceiver();
                        }

                        if (flightSequence != null && flightSequence.isEnabled)
                        {
                                flightSequence.OnUpdate();
                        }

                        flightTelemetry.OnUpdate();
                        Transmit(flightTelemetry);

                }

                internal bool RXNewSequence(SequenceEngine newsequence)
                {
                        if (AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
                        {
                                sequenceMessageOrder = 0;
                                sequenceTransmissionTime = 0;
                                newSequence = newsequence;
                                Debug.Log("RX Sequence Transfer successful");
                        }
                        else
                        {
                                LoadNewSequence(newsequence);

                        }


                        
                        return true;
                }

                void LoadNewSequence(SequenceEngine newsequence)
                {
                        flightTelemetry.sensorsOnBoard.Clear();
                        flightSequence = newsequence;
                        flightSequence.AssignToModule(this);
                        flightSequence.isEnabled = true;
                        flightSequence.ExecuteActions(0);

                }

                void RXSequenceReceiver()
                {
                        
                                if (sequenceTransmissionTime == 0)
                                { sequenceTransmissionTime = Planetarium.GetUniversalTime(); }


                                if (sequenceMessageOrder == 6)
                                {
                                        newSequence = null;
                                        return;
                                }


                                var messagearray = listRXReceiverMessage[sequenceMessageOrder];

                                
                                if (Planetarium.GetUniversalTime() > sequenceTransmissionTime + Convert.ToDouble(messagearray[1]) + RemoteTech.API.GetSignalDelayToKSC(vessel.id))
                                {
                                        Debug.Log("sequence#: "+sequenceMessageOrder+" seqdelaytime: "+messagearray[1]);
                                        if (sequenceMessageOrder == 4)
                                        {
                                                LoadNewSequence(newSequence);
                                                flightTelemetry.AddLog("Reconfiguration successful: sequence loaded");
                                                Log.Level(LogType.Verbose, "Sequence Loaded");
                                                Log.Level(LogType.Verbose, "this module enabled: " + this.isEnabled);
                                        }

                                        ScreenMessages.PostScreenMessage(new ScreenMessage(Convert.ToString(messagearray[2]), (float)messagearray[0], ScreenMessageStyle.UPPER_LEFT));
                                        sequenceTransmissionTime = Planetarium.GetUniversalTime();
                                        sequenceMessageOrder++;

                                }
                        
                }
                

                void InitNewSequenceMessage()
                {
                        
                        listRXReceiverMessage.Add(new object[] { 4.0f, 0, "RX: APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });
                        listRXReceiverMessage.Add(new object[] { 3.0f, 1, "RX: Reconfiguration packets received from frame" });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 4, "RX: Checksum verification in progress, please standby " });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 6, "RX: Telemetry data buffer: Reset" });
                        listRXReceiverMessage.Add(new object[] { 8.0f, 4, "RX: Reconfiguration successful: sequence loaded" });
                        listRXReceiverMessage.Add(new object[] { 7.0f, 2, "RX: APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });
                
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
