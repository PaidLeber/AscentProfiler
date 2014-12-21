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

                Rect WindowRect = new Rect(400, 400, 240, 60);

                string windowTitle = "Change Sequence Unique Identifier";
                string changesuid;

                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal void InitWindow(SequenceEngine module)
                {
                        this.module = module;
                        changesuid = module.SUID;

                }

                void OnGUI()
                {
                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, windowTitle);
                }

                void DrawLoadoutEditor(int id)
                {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        changesuid = GUILayout.TextField(changesuid, GUILayout.Width(220));


                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Close"))
                        {
                                module.SUID = changesuid;
                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUISUIDEditor>());
                        }
                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }






        }
}
