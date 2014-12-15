using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AscentProfiler
{
        class GUITelemetry
        {

                Texture2D loadIcon;

                //GUI Styles
                GUIStyle STYLE_WINDOW_BUTTON;

                GUIStyle BackgroundStyle;


                Rect telemetryWindowPos = new Rect(60, 50, 700, 400);
                Vector2 minDefaultWindowSize = new Vector2(700, 400);
                Rect telemetryWindowPosLast = new Rect(0, 0, 0, 0);
                Vector2 telemetryWindowScrollPos = new Vector2(0, 0);
                bool telemetryWindowEnabled = false;

                KerbalGraph graph = new KerbalGraph(500, 300, 0, 0, 0, 0);


                
                // Unique window id
                int windowId = 93972;

                // For dragging windows
                Rect titleBarRect = new Rect(0, 0, 10000, 20);

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");

                bool isDataLoaded = true;



                public GUITelemetry()
                {
                        loadIcon = GetTexture("load");

                }



                public void ChangeState(bool state)
                {
                        telemetryWindowEnabled = state;
                }


                public void OnGUI()
                {
                        if (!isDataLoaded)
                        {
                                //graph.autoscale = true;
                                //graph.SetBoundaries(0, 100, 0, 100);
                                Color val;
                                foreach (KeyValuePair<SensorType, List<double>> data in AscentProfilerFlight.telemetryReceiver.telemetryData)
                                {
                                        Debug.Log("DATA KEY IS: " + data.Key);
                                        Debug.Log(String.Join(" ", data.Value.ConvertAll(i => i.ToString()).ToArray()));
                                        if (data.Key != SensorType.TIME)
                                        {
                                                switch(data.Key)
                                                {
                                                        case SensorType.ALTITUDE:
                                                                val = new Color(.9f, .9f, 0f);
                                                                break;
                                                        case SensorType.MAXQ:
                                                                val = new Color(.2f, .5f, .7f);
                                                                break;
                                                        default:
                                                                val = new Color(1.0f, 0.0f, 0.0f);
                                                                break;
                                                }

                                                                
                
                                                graph.AddLine(data.Key.ToString(), AscentProfilerFlight.telemetryReceiver.telemetryData[SensorType.TIME].ToArray(), data.Value.ToArray(), val);
                                                //graph.SetLineHorizontalScaling(data.Key.ToString(), 1);
                                                //graph.SetLineVerticalScaling(data.Key.ToString(), 1);
                                        }
                                        


                                }

                                //double minx = Math.Round( AscentProfiler.telemetryReceiver.telemetryData[SensorType.TIME].Min() , 2);
                                //double maxx = Math.Round( AscentProfiler.telemetryReceiver.telemetryData[SensorType.TIME].Max() , 2);
                                //double miny = Math.Round( AscentProfiler.telemetryReceiver.telemetryData[SensorType.ALTITUDE].Min() , 2);
                                //double maxy = Math.Round(AscentProfiler.telemetryReceiver.telemetryData[SensorType.ALTITUDE].Max(), 2);
                                graph.SetBoundaries(0,500,0,500);
                                //graph.SetGridScaleUsingPixels(20, 20);
                                graph.horizontalLabel = "TIME IN SECS";
                                graph.verticalLabel = "ALTITUDE";
                                graph.Update();
                                isDataLoaded = true;
                        
                        }

                        if (BackgroundStyle == null )
                        {
                                // DM: initialize styles on first use
                                BackgroundStyle = new GUIStyle(GUI.skin.box);
                                BackgroundStyle.richText = true;
                                BackgroundStyle.hover = BackgroundStyle.active = BackgroundStyle.normal;
                                BackgroundStyle.padding = new RectOffset(0, 0, 0, 0);

                                //graph.SetBoundaries(0, 50, -10, 10);
                                //graph.SetGridScaleUsingValues(1, 5);
                                //graph.horizontalLabel = "";
                                //graph.verticalLabel = "";
                                //graph.Update();

                        }

                        GUI.skin = null;

                        STYLE_WINDOW_BUTTON = new GUIStyle(GUI.skin.GetStyle("button"));
                        STYLE_WINDOW_BUTTON.margin = new RectOffset(0, 0, 0, 0);
                        STYLE_WINDOW_BUTTON.padding = new RectOffset(0, 0, 0, 0);

                        if (telemetryWindowEnabled)
                        {
                                telemetryWindowPos = GUILayout.Window(windowId + 1, telemetryWindowPos, DrawMainWindow, "Ascent Profile for " + AscentProfilerFlight.currentVessel.vesselName);
                        }

                }


                public void DrawMainWindow(int id)
                {

                        //telemetryWindowEnabled = !GUI.Toggle(new Rect(telemetryWindowPos.width - 25, 0, 20, 20), !telemetryWindowEnabled, "");


                        GUIStyle defaultButton = new GUIStyle(GUI.skin.GetStyle("button"));

                        GUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button(loadIcon, STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                        {
                                isDataLoaded = false;
                        }
                        if (GUILayout.Button(loadIcon, STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                        {
                                graph.Clear();
                        }

                        GUILayout.EndHorizontal();



                        //GUILayout.Space(1);


                        if (telemetryWindowPos.width != telemetryWindowPosLast.width || telemetryWindowPos.height != telemetryWindowPosLast.height)
                        {
                                telemetryWindowPosLast = telemetryWindowPos;
                                //graph.resizeGraph((int)(telemetryWindowPos.width - minDefaultWindowSize.x + defaultGraphSize.x), (int)(telemetryWindowPos.height - minDefaultWindowSize.y + defaultGraphSize.y));
                                
                                
                                
                        }

                        graph.Display(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        
                        //graph.Display(BackgroundStyle);
                        
                        //GUILayout.Label("w: " + telemetryWindowPos.width + " h: " + telemetryWindowPos.height);
                        //GUILayout.Label("gw: " + graph.width + " gh: " + graph.height);
                        //GUILayout.Label("mx: " + mousecheck.x + " my: " + mousecheck.y);
                        //GUILayout.Label("deltaw: " + (int)(telemetryWindowPos.width - minDefaultWindowSize.x + defaultGraphSize.x) + " deltah: " + (int)(telemetryWindowPos.height - minDefaultWindowSize.y + defaultGraphSize.y));

                        telemetryWindowPos = ResizeWindow(id, telemetryWindowPos, minDefaultWindowSize);
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
