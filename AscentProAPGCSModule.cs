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

                private UnityEngine.Random random = new UnityEngine.Random(); //int randomNumber = random.Next(0, 100);
                private double rxprofilercvd = 0;
                private int rxsequencenum = 0;
                
                



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
                        if (rxprofilercvd == 0)
                        {
                                rxprofilercvd = vessel.missionTime;
                        }



                        if( rxprofilercvd + RemoteTech.API.GetSignalDelayToKSC(vessel.id) > vessel.missionTime && rxsequencenum == 0 )
                        {
                                
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
                        flightRecorder = new FlightRecorder(this); 

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

        }


}
