using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using UnityEngine;


namespace AscentProfiler
{
        [Serializable]
        public class AscentProAPGCSModule : PartModule
        {
                
                internal Sequence SequenceEngine;

                private GUISensorLoadoutEditor sequenceWindow;
                private GUISensorLoadoutEditor controllerWindow;
                private GUISensorLoadoutEditor sensorWindow;

                public AscentProAPGCSModule()
                {

                }

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Module ID")]
                public string moduleID = "";

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Sequence")]
                public string activeSequence = "";

                [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Mode")]
                public string modeSeq = "";                                                                           // Active, Inactive, Hibernation

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Change Module ID")]
                public void GUIDChange()
                {

                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Sequence(s)")]
                public void ModifySequenceLoadout()
                {

                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Controller(s)")]
                public void ModifyControllerLoadout()
                {
                        /*
                        string base64;

                        using (MemoryStream ms = new MemoryStream())
                        {
                                BinaryFormatter f = new BinaryFormatter();

                                using (DeflateStream gz = new DeflateStream(ms, CompressionMode.Compress))
                                {
                                        f.Serialize(gz, SequenceEngine);
                                }

                               base64 = Convert.ToBase64String(ms.ToArray()).Replace('/', '_');
                        }

                        SequenceEngine = null;

                        base64 = base64.Replace('_', '/');
                        byte[] data = Convert.FromBase64String(base64);
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                                BinaryFormatter f = new BinaryFormatter();

                                using (DeflateStream gz = new DeflateStream(ms, CompressionMode.Decompress))
                                {
                                        SequenceEngine = (Sequence)f.Deserialize(gz);
                                }

                        }

                        */
                }

                [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Add Sensor(s)")]
                public void ModifySensorLoadout()
                {
                        sensorWindow = gameObject.AddComponent<GUISensorLoadoutEditor>();
                        sensorWindow.InitWindow(this, LoadoutType.Sensor, "Sensor Loadout Window");
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
                public static MemoryStream SerializeToStream(object o)
                {   
                        MemoryStream stream = new MemoryStream();
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, o);
                        return stream;

                }

                public static object DeserializeFromStream(MemoryStream stream)
                {

                        IFormatter formatter = new BinaryFormatter();
                        stream.Seek(0, SeekOrigin.Begin);
                        object o = formatter.Deserialize(stream);
                        return o;

                }

                public override void OnLoad(ConfigNode node)
                {
                        Debug.Log("OnLoad Loading APGCSModule...");



                        if (node.HasValue("SequenceEngine"))
                        {

                                string base64 = node.GetValue("SequenceEngine");

                                byte[] data = Convert.FromBase64String(base64);
                                using (MemoryStream ms = new MemoryStream(data))
                                {
                                        BinaryFormatter f = new BinaryFormatter();

                                        using (DeflateStream gz = new DeflateStream(ms, CompressionMode.Decompress))
                                        {
                                                SequenceEngine = null;
                                                SequenceEngine = (Sequence)f.Deserialize(gz);
                                        }

                                }
                        
                        
                        }

                        

                }

                public override void OnSave(ConfigNode node)
                {
                        Debug.Log("Saving APGCSModule... ");


                        try
                        {
                                if (SequenceEngine != null)
                                {

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                                BinaryFormatter f = new BinaryFormatter();

                                                using (DeflateStream gz = new DeflateStream(ms, CompressionMode.Compress))
                                                {
                                                        f.Serialize(gz, SequenceEngine);
                                                }

                                                string base64 = Convert.ToBase64String(ms.ToArray());


                                                node.AddValue("SequenceEngine", base64);
                                        }


                                }



                        }
                        catch (Exception e)
                        {
                                Debug.Log("Unable to save APGCSModule state: " + e.Message + " at " + e.StackTrace);
                        }


                        /*

                        try
                        {
                                if (SequenceEngine != null)
                                {

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                                BinaryFormatter f = new BinaryFormatter();

                                                using (DeflateStream gz = new DeflateStream(ms, CompressionMode.Compress))
                                                {
                                                        f.Serialize(gz, SequenceEngine);
                                                }

                                                string base64 = Convert.ToBase64String(ms.ToArray()).Replace('/', '_');

                                                node.AddValue("SequenceEngine", base64);
                                        }
                                }



                        }
                        catch (Exception e)
                        {
                                Debug.Log("Unable to save APGCSModule state: " + e.Message + " at " + e.StackTrace);
                        }
                         * 
                         * */

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
