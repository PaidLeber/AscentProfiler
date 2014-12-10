using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class GUILoadoutEditor : MonoBehaviour
        {
                Rect WindowRect = new Rect(200, 100, 450, 400);

                string windowTitle;
                string windowType;

                int windowId = 98473;
                Vector2 leftScrollPosition;
                Vector2 rightScrollPosition;

                private List<string> leftList = new List<string>();
                private List<string> rightList = new List<string>();

                //Styles
                GUIStyle labelStyle = new GUIStyle();
                

                internal GUILoadoutEditor()
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

                                                foreach (string value in leftList)
                                                {
                                                        GUILayout.Button(value);

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

                                                foreach (SensorType value in (SensorType[])Enum.GetValues(typeof(SensorType)))
                                                {
                                                        GUILayout.Button(value.ToString());

                                                }

                                                GUILayout.EndScrollView();

                                        GUILayout.EndVertical();
                                        


                        GUILayout.EndHorizontal();
                        
                        GUILayout.Button("Apply");
                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }

                public void InitWindow(string windowtype, string title)
                {
                        windowType = windowtype;
                        windowTitle = title;
                        
                        switch(windowtype)
                        {
                                case "Sensor":
                                        LoadSensorTypes();
                                        break;
                        }

                }

                void LoadSensorTypes()
                {
                        foreach (SensorType value in (SensorType[])Enum.GetValues(typeof(SensorType)))
                        {
                                leftList.Add(value.ToString());
                        }
                        leftList.Sort();
                }



        }
}
