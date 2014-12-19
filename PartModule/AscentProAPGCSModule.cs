using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using UnityEngine;


namespace AscentProfiler
{
        
        public class AscentProAPGCSModule : PartModule
        {
                
                internal Sequence SequenceEngine;

                private GUISequenceLoadoutEditor sequenceWindow;
                private GUIControlLoadoutEditor controlWindow;

                public AscentProAPGCSModule()
                {

                }
                
                [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "SUID")]
                public string SUID = "lander";

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Sequence")]
                public string activeSequence = "";

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Sequence Mode")]
                public string modeSeq = "";                                                                           // Active, Inactive, Hibernation

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Change Module ID")]
                public void GUIDChange()
                {

                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Sequence Block(s)")]
                public void ModifySequenceLoadout()
                {
                        sequenceWindow = gameObject.AddComponent<GUISequenceLoadoutEditor>();
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
                        Debug.Log("APGCSModule OnStart");

                        if(SequenceEngine == null)
                                SequenceEngine = new Sequence();


                }

                /*
 * Called when the game is loading the part information. It comes from: the part's cfg file,
 * the .craft file, the persistence file, or the quicksave file.
 */


                public override void OnLoad(ConfigNode node)
                {
                        Debug.Log("OnLoad Loading APGCSModule...");



                        if (node.HasValue("SequenceEngine"))
                        {
                                SequenceEngine = (Sequence)Serializer.DeserializeFromString(node.GetValue("SequenceEngine"));
                        }
                        else
                        {
                                Debug.Log("No Sequence found for command part "+part.name);
                        }

                        

                }

                public override void OnSave(ConfigNode node)
                {
                        Debug.Log("Saving APGCSModule... ");

                        try
                        {
                                if(SequenceEngine != null)
                                {
                                        node.AddValue("SequenceEngine", Serializer.SerializeToString(SequenceEngine) );
                                }

                        }
                        catch (Exception e)
                        {
                                Debug.Log("Unable to save APGCSModule state: " + e.Message + " at " + e.StackTrace);
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





                        if (SequenceEngine != null)
                        {
                                SequenceEngine.Process(this);
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
                        //if (flightController != null) flightController.isEngaged = false;

                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnInactive");
                }



                /*
                 * Called when the game is saving the part information.
                 */
                /*
                public override void OnSave(ConfigNode node)
                {
                        Debug.Log("TAC Examples-SimplePartModule [" + this.GetInstanceID().ToString("X")
                            + "][" + Time.time.ToString("0.0000") + "]: OnSave: " + node);
                }
                 
                 */


        }


}
