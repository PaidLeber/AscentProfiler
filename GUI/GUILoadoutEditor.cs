using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class GUILoadoutEditor : MonoBehaviour
        {

                Rect WindowRect = new Rect(50, 50, 800, 600);

                int windowId = 98473;
                string loadoutTitle;

                Vector2 leftScrollPosition;


                internal GUILoadoutEditor(string loadoutitle)
                {
                        this.loadoutTitle = loadoutitle;

                }


                void OnGUI()
                {

                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, loadoutTitle);

                }

                void DrawLoadoutEditor(int id)
                {

                        GUILayout.BeginHorizontal();

                                leftScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);

                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                
                                        foreach (SensorType value in (SensorType[])Enum.GetValues(typeof(SensorType)))
                                        {
                                                GUILayout.Label(value.ToString());

                                        }

                                        GUILayout.BeginVertical();

                                GUILayout.EndScrollView();

                        GUILayout.EndHorizontal();




                }



        }
}
