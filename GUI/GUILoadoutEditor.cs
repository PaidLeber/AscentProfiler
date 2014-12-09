using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class GUILoadoutEditor : MonoBehaviour
        {
                Rect WindowRect = new Rect(200, 100, 400, 400);

                string windowTitle;
                string windowType;

                int windowId = 98473;
                Vector2 leftScrollPosition;
                Vector2 rightScrollPosition;

                //Styles
                GUIStyle style = new GUIStyle();
                

                internal GUILoadoutEditor()
                {
                        

                }

                void Start()
                {
                        style.alignment = TextAnchor.MiddleCenter;
                }

                void OnGUI()
                {

                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, windowTitle);
                        Debug.Log("working");
                }

                void DrawLoadoutEditor(int id)
                {

                        GUILayout.BeginVertical();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();

                                        leftScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);

                                        GUILayout.BeginVertical(GUILayout.Width(150));
                                        style.normal.textColor = Color.yellow;
                                        GUILayout.Label(windowType + " Available", style);

                                        GUILayout.Space(5);

                                                foreach (SensorType value in (SensorType[])Enum.GetValues(typeof(SensorType)))
                                                {
                                                        GUILayout.Button(value.ToString());

                                                }

                                        GUILayout.EndVertical();
                                        GUILayout.EndScrollView();

                                        GUILayout.FlexibleSpace();

                                        rightScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);

                                        GUILayout.BeginVertical(GUILayout.Width(150));

                                        style.normal.textColor = Color.green;
                                        GUILayout.Label(windowType + " Loadout", style);

                                        GUILayout.Space(5);

                                        foreach (SensorType value in (SensorType[])Enum.GetValues(typeof(SensorType)))
                                        {
                                                GUILayout.Button(value.ToString());

                                        }

                                        GUILayout.EndVertical();
                                        GUILayout.EndScrollView();


                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();


                        GUI.DragWindow();
                }

                public void InitWindow(string windowtype, string title)
                {
                        windowType = windowtype;
                        windowTitle = title;
                        

                }

        }
}
