using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUIControlLoadoutEditor : MonoBehaviour
        {
                AscentProAPGCSModule module;

                int windowId = 98473;

                Rect WindowRect = new Rect(200, 100, 450, 600);

                string windowTitle;
                LoadoutType LoadoutType;


                Vector2 controlLeftScrollPosition;
                Vector2 controlRightScrollPosition;
                List<ControlType> controlLeftList = new List<ControlType>();
                List<ControlType> controlRightList = new List<ControlType>();

                Vector2 attitudeLeftScrollPosition;
                Vector2 attitudeRightScrollPosition;
                List<AttitudeControlType> attitudeLeftList = new List<AttitudeControlType>();
                List<AttitudeControlType> attitudeRightList = new List<AttitudeControlType>();

                Vector2 sensorLeftScrollPosition;
                Vector2 sensorRightScrollPosition;
                List<SensorType> sensorLeftList = new List<SensorType>();
                List<SensorType> sensorRightList = new List<SensorType>();

                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal GUIControlLoadoutEditor()
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

                                        InitController(module);
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


                                                controlLeftScrollPosition = GUILayout.BeginScrollView(controlLeftScrollPosition);

                                                foreach (ControlType control in controlLeftList.ToList())
                                                {
                                                        if(GUILayout.Button(control.ToString()))
                                                        {
                                                                controlRightList.Add(control);
                                                                controlRightList.Sort();

                                                                controlLeftList.Remove(control);
                                                                controlLeftList.Sort();

                                                                
                                                                if(control == ControlType.SENSOR)
                                                                {

                                                                        if (!module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                                                                        {
                                                                                module.SequenceEngine.ControllerModules.Add(ControlType.SENSOR, new ControlSensors());

                                                                        }

                                                                        sensorRightList = module.SequenceEngine.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>();
                                                                        sensorRightList.Remove(SensorType.TIME);
                                                                        sensorRightList.Sort();

                                                                        foreach (SensorType sensor in (SensorType[])Enum.GetValues(typeof(SensorType)))
                                                                        {
                                                                                sensorLeftList.Add(sensor);
                                                                        }
                                                                        sensorLeftList.Remove(SensorType.TIME);
                                                                        sensorLeftList = sensorLeftList.Except(sensorRightList).ToList();

                                                                        sensorLeftList.Sort();
                                                                }

                                                                if (control == ControlType.ATTITUDE)
                                                                {
                                                                        if (!module.SequenceEngine.ControllerModules.ContainsKey(ControlType.ATTITUDE))
                                                                        {
                                                                                module.SequenceEngine.ControllerModules.Add(ControlType.ATTITUDE, new ControlAttitude());


                                                                        }
                                                                }



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

                                                controlRightScrollPosition = GUILayout.BeginScrollView(controlRightScrollPosition, false, false);

                                                foreach (ControlType control in controlRightList.ToList())
                                                {
                                                        if (GUILayout.Button(control.ToString()))
                                                        {
                                                                controlLeftList.Add(control);
                                                                controlLeftList.Sort();

                                                                controlRightList.Remove(control);
                                                                controlRightList.Sort();

                                                                if (control == ControlType.SENSOR)
                                                                {
                                                                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                                                                        {
                                                                                module.SequenceEngine.ControllerModules.Remove(ControlType.SENSOR);


                                                                        }
                                                                }

                                                                if (control == ControlType.ATTITUDE)
                                                                {
                                                                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.ATTITUDE))
                                                                        {
                                                                                module.SequenceEngine.ControllerModules.Remove(ControlType.ATTITUDE);


                                                                        }
                                                                }

                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        
                        if (controlRightList.Contains(ControlType.ATTITUDE))
                        {
                                GUILayout.BeginVertical();
                                GUILayout.Label("ATTITUDE", labelStyle);
                                GUILayout.EndVertical();

                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.yellow;

                                GUILayout.Label("Controllers Available", labelStyle);

                                GUILayout.Space(5);


                                attitudeLeftScrollPosition = GUILayout.BeginScrollView(attitudeLeftScrollPosition);

                                foreach (AttitudeControlType attitudecontroller in attitudeLeftList.ToList())
                                {
                                        if (GUILayout.Button(attitudecontroller.ToString()))
                                        {
                                                attitudeRightList.Add(attitudecontroller);
                                                attitudeRightList.Sort();

                                                attitudeLeftList.Remove(attitudecontroller);
                                                attitudeLeftList.Sort();


                                        }

                                }

                                GUILayout.EndScrollView();


                                GUILayout.EndVertical();




                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical();

                                GUILayout.Space(1);
                                labelStyle.normal.textColor = Color.cyan;
                                GUILayout.Label(">", labelStyle);
                                GUILayout.Space(1);
                                GUILayout.Label("<", labelStyle);

                                GUILayout.EndVertical();

                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.green;
                                GUILayout.Label("Controllers On Board", labelStyle);

                                GUILayout.Space(5);

                                attitudeRightScrollPosition = GUILayout.BeginScrollView(attitudeRightScrollPosition, false, false);

                                foreach (AttitudeControlType attitudecontroller in attitudeRightList.ToList())
                                {
                                        if (GUILayout.Button(attitudecontroller.ToString()))
                                        {
                                                attitudeLeftList.Add(attitudecontroller);
                                                attitudeLeftList.Sort();

                                                attitudeRightList.Remove(attitudecontroller);
                                                attitudeRightList.Sort();


                                        }

                                }

                                GUILayout.EndScrollView();

                                GUILayout.Space(5);



                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                        }



                        if (controlRightList.Contains(ControlType.SENSOR))
                        {
                                GUILayout.BeginVertical();
                                GUILayout.Label("SENSORS", labelStyle);
                                GUILayout.EndVertical();

                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.yellow;

                                GUILayout.Label("Sensors Available", labelStyle);

                                GUILayout.Space(5);


                                sensorLeftScrollPosition = GUILayout.BeginScrollView(sensorLeftScrollPosition);

                                foreach (SensorType sensor in sensorLeftList.ToList())
                                {
                                        if (GUILayout.Button(sensor.ToString()))
                                        {
                                                sensorRightList.Add(sensor);
                                                sensorRightList.Sort();

                                                sensorLeftList.Remove(sensor);
                                                sensorLeftList.Sort();


                                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(sensor);


                                        }

                                }

                                GUILayout.EndScrollView();


                                GUILayout.EndVertical();




                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical();

                                GUILayout.Space(1);
                                labelStyle.normal.textColor = Color.cyan;
                                GUILayout.Label(">", labelStyle);
                                GUILayout.Space(1);
                                GUILayout.Label("<", labelStyle);

                                GUILayout.EndVertical();

                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.green;
                                GUILayout.Label("Sensors On Board", labelStyle);

                                GUILayout.Space(5);

                                sensorRightScrollPosition = GUILayout.BeginScrollView(sensorRightScrollPosition, false, false);

                                foreach (SensorType sensor in sensorRightList.ToList())
                                {
                                        if (GUILayout.Button(sensor.ToString()))
                                        {
                                                sensorLeftList.Add(sensor);
                                                sensorLeftList.Sort();

                                                sensorRightList.Remove(sensor);
                                                sensorRightList.Sort();

                                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].RemoveType<SensorType>(sensor);

                                        }

                                }

                                GUILayout.EndScrollView();

                                GUILayout.Space(5);



                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                        }



                        if (GUILayout.Button("Save Loadout"))
                        {

                                if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                                {
                                        module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(SensorType.TIME);  

                                }

                                //SaveLoadout();

                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUIControlLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }





                void InitController(AscentProAPGCSModule module)
                {


                }
                
                void LoadFromPartModule(AscentProAPGCSModule module)
                {
                        controlRightList = module.SequenceEngine.ControllerModules.Keys.ToList();
                        controlRightList.Sort();

                        

                }

                void EnumTypes()
                {

                        foreach (ControlType control in (ControlType[])Enum.GetValues(typeof(ControlType)))
                        {
                                controlLeftList.Add(control);
                        }
                        controlLeftList = controlLeftList.Except(controlRightList).ToList();
                        
                        controlLeftList.Sort();

                        foreach (AttitudeControlType attitude in (AttitudeControlType[])Enum.GetValues(typeof(AttitudeControlType)))
                        {
                                attitudeLeftList.Add(attitude);
                        }
                        attitudeLeftList.Sort();

                }


                void SaveLoadout()
                {


                        module.SequenceEngine.ControllerModules.Clear();

                        foreach(ControlType control in controlRightList)
                        {

                                module.SequenceEngine.AddControl(control);

                                        
                        }

                        

                        
                
                }


        }
}
