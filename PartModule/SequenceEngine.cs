using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using UnityEngine;


namespace AscentProfiler
{
        
        public class SequenceEngine : PartModule
        {
                
                internal Sequence Sequencer;
                
                private GUIGroundStationTerminal sequenceWindow;
                private GUIControlLoadoutEditor controlWindow;

                public SequenceEngine()
                {

                }
                
                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Sequence UID")]
                public string SUID = "";

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Active Sequence")]
                public string ActiveSequence = "";

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "test button")]
                public void TestEvent()
                {
                        ActiveSequence =  this.name;

                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Sequence(s)")]
                public void ModifySequenceLoadout()
                {
                        sequenceWindow = gameObject.AddComponent<GUIGroundStationTerminal>();
                        sequenceWindow.InitWindow(this);
                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Controller(s)")]
                public void ModifyControllerLoadout()
                {
                        controlWindow = gameObject.AddComponent<GUIControlLoadoutEditor>();
                        controlWindow.InitWindow(this);
                        
                }


                /*
 * Called after OnAwake.
 */
                public override void OnStart(PartModule.StartState state)
                {
                        Debug.Log("AEDL Systems Module OnStart");

                        if (HighLogic.LoadedSceneIsEditor && string.IsNullOrEmpty(SUID))
                        {
                                SUID = this.part.name.Replace("(Clone)", AscentProfilerVAB.GetSUID);

                        }

                        if(Sequencer == null)
                                Sequencer = new Sequence();




                }

                /*
 * Called when the game is loading the part information. It comes from: the part's cfg file,
 * the .craft file, the persistence file, or the quicksave file.
 */


                public override void OnLoad(ConfigNode node)
                {
                        Debug.Log("OnLoad Loading AEDL Systems Module...");



                        if (node.HasValue("SequenceEngine"))
                        {
                                Sequencer = (Sequence)Serializer.DeserializeFromString(node.GetValue("SequenceEngine"));
                        }
                        else
                        {
                                Debug.Log("No Sequence found for command part "+part.name);
                        }

                        

                }

                public override void OnSave(ConfigNode node)
                {
                        Debug.Log("Saving AEDL Systems Module... ");

                        try
                        {
                                if(Sequencer != null)
                                {
                                        node.AddValue("SequenceEngine", Serializer.SerializeToString(Sequencer) );
                                }

                        }
                        catch (Exception e)
                        {
                                Debug.Log("Unable to save AEDL Systems Module state: " + e.Message + " at " + e.StackTrace);
                        }



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





                        if (Sequencer != null)
                        {
                                Sequencer.Process(this);
                        }
                                


                                

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






        }


}
