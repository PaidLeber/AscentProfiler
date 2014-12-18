﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


namespace AscentProfiler
{
        class GUISequenceLoadoutEditor : MonoBehaviour
        {
                SequenceLoader sequenceLoader;
                bool directoryLoaded;

                AscentProAPGCSModule module;

                int windowId = 98476;

                Rect mainWindowRect = new Rect(200, 100, 600, 500);
                Vector2 minProfileWindowSize = new Vector2(600, 500);
                string windowTitle = "Sequence Upload Window";

                // For resizing windows
                int resizing = 0;
                Rect resizeStart = new Rect();
                GUIContent gcDrag = new GUIContent("><", "Drag to resize window");


                Vector2 sequenceLeftScrollPosition;
                Vector2 sequenceRightScrollPosition;
                Vector2 editorLeftScrollPosition;
                string stringToEdit = "";
                string sequencename = "";

                List<ControlType> sequenceRightList = new List<ControlType>();

                private float m_TimeStamp;
                private bool cursor = false;
                private string cursorChar = "";
                Texture2D consoleBackgroundTexture;
                //Styles
                GUIStyle labelStyle;
                GUIStyle consoleStyle;
                GUIStyle consoleLabelStyle;
                GUIStyle ctextStyle;
                GUIStyle pathStyle;
                bool setstyle;

                internal void InitWindow(AscentProAPGCSModule module)
                {
                        this.module = module;
                        Log.consolebuffer.Clear();
                        Log.Console("Kernix OS Version "+ AscentProfiler.version);
                        Log.Console("");
                        Log.Console("[root@VAB]:~/$ sequence_loader.sh");
                        Log.Console("");
                        
                }

                void Start()
                {
                        consoleBackgroundTexture = MakeTexture(1080, 200, new Color(0.0f, 0.0f, 0.0f));
                }

                void OnGUI()
                {
                        if(!setstyle)
                        {


                                labelStyle = new GUIStyle();

                                consoleStyle = new GUIStyle();
                                consoleStyle.normal.background = consoleBackgroundTexture;
                                consoleStyle.padding = new RectOffset(5, 5, 4, 0);

                                consoleLabelStyle = new GUIStyle(GUI.skin.label);
                                consoleLabelStyle.margin = new RectOffset(0, 0, 0, 0);
                                consoleLabelStyle.padding = new RectOffset(0, 0, 0, 0);
                                consoleLabelStyle.normal.textColor = new Color(1F, .6F, 0F);
                                consoleLabelStyle.alignment = TextAnchor.UpperLeft;
                                consoleLabelStyle.wordWrap = true;
                                consoleLabelStyle.fontSize = 11;

                                ctextStyle = new GUIStyle(GUI.skin.label);
                                ctextStyle.padding = new RectOffset(0, 0, 0, 0);
                                ctextStyle.margin = new RectOffset(0, 0, 0, 0);
                                ctextStyle.normal.textColor = Color.grey;
                                ctextStyle.fontSize = 11;
                                ctextStyle.alignment = TextAnchor.LowerLeft;

                                pathStyle = new GUIStyle(GUI.skin.label);
                                pathStyle.fontSize = 11;
                                pathStyle.alignment = TextAnchor.UpperLeft;
                                pathStyle.wordWrap = false;

                                setstyle = true;
                        }

                        mainWindowRect = GUILayout.Window(windowId, mainWindowRect, DrawLoadoutEditor, windowTitle);
                }





                void DrawLoadoutEditor(int id)
                {

                        if(!directoryLoaded)
                        {
                                sequenceLoader = new SequenceLoader(AscentProfiler.sequenceLoadPath);                                
                                directoryLoaded = true;
                        }


                        GUILayout.BeginVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button("<- Directory", GUILayout.Width(150)))
                        {
                                AscentProfiler.sequenceLoadPath = Directory.GetParent(AscentProfiler.sequenceLoadPath).ToString();
                              directoryLoaded = false;
                        }

                        GUILayout.Label(AscentProfiler.sequenceLoadPath, pathStyle);

                        GUILayout.EndHorizontal();

                        labelStyle.alignment = TextAnchor.MiddleCenter;

                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();

       
                                        GUILayout.BeginVertical(GUILayout.Width(160));

                                                labelStyle.normal.textColor = Color.yellow;

                                                GUILayout.Label("Sequences Available", labelStyle);

                                                GUILayout.Space(5);

                                                sequenceLeftScrollPosition = GUILayout.BeginScrollView(sequenceLeftScrollPosition);


                                                GUI.skin.button.fontStyle = FontStyle.Bold;
                                                foreach (string folder in sequenceLoader.GetDirectoryNames())
                                                {

                                                        if (GUILayout.Button(folder))
                                                        {
                                                                AscentProfiler.sequenceLoadPath = AscentProfiler.sequenceLoadPath + "/" + folder;
                                                                directoryLoaded = false;
                                                        }
                                                       

                                                }
                                                GUI.skin.button.fontStyle = FontStyle.Normal;


                                                foreach (KeyValuePair<string, string> pair in sequenceLoader.GetSequences())
                                                {

                                                        GUILayout.BeginHorizontal();

                                                        if (GUILayout.Button(pair.Key))
                                                        {
                                                                if (sequenceLoader.LoadSequence(pair.Key))
                                                                {
                                                                        
                                                                        Log.Console("Sequence Uploaded to Vessel Module: " + AscentProfilerFlight.currentVessel.vesselName);
                                                                }
                                                                else
                                                                {
                                                                        Log.Console("Unable to Upload Sequence File: " + AscentProfilerFlight.currentVessel.vesselName);
                                                                }




                                                        }
                                                        if (GUILayout.Button("E", GUILayout.Width(20)))
                                                        {
                                                                sequencename = pair.Key;
                                                                stringToEdit = pair.Value;
                                                                

                                                        }
                                                        GUILayout.EndHorizontal();

                                                }

                        
                                                GUILayout.EndScrollView();


                                        GUILayout.EndVertical();


                                        GUILayout.BeginVertical();

                                        GUILayout.BeginHorizontal();
                                        if (GUILayout.Button("New", GUILayout.Width(50)))
                                        {
                                                sequencename = "";
                                                stringToEdit = "";
                                        }
                                        GUILayout.Label("Name", GUILayout.Width(35));
                                        sequencename = GUILayout.TextField(sequencename);

                                        if (GUILayout.Button("Save", GUILayout.Width(50)))
                                        {
                                                if (Regex.IsMatch(sequencename, @"^[a-zA-Z0-9_]*$") && !Regex.IsMatch(sequencename, @"^\s*$"))
                                                {
                                                        sequencename = MakeValidFileName(sequencename);

                                                        try
                                                        {
                                                                string filename = AscentProfiler.sequenceLoadPath + "/" + sequencename + AscentProfiler.sequenceExt;

                                                                if (System.IO.File.Exists(filename))
                                                                        System.IO.File.Delete(filename);

                                                                System.IO.File.WriteAllText(filename, stringToEdit);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                                Log.Console("File save error: " + e.Message + " at " + e.StackTrace);
                                                        }

                                                        



                                                        directoryLoaded = false;

                                                        Log.Console(sequencename + ".seq saved.");

                                                }
                                                else
                                                {
                                                        Log.Console("Error: Unable to save to disk: Bad file name");
                                                
                                                }


                                                

                                        }
                                        GUILayout.EndHorizontal();
                                        editorLeftScrollPosition = GUILayout.BeginScrollView(editorLeftScrollPosition);

                                                stringToEdit = GUILayout.TextArea( stringToEdit, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                                        GUILayout.EndScrollView();


                                        GUILayout.EndVertical();

                                        GUILayout.BeginVertical(GUILayout.Width(160));

                                                labelStyle.normal.textColor = Color.green;
                                                GUILayout.Label("Sequences Loaded", labelStyle);

                                                GUILayout.Space(10);

                                                sequenceRightScrollPosition = GUILayout.BeginScrollView(sequenceRightScrollPosition, false, false);

     




















                        
                                                GUILayout.EndScrollView();

                                                



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();
                        GUILayout.Label("Console:", ctextStyle);
                        GUILayout.BeginHorizontal();




                                GUILayout.BeginVertical(consoleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(100));

                                int count = 1;
                                foreach(string message in Log.consolebuffer)
                                {
                                        
                                        if (count != Log.consolebuffer.Count)
                                        {
                                                GUILayout.Label(message, consoleLabelStyle, GUILayout.ExpandWidth(true));
                                        }
                                        else
                                        {
                                                GUILayout.Label(message + cursorChar, consoleLabelStyle, GUILayout.ExpandWidth(true));
                                        }

                                        count++;
                                }



                                GUILayout.EndVertical();
                        GUILayout.EndHorizontal();


                        if (GUILayout.Button("Save Configuration"))
                        {

                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUISequenceLoadoutEditor>());
                                
                        
                        }
                        GUILayout.Space(8);
                        mainWindowRect = ResizeWindow(id, mainWindowRect, minProfileWindowSize);
                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                        GUILayout.EndVertical();


                }



                private Texture2D MakeTexture(int width, int height, Color col)
                {
                        Color[] pix = new Color[width * height];

                        for (int i = 0; i < pix.Length; i++)
                                pix[i] = col;

                        Texture2D result = new Texture2D(width, height);
                        result.SetPixels(pix);
                        result.Apply();

                        return result;
                }


                void Update()
                {
                        if (Time.time - m_TimeStamp >= 0.25)
                        {
                                m_TimeStamp = Time.time;
                                if (cursor == false)
                                {
                                        cursor = true;
                                        cursorChar += "_";
                                }
                                else
                                {
                                        cursor = false;
                                        if (cursorChar.Length != 0)
                                        {
                                                cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
                                        }
                                }
                        }
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




                private static string MakeValidFileName(string name)
                {
                        string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

                        return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
                }





        }
}
