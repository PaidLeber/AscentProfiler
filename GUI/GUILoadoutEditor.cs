﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class GUILoadoutEditor : MonoBehaviour
        {
                AscentProAPGCSModule module;

                Rect WindowRect = new Rect(200, 100, 450, 400);

                string windowTitle;
                string windowType;

                int windowId = 98473;
                Vector2 leftScrollPosition;
                Vector2 rightScrollPosition;

                private List<SensorType> leftList = new List<SensorType>();
                private List<SensorType> rightList;

                //Styles
                GUIStyle labelStyle = new GUIStyle();
                

                internal GUILoadoutEditor()
                {
                        

                }

                public void InitWindow(AscentProAPGCSModule module, string windowtype, string title)
                {
                        this.module = module;
                        windowType = windowtype;
                        windowTitle = title;

                        switch (windowtype)
                        {
                                case "Sensor":

                                        InitSensorController(module);
                                        EnumSensorTypes();
                                        LoadSensorsFromPartModule(module);
                                        break;
                        }

                }

                void LoadSensorsFromModule(AscentProAPGCSModule module)
                {
                        

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

                                                GUILayout.Label(windowType + "s Available", labelStyle);

                                                GUILayout.Space(5);


                                                leftScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);

                                                foreach (SensorType sensor in leftList)
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
                                                GUILayout.Label(windowType+" Loadout", labelStyle);

                                                GUILayout.Space(5);

                                                rightScrollPosition = GUILayout.BeginScrollView(rightScrollPosition, false, false);

                                                foreach (SensorType sensor in rightList)
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
                                
                        
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }





                void InitSensorController(AscentProAPGCSModule module)
                {
                        if (!module.ControllerModules.ContainsKey(ControlType.SENSOR))
                        {
                                module.ControllerModules.Add(ControlType.SENSOR, new ControlSensors());
                                module.ControllerModules[ControlType.SENSOR].AddType<SensorType>(SensorType.TIME);

                        }

                }

                void EnumSensorTypes()
                {
                        foreach (SensorType sensor in (SensorType[])Enum.GetValues(typeof(SensorType)))
                        {
                                leftList.Add(sensor);
                        }
                        leftList.Sort();
                }

                void LoadSensorsFromPartModule(AscentProAPGCSModule module)
                { 
                       rightList = module.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>();
                      
                
                }


        }
}
