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

                Rect rectLook;
                Vector2 mousecheck;

                GUIStyle BackgroundStyle;
                int paddingWidth = 10;
                int paddingHeight = 10;

                Rect mainWindowPos = new Rect(60, 50, 500, 400);
                Vector2 minProfileWindowSize = new Vector2(500, 400);

                bool mainWindowEnabled = false;

                Vector2 mainWindowScrollPos = new Vector2(0, 0);

                //
                // Unique window id
                int windowId = 93972;

                // For dragging windows
                Rect titleBarRect = new Rect(0, 0, 10000, 20);

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");



                ferramGraph graph;

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

                        if(graph == null)
                        {

                                graph = new ferramGraph(320, 240);
                        }

                        if (BackgroundStyle == null )
                        {
                                // DM: initialize styles on first use
                                BackgroundStyle = new GUIStyle(GUI.skin.box);
                                BackgroundStyle.richText = true;
                                BackgroundStyle.hover = BackgroundStyle.active = BackgroundStyle.normal;
                                BackgroundStyle.padding = new RectOffset(0, 0, 0, 0);

                                graph.SetBoundaries(0, 50, -10, 10);
                                graph.SetGridScaleUsingValues(1, 5);
                                graph.horizontalLabel = "time";
                                graph.verticalLabel = "value";
                                graph.Update();

                        }

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



                        GUILayout.Space(1);

                        if (Convert.ToInt32(rectLook.width) - 220 != graph.width || Convert.ToInt32(rectLook.height) - 100 != graph.height)
                        {
                                int graphWidth = Convert.ToInt32(rectLook.width) - 220;
                                int graphHeight = Convert.ToInt32(rectLook.height) - 100;
                                //graphHeight = Convert.ToInt32(rectLook.height) - 250;
                                graph.resizeGraph(graphWidth, graphHeight);
                                graph.Update();
                        }
                        
                        graph.Display(BackgroundStyle, 0, 0);

                        GUILayout.Label("w: " + rectLook.width + " h: " + rectLook.height + " xMax: " + rectLook.xMax + " yMax: " + rectLook.yMax);
                        GUILayout.Label("gw: " + graph.width + " gh: " + graph.height);

                        GUI.Label(new Rect(10, 10, 150, 20), "This text is not rotated.");
                        GUIUtility.RotateAroundPivot(-90, new Vector2(160, 30));
                        GUI.Label(new Rect(10, 30, 150, 20), "This text is rotated.");

                        GUILayout.Label("mx: " + mousecheck.x + " my: " + mousecheck.y);
                        /* if (GUILayout.Button(loadIcon, STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                        {
                                
                                //graph.resizeGraph(800, 600);
                                
                        }*/
                        mainWindowPos = ResizeWindow(id, mainWindowPos, minProfileWindowSize);
                        GUI.DragWindow(titleBarRect);
                }

                Rect ResizeWindow(int id, Rect windowRect, Vector2 minWindowSize)
                {
                        Vector2 mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
                        mousecheck = mouse;
                        //Rect r = GUILayoutUtility.GetRect(gcDrag, GUI.skin.window);
                        Rect r = new Rect(windowRect.width - 20, windowRect.height - 20, 20, 20);
                        if (Event.current.type == EventType.mouseDown && r.Contains(mouse))
                        {
                                resizing = id;
                                resizeStart = new Rect(mouse.x, mouse.y, windowRect.width, windowRect.height);
                        }
                        else if (Event.current.type == EventType.mouseUp && resizing == id)
                        {
                                resizing = 0;

                                windowRect.width = Mathf.Max(minWindowSize.x, resizeStart.width + (mouse.x - resizeStart.x));
                                windowRect.height = Mathf.Max(minWindowSize.y, resizeStart.height + (mouse.y - resizeStart.y));
                                windowRect.xMax = Mathf.Min(Screen.width, windowRect.xMax); // modifying xMax affects width, not x
                                windowRect.yMax = Mathf.Min(Screen.height, windowRect.yMax); // modifying yMax affects height, not y

                                rectLook.width = windowRect.width;
                                rectLook.height = windowRect.height;
                                rectLook.xMax = windowRect.xMax;
                                rectLook.yMax = windowRect.yMax;
                        }
                        else if (!Input.GetMouseButton(0))
                                resizing = 0;
                        else if (resizing == id)
                        {
                                windowRect.width = Mathf.Max(minWindowSize.x, resizeStart.width + (mouse.x - resizeStart.x));
                                windowRect.height = Mathf.Max(minWindowSize.y, resizeStart.height + (mouse.y - resizeStart.y));
                                windowRect.xMax = Mathf.Min(Screen.width, windowRect.xMax); // modifying xMax affects width, not x
                                windowRect.yMax = Mathf.Min(Screen.height, windowRect.yMax); // modifying yMax affects height, not y

                                rectLook.width = windowRect.width;
                                rectLook.height = windowRect.height;
                                rectLook.xMax = windowRect.xMax;
                                rectLook.yMax = windowRect.yMax;

                        }
                        if (GUI.Button(r, gcDrag, GUI.skin.label))
                        {
                                
                        }
                        return windowRect;
                }


                private Texture2D GetTexture(string id)
                {
                        return GameDatabase.Instance.GetTexture("AscentProfiler/Textures/" + id, false);
                }

        }
}
