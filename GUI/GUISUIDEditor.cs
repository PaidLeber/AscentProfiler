using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUISUIDEditor : MonoBehaviour
        {
                SequenceEngine module;

                int windowId = 98480;

                Rect WindowRect = new Rect(400, 400, 200, 200);

                string windowTitle = "Sequence Unique Identifier Editor";



                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal void InitWindow(SequenceEngine module)
                {
                        this.module = module;

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




                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }






        }
}
