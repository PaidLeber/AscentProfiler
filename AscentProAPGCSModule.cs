﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        public class AscentProAPGCSModule : PartModule
        {
                FlightProfile flightProfile = null;
                internal FlightRecorder flightRecorder = null;

                //On RX Sequence of New Profile
                private List<string> listRXReceiverMessage = new List<string>();
                private System.Random RandGenerator = new System.Random();

                private bool isNewProfile = false;
                private double profileSequenceDelay = 0;
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
                        flightRecorder = new FlightRecorder(this); 
                        
                }

                private float lastUpdate = 0.0f;
                private float lastFixedUpdate = 0.0f;
                private float logInterval = 5.0f;

                internal void Test()
                {
                        Log.Level(LogType.Verbose, "AscentProAPGCSModule TEST1");
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
                 * Called every frame
                 */
                public override void OnUpdate()
                {
                        RXProfileReceiverSequence();
                        Transmit(flightRecorder);


                        if ((Time.time - lastUpdate) > logInterval)
                        {
                                lastUpdate = Time.time;
                                Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                                    + "][" + Time.time.ToString("0.0000") + "]: OnUpdate");
                        }
                }

                /*
                 * Called at a fixed time interval determined by the physics time step.
                 */
                public override void OnFixedUpdate()
                {
                        flightProfile.TriggerLoop();

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


                internal bool RXProfile(FlightProfile newprofile)
                {
                        flightProfile = newprofile;
                        flightProfile.isEnabled = true;
                        Debug.Log("RX Profile successful");
                        return isNewProfile = true;
                }

                void Transmit(FlightRecorder flightrecorder)
                {
                        if(isConnectedtoKSC)
                                if (flightrecorder.logEnabled && flightrecorder.FlightLog.Count > flightrecorder.lastFlightLogTransmitCount)
                                        if (AscentProfiler.telemetryReceiver.ReceiveFlightLog(TransitTimeUT(), flightrecorder))
                                                flightrecorder.lastFlightLogTransmitCount = flightrecorder.FlightLog.Count;

                }

                double TransitTimeUT()
                {
                        if (!AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
                                { return 0; }
                        Debug.Log("UT: " + Planetarium.GetUniversalTime());
                        Debug.Log("Signal Delay: " + RemoteTech.API.GetSignalDelayToKSC(vessel.id));
                        return Planetarium.GetUniversalTime() + RemoteTech.API.GetSignalDelayToKSC(vessel.id);
                
                }

                void RXProfileReceiverSequence()
                {
                        if (!isNewProfile)
                                { return; }

                        if (profileTransmissionTime == 0) 
                                { profileTransmissionTime = vessel.missionTime;}

                        if ( vessel.missionTime > profileTransmissionTime + profileSequenceDelay + RemoteTech.API.GetSignalDelayToKSC(vessel.id) )
                        {
                                ScreenMessages.PostScreenMessage(new ScreenMessage(listRXReceiverMessage[profileMessageSequence], 4.0f, ScreenMessageStyle.UPPER_LEFT));

                                switch (profileMessageSequence)
                                {
                                        case 0:
                                                profileSequenceDelay = RandGenerator.Next(2, 9);
                                                break;
                                        case 1:
                                                profileSequenceDelay = 0;
                                                break;
                                        case 2:
                                                profileSequenceDelay = RandGenerator.Next(3, 15);
                                                break;
                                        case 3:
                                                flightProfile.AssignToModule(this);
                                                flightProfile.isEnabled = true;
                                                Log.Level(LogType.Verbose, "Profile Loaded");
                                                profileSequenceDelay = 0;
                                                break;
                                        case 4:
                                                profileSequenceDelay = RandGenerator.Next(4, 7);
                                                break;
                                        case 5:
                                                profileSequenceDelay = 10;
                                                break;
                                        case 6:
                                                profileSequenceDelay = 0;
                                                break;
                                        case 7:
                                                isNewProfile = false;
                                                profileSequenceDelay = 0;
                                                profileMessageSequence = 0;
                                                profileTransmissionTime = 0;
                                                return;
                                }
                                

                                profileMessageSequence++;
                        }




                }



                void InitNewProfileSequence()
                {
                        listRXReceiverMessage.Add("RX: APGCS Telecommand Receiver Version " + AscentProfiler.version +" Ready");
                        listRXReceiverMessage.Add("RX: Reconfiguration packets received from frame");
                        listRXReceiverMessage.Add("RX: Checksum verification in progress, please standby ");
                        listRXReceiverMessage.Add("RX: Reconfiguration successful: Profile loaded");
                        listRXReceiverMessage.Add("RX: Flashing to failsafe EEPROM device");
                        listRXReceiverMessage.Add("RX: Rebooting APGCS failsafe...");
                        listRXReceiverMessage.Add("RX: I feel fine. How about you?");
                        listRXReceiverMessage.Add("RX: APGCS Telecommand Receiver Version " + AscentProfiler.version + " Ready");

                
                }

        }


}
