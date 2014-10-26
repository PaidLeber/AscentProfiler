using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        enum APGCSTransmit
        {
                FLIGHT,
                TELEMETRY
        }

        struct APGCSDataPacket
        {
                Guid source;
                APGCSTransmit destination;
                double transitime;
                int datacount;
                object data;

                internal APGCSDataPacket(Guid source, APGCSTransmit destination, double transitime, int datacount, object data)
                {
                        this.source = source;
                        this.destination = destination;
                        this.transitime = transitime;
                        this.datacount = datacount;
                        this.data = data;
                }


        }

        public class AscentProAPGCSModule : PartModule
        {
                FlightProfile flightProfile;
                internal FlightRecorder flightRecorder;

                private List<string> listRXReceiverMessage = new List<string>();
                private System.Random RandGenerator = new System.Random();
                private int rxproRand = 0;
                private double rxproRcvd = 0;
                private int rxproSequence = 0;



                private float lastUpdate = 0.0f;
                private float lastFixedUpdate = 0.0f;
                private float logInterval = 5.0f;



                private bool isConnectedtoKSC
                {
                        get
                        {
                                if (AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
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

                internal bool RXProfile(FlightProfile newprofile)
                {
                        
                        flightProfile = newprofile;
                        flightProfile.AssignToModule(this);

                        Log.Level(LogType.Verbose, "Profile loaded");

                        return true;
                }

                void TXToMissionLog()
                { 
                  
                                if(flightRecorder.Log.Any())
                                {
                                        APGCSDataPacket packet = new APGCSDataPacket(
                                                vessel.id, APGCSTransmit.FLIGHT, 
                                                RemoteTech.API.GetSignalDelayToKSC(vessel.id) + vessel.missionTime, 
                                                flightRecorder.Log.Count(), 
                                                flightRecorder.Log
                                                );
                                        
                                        //if transmit true, flightRecorder.Log.Clear();
                                }


                }


                void RXProfileReceiverSequence()
                {
                        if (rxproRcvd == 0) { rxproRcvd = vessel.missionTime;}

                        if ( vessel.missionTime > rxproRcvd + rxproRand + RemoteTech.API.GetSignalDelayToKSC(vessel.id) )
                        {
                                ScreenMessages.PostScreenMessage(new ScreenMessage(listRXReceiverMessage[rxproSequence], 4.0f, ScreenMessageStyle.UPPER_LEFT));

                                switch (rxproSequence)
                                {
                                        case 0:
                                                rxproRand = RandGenerator.Next(2, 9);
                                                break;
                                        case 1:
                                                rxproRand = 0;
                                                break;
                                        case 2:
                                                rxproRand = RandGenerator.Next(7, 15);
                                                break;
                                        case 3:
                                                rxproRand = 0;
                                                break;
                                        case 4:
                                                rxproRand = RandGenerator.Next(4, 7);
                                                break;
                                        case 5:
                                                rxproRand = 10;
                                                break;
                                        case 6:
                                                rxproRand = 0;
                                                break;
                                        case 7:
                                                rxproRand = 0;
                                                break;
                                }
                                
                                rxproSequence++;
                        }




                }

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
                        if(isConnectedtoKSC)
                        {
                                TXToMissionLog();
                        }


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

                void InitNewProfileSequence()
                {
                        listRXReceiverMessage.Add("RX: APGCS Receiver Version " + AscentProfiler.version +" Ready");
                        listRXReceiverMessage.Add("RX: Reconfiguration packets received");
                        listRXReceiverMessage.Add("RX: Checksum verification in progress, please standby ");
                        listRXReceiverMessage.Add("RX: Reconfiguration successful: Profile loaded");
                        listRXReceiverMessage.Add("RX: Flashing to failsafe ROM");
                        listRXReceiverMessage.Add("RX: Rebooting APGCS failsafe...");
                        listRXReceiverMessage.Add("RX: I feel fine. How about you?");
                        listRXReceiverMessage.Add("RX: APGCS Receiver Version " + AscentProfiler.version + " Ready");

                
                }

        }


}
