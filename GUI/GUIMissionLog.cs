using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUIMissionLog : MonoBehaviour
        {
                SequenceEngine module;

                int windowId = 98480;
                Rect WindowRect = new Rect(400, 400, 240, 60);

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");
                Vector2 minLogWindowSize = new Vector2(200, 200);


                string windowTitle = "Mission Log";
                

                //Styles
                GUIStyle logStyle = new GUIStyle();

                void OnGUI()
                {
                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, windowTitle);
                }

                void DrawLoadoutEditor(int id)
                {
                        
                        GUILayout.BeginVertical(logStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                      
                        foreach (string message in Log.consolebuffer)
                        {

                                GUILayout.Label(message, logStyle, GUILayout.ExpandWidth(true));
                        }

                        WindowRect = ResizeWindow(id, WindowRect, minLogWindowSize);
                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                        GUILayout.EndVertical();


                        
                }



                Rect ResizeWindow(int id, Rect windowRect, Vector2 minWindowSize)
                {
                        Vector2 mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
                        //Rect r = GUILayoutUtility.GetRect(gcDrag, GUI.skin.window);
                        Rect r = new Rect(windowRect.width - 20, windowRect.height - 20, 20, 20);
                        if (Event.current.type == EventType.mouseDown && r.Contains(mouse))
                        {
                                resizing = id;
                                resizeStart = new Rect(mouse.x, mouse.y, windowRect.width, windowRect.height);
                        }
                        else if (Event.current.type == EventType.mouseUp && resizing == id)
                                resizing = 0;
                        else if (!Input.GetMouseButton(0))
                                resizing = 0;
                        else if (resizing == id)
                        {
                                windowRect.width = Mathf.Max(minWindowSize.x, resizeStart.width + (mouse.x - resizeStart.x));
                                windowRect.height = Mathf.Max(minWindowSize.y, resizeStart.height + (mouse.y - resizeStart.y));
                                windowRect.xMax = Mathf.Min(Screen.width, windowRect.xMax); // modifying xMax affects width, not x
                                windowRect.yMax = Mathf.Min(Screen.height, windowRect.yMax); // modifying yMax affects height, not y

                        }
                        GUI.Button(r, gcDrag, GUI.skin.label);
                        return windowRect;
                }


        }
}
