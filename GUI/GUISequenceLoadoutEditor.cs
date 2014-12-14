using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUISequenceLoadoutEditor : MonoBehaviour
        {
                AscentProAPGCSModule module;

                int windowId = 98473;

                Rect WindowRect = new Rect(200, 100, 450, 600);

                string windowTitle;
                LoadoutType LoadoutType;


                Vector2 sequenceLeftScrollPosition;
                Vector2 sequenceRightScrollPosition;
                List<ControlType> sequenceLeftList = new List<ControlType>();
                List<ControlType> sequenceRightList = new List<ControlType>();

                Vector2 attitudeLeftScrollPosition;
                Vector2 attitudeRightScrollPosition;
                List<AttitudeControlType> attitudeLeftList = new List<AttitudeControlType>();
                List<AttitudeControlType> attitudeRightList = new List<AttitudeControlType>();

                Vector2 sensorLeftScrollPosition;
                Vector2 sensorRightScrollPosition;
                List<SensorType> sensorLeftList = new List<SensorType>();

                int maxAttitudeControllers = 1;

                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal GUISequenceLoadoutEditor()
                {
                        

                }

                internal void InitWindow(AscentProAPGCSModule module, LoadoutType loadouttype, string title)
                {
                        this.module = module;
                        this.LoadoutType = loadouttype;
                        this.windowTitle = title;

                        switch (loadouttype)
                        {
                                case LoadoutType.Control:

                                        LoadFromPartModule(module);
                                        EnumTypes();

                                        break;
                        }

                }

                void Start()
                {
                        labelStyle.alignment = TextAnchor.MiddleCenter;

                }

                void OnGUI()
                {
                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, windowTitle);
                }





                void DrawLoadoutEditor(int id)
                {

                        GUILayout.BeginVertical();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();

       
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.yellow;

                                                GUILayout.Label(LoadoutType + "s Available", labelStyle);

                                                GUILayout.Space(5);


                                                sequenceLeftScrollPosition = GUILayout.BeginScrollView(sequenceLeftScrollPosition);

                                                foreach (ControlType control in sequenceLeftList.ToList())
                                                {
                                                        if(GUILayout.Button(control.ToString()))
                                                        {
                                                                sequenceRightList.Add(control);
                                                                sequenceRightList.Sort();

                                                                sequenceLeftList.Remove(control);
                                                                sequenceLeftList.Sort();

                                                                AddControl(control);


                                                                

                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();


                                        GUILayout.EndVertical();


                        

                                        GUILayout.FlexibleSpace();

                                        GUILayout.BeginVertical();

                                                GUILayout.Space(70);
                                                labelStyle.normal.textColor = Color.cyan;
                                                GUILayout.Label(">", labelStyle);
                                                GUILayout.Space(70);
                                                GUILayout.Label("<", labelStyle);

                                        GUILayout.EndVertical();

                                        GUILayout.FlexibleSpace();
                                        
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.green;
                                                GUILayout.Label(LoadoutType+" Loadout", labelStyle);

                                                GUILayout.Space(5);

                                                sequenceRightScrollPosition = GUILayout.BeginScrollView(sequenceRightScrollPosition, false, false);

                                                foreach (ControlType control in sequenceRightList.ToList())
                                                {
                                                        if (GUILayout.Button(control.ToString()))
                                                        {
                                                                sequenceLeftList.Add(control);
                                                                sequenceLeftList.Sort();

                                                                sequenceRightList.Remove(control);
                                                                sequenceRightList.Sort();

                                                                RemoveControl(control);

                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        




                        if (GUILayout.Button("Save Loadout"))
                        {


                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUIControlLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }



                
                void LoadFromPartModule(AscentProAPGCSModule module)
                {
                        sequenceRightList = module.SequenceEngine.ControllerModules.Keys.ToList();
                        sequenceRightList.Sort();






                }

                void EnumTypes()
                {

                        foreach (ControlType control in (ControlType[])Enum.GetValues(typeof(ControlType)))
                        {
                                sequenceLeftList.Add(control);
                        }
                        sequenceLeftList = sequenceLeftList.Except(sequenceRightList).ToList();
                        
                        sequenceLeftList.Sort();



                        EnumAttitudeTypes();


                        EnumSensorTypes();



                }

                void EnumSensorTypes()
                {
                        sensorLeftList.Clear();
                        foreach (SensorType sensor in (SensorType[])Enum.GetValues(typeof(SensorType)))
                        {
                                sensorLeftList.Add(sensor);
                        }


                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                                sensorLeftList = sensorLeftList.Except(module.SequenceEngine.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>()).ToList();

                        sensorLeftList.Remove(SensorType.TIME);
                        sensorLeftList.Sort();
                
                }


                void EnumAttitudeTypes()
                {
                        attitudeLeftList.Clear();
                        attitudeRightList.Clear();
                        foreach (AttitudeControlType attitude in (AttitudeControlType[])Enum.GetValues(typeof(AttitudeControlType)))
                        {
                                attitudeLeftList.Add(attitude);
                        }

                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.ATTITUDE))
                        {
                                attitudeRightList.Add(module.SequenceEngine.ControllerModules[ControlType.ATTITUDE].GetLoadedTypes<AttitudeControlType>());
                                attitudeLeftList = attitudeLeftList.Except(attitudeRightList).ToList();
                        }

                        attitudeLeftList.Sort();
                
                }

                void AddControl(ControlType control)
                {
                        if (!module.SequenceEngine.ControllerModules.ContainsKey(control))
                        {
                                switch (control)
                                {
                                        case ControlType.ATTITUDE:
                                                //module.SequenceEngine.ControllerModules.Add(control, new ControlAttitude());
                                                EnumAttitudeTypes();
                                                break;
                                        case ControlType.SENSOR:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlSensors());
                                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(SensorType.TIME);  
                                                EnumSensorTypes();
                                                break;
                                        case ControlType.MISSIONLOG:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlMissionLog());
                                                break;
                                        case ControlType.TELEMETRY:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlTelemetry());
                                                break;
                                }
                        }



                }

                void RemoveControl(ControlType control)
                {
                        module.SequenceEngine.ControllerModules.Remove(control);
                }

                void AddAttitudeController(AttitudeControlType controller)
                {
                        switch (controller)
                        {
                                case AttitudeControlType.SAS:
                                        module.SequenceEngine.ControllerModules.Add(ControlType.ATTITUDE, new SASController());
                                        break;


                        }


                }

                void RemoveAttitudeController()
                {
                        module.SequenceEngine.ControllerModules.Remove(ControlType.ATTITUDE);

                }

                void AddSensor()
                { 
                
                }

                void RemoveSensor()
                { 
                
                }

                void SaveLoadout()
                {

                        /*
                        module.SequenceEngine.ControllerModules.Clear();

                        foreach(ControlType control in controlRightList)
                        {

                                module.SequenceEngine.AddControl(control);

                                        
                        }

                        */

                        
                
                }






        }
}
