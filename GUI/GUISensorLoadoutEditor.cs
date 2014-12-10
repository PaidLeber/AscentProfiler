using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUISensorLoadoutEditor : MonoBehaviour
        {
                AscentProAPGCSModule module;

                int windowId = 98473;

                Rect WindowRect = new Rect(200, 100, 450, 400);

                string windowTitle;
                LoadoutType LoadoutType;

                Vector2 leftScrollPosition;
                Vector2 rightScrollPosition;

                List<SensorType> leftList = new List<SensorType>();
                List<SensorType> rightList = new List<SensorType>();

                //Styles
                GUIStyle labelStyle = new GUIStyle();
                

                internal GUISensorLoadoutEditor()
                {
                        

                }

                internal void InitWindow(AscentProAPGCSModule module, LoadoutType loadouttype, string title)
                {
                        this.module = module;
                        this.LoadoutType = loadouttype;
                        this.windowTitle = title;

                        switch (loadouttype)
                        {
                                case LoadoutType.Sensor:

                                        InitSensorController(module);
                                        LoadSensorsFromPartModule(module);
                                        EnumSensorTypes();

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


                                                leftScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);
                        
                                                foreach (SensorType sensor in leftList.ToList())
                                                {
                                                        if(GUILayout.Button(sensor.ToString()))
                                                        {
                                                                rightList.Add(sensor);
                                                                rightList.Sort();

                                                                leftList.Remove(sensor);
                                                                leftList.Sort();


                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();


                                        GUILayout.EndVertical();


                        

                                        GUILayout.FlexibleSpace();

                                        GUILayout.BeginVertical();

                                                GUILayout.Space(70);
                                                labelStyle.normal.textColor = Color.cyan;
                                                GUILayout.Label(">", labelStyle);
                                                GUILayout.Space(200);
                                                GUILayout.Label("<", labelStyle);

                                        GUILayout.EndVertical();

                                        GUILayout.FlexibleSpace();
                                        
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.green;
                                                GUILayout.Label(LoadoutType+" Loadout", labelStyle);

                                                GUILayout.Space(5);

                                                rightScrollPosition = GUILayout.BeginScrollView(rightScrollPosition, false, false);
                        
                                                foreach (SensorType sensor in rightList.ToList())
                                                {
                                                        if (GUILayout.Button(sensor.ToString()))
                                                        {
                                                                leftList.Add(sensor);
                                                                leftList.Sort();

                                                                rightList.Remove(sensor);
                                                                rightList.Sort();


                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();

                                        GUILayout.EndVertical();
                                        


                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Save Loadout"))
                        {

                                SaveSensorLoadout();

                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUISensorLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }





                void InitSensorController(AscentProAPGCSModule module)
                {
                        if (!module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                        {
                                module.SequenceEngine.ControllerModules.Add(ControlType.SENSOR, new ControlSensors());
                                

                        }

                }
                
                void LoadSensorsFromPartModule(AscentProAPGCSModule module)
                {
                        rightList = module.SequenceEngine.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>();
                        rightList.Remove(SensorType.TIME);
                        rightList.Sort();

                }

                void EnumSensorTypes()
                {
                        
                        foreach (SensorType sensor in (SensorType[])Enum.GetValues(typeof(SensorType)))
                        {
                                leftList.Add(sensor);
                        }
                        leftList.Remove(SensorType.TIME);
                        leftList = leftList.Except(rightList).ToList();
                        
                        leftList.Sort();
                }


                void SaveSensorLoadout()
                {
                        module.SequenceEngine.ControllerModules[ControlType.SENSOR].ClearTypes();

                        module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(SensorType.TIME);                              // Remove Time array from available sensor options to user, but add it here

                        foreach(SensorType sensor in rightList)
                        {
                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(sensor);
                        }



                        
                
                }

                private void SetEnabled(bool newVal)
                {
                        if (newVal != enabled)
                                enabled = newVal;
                }

        }
}
