using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AscentProfiler
{
        class GraphGUI
        {

                Texture2D loadIcon;

                //GUI Styles
                GUIStyle STYLE_WINDOW_BUTTON;



                Rect mainWindowPos = new Rect(60, 50, 480, 320);
                bool mainWindowEnabled = false;
                Vector2 minProfileWindowSize = new Vector2(480, 320);
                Vector2 mainWindowScrollPos = new Vector2(0, 0);

                Vector2 logWindowScrollPos = new Vector2(0, 0);


                // Unique window id
                int windowId = 93972;

                // For dragging windows
                Rect titleBarRect = new Rect(0, 0, 10000, 20);

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");

                public GraphGUI()
                {
                        loadIcon = GetTexture("load");

                }



                public void ChangeState(bool state)
                {
                        mainWindowEnabled = state;
                }


                public void OnGUI()
                {

                        GUI.skin = null;

                        STYLE_WINDOW_BUTTON = new GUIStyle(GUI.skin.GetStyle("button"));
                        STYLE_WINDOW_BUTTON.margin = new RectOffset(0, 0, 0, 0);
                        STYLE_WINDOW_BUTTON.padding = new RectOffset(0, 0, 0, 0);

                        if (mainWindowEnabled)
                        {
                                mainWindowPos = GUILayout.Window(windowId + 1, mainWindowPos, DrawMainWindow, "Telemetry");
                        }

                }


                public void DrawMainWindow(int id)
                {

                        mainWindowEnabled = !GUI.Toggle(new Rect(mainWindowPos.width - 25, 0, 20, 20), !mainWindowEnabled, "");


                        GUIStyle defaultButton = new GUIStyle(GUI.skin.GetStyle("button"));

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(loadIcon, STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                        {

                        }


                        GUILayout.EndHorizontal();



                        GUILayout.Space(10);




                        mainWindowPos = ResizeWindow(id, mainWindowPos, minProfileWindowSize);
                        GUI.DragWindow(titleBarRect);
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


                private Texture2D GetTexture(string id)
                {
                        return GameDatabase.Instance.GetTexture("AscentProfiler/Textures/" + id, false);
                }

        }
}
