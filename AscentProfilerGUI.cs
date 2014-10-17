using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AscentProfiler
{
        class AscentProfilerGUI
        {
                private bool profileLoaded = false;

                Texture2D loadIcon;

                //GUI Styles
                private GUIStyle STYLE_WINDOW_BUTTON;



                Rect mainWindowPos = new Rect(60, 50, 280, 400);
                bool mainWindowEnabled = false;
                Vector2 minProfileWindowSize = new Vector2(280, 400);
                Vector2 mainWindowScrollPos = new Vector2(0,0);

                // Unique window id
                int windowId = 93971;

                // For dragging windows
                Rect titleBarRect = new Rect(0, 0, 10000, 20);

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");

                //Profiling loading
                private string selectedProfile = "";
                private string selectedContent = "";
                private float selectedHeight = 0;

                //test values
                private bool testbool = false;
                private ProfileLoader profileLoader;



                public AscentProfilerGUI()
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
                                mainWindowPos = GUILayout.Window(windowId + 1, mainWindowPos, DrawMainWindow, "Ascent Profiler");
                        }

                }


                public void DrawMainWindow(int id) {



                        
                        mainWindowEnabled = !GUI.Toggle(new Rect(mainWindowPos.width - 25, 0, 20, 20), !mainWindowEnabled, "");


                        GUIStyle defaultButton = new GUIStyle(GUI.skin.GetStyle("button"));

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(loadIcon, STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                                {
                                
                                }

                        GUILayout.EndHorizontal();

                        mainWindowScrollPos = GUILayout.BeginScrollView(mainWindowScrollPos);

                        if (!profileLoaded)
                        {
                                if(!testbool)
                                { 
                                        profileLoader = new ProfileLoader();



                                        testbool = true;


                                }

                                


                                GUILayout.BeginVertical();

                                        foreach(KeyValuePair<string, string> pair in profileLoader.GetProfiles())
                                        {
                                                GUILayout.BeginHorizontal();
                                                        if (GUILayout.Button("V", STYLE_WINDOW_BUTTON, GUILayout.Width(24), GUILayout.Height(24)))
                                                        {
                                                                selectedProfile = pair.Key;
                                                                selectedContent = pair.Value;
                                                                selectedHeight = GUI.skin.GetStyle("label").CalcHeight(new GUIContent(selectedContent), 200);
                                                                testbool = false;
                                                        }
                                                        if (GUILayout.Button(pair.Key, STYLE_WINDOW_BUTTON, GUILayout.Height(24)))
                                                        {
                                                                //profileLoaded = profileLoader.LoadProfile(pair.Key);
                                                        }

                                                        if (GUILayout.Button("E", STYLE_WINDOW_BUTTON, GUILayout.Width(24) , GUILayout.Height(24)))
                                                        {
                                                                
                                                                selectedProfile = pair.Key;
                                                                selectedContent = profileLoader.LoadProfile(pair.Key);

                                                                selectedHeight = GUI.skin.GetStyle("label").CalcHeight(new GUIContent(selectedContent), 200);
                                                                
                                                        }

                                                GUILayout.EndHorizontal();

                                                if (selectedProfile == pair.Key)
                                                {
                                                        GUILayout.Label(selectedContent, GUILayout.Width(200), GUILayout.Height(selectedHeight));
                                                }
                                        }
                                GUILayout.EndVertical();

                                
                        }

                        GUILayout.EndScrollView();

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
