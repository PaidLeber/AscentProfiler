using System;
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
                List<string> directoryLeftList;
                List<string> fileLeftList;
                Dictionary<string, string> sequences;
                string stringToEdit = "";
                string sequencename = "";
                string terminal = "APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready";

                List<ControlType> sequenceRightList = new List<ControlType>();

                private float m_TimeStamp;
                private bool cursor = false;
                private string cursorChar = "";
                Texture2D labeltex;
                //Styles
                GUIStyle labelStyle = new GUIStyle();
                

                internal void InitWindow(AscentProAPGCSModule module)
                {
                        this.module = module;
                        
                }

                void Start()
                {
                        labeltex = MakeTexture(1080, 200, new Color(0.0f, 0.0f, 0.0f));

                        Log.Console("APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready");
                        Log.Console("APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready");
                        Log.Console("APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready");
                        Log.Console("APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready");
                        Log.Console("APGCS Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready");

                }

                void OnGUI()
                {
                        mainWindowRect = GUILayout.Window(windowId, mainWindowRect, DrawLoadoutEditor, windowTitle);
                }





                void DrawLoadoutEditor(int id)
                {

                        if(!directoryLoaded)
                        {
                                sequenceLoader = new SequenceLoader();
                                directoryLeftList = new List<string>(sequenceLoader.GetDirectoryNames(AscentProfilerVAB.sequenceLoadPath));
                                fileLeftList = new List<string>(sequenceLoader.GetFileNames(AscentProfilerVAB.sequenceLoadPath));
                                sequences = new Dictionary<string, string>(sequenceLoader.GetFileContents(AscentProfilerVAB.sequenceLoadPath));
                                


                                
                                directoryLoaded = true;
                        }


                        GUILayout.BeginVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button("<- Directory", GUILayout.Width(160)))
                        {
                                AscentProfilerVAB.sequenceLoadPath = Directory.GetParent(AscentProfilerVAB.sequenceLoadPath).ToString();
                              directoryLoaded = false;
                        }
                        var origfont = GUI.skin.label.fontSize;

                        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                        GUI.skin.label.fontSize = 10;
                        GUILayout.Label(AscentProfilerVAB.sequenceLoadPath);
                        GUI.skin.label.fontSize = origfont;
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
                                                foreach (string folder in directoryLeftList)
                                                {

                                                        if (GUILayout.Button(folder))
                                                        {
                                                                AscentProfilerVAB.sequenceLoadPath = AscentProfilerVAB.sequenceLoadPath + "/" + folder;
                                                                directoryLoaded = false;
                                                        }
                                                       

                                                }
                                                GUI.skin.button.fontStyle = FontStyle.Normal;


                                                foreach(string file in fileLeftList)
                                                {

                                                        GUILayout.BeginHorizontal();

                                                        if (GUILayout.Button(file))
                                                        {


                                                        }
                                                        if (GUILayout.Button("E", GUILayout.Width(20)))
                                                        {

                                                                stringToEdit = sequences[file];
                                                                sequencename = file;

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
                                                if (!Regex.IsMatch(sequencename, @"^.*$"))
                                                {
                                                        sequencename = sequencename.Replace(" ", "_");

                                                        string filename = AscentProfilerVAB.sequenceLoadPath + "/" + sequencename + AscentProfilerVAB.sequenceExt;

                                                        if (System.IO.File.Exists(filename))
                                                                System.IO.File.Delete(filename);

                                                        System.IO.File.WriteAllText(filename, stringToEdit);

                                                        directoryLoaded = false;

                                                }
                                                else
                                                { 
                                                        //Print Error
                                                
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

                        GUILayout.BeginHorizontal();
                                var consoleStyle = new GUIStyle();
                                consoleStyle.normal.background = labeltex;
                                var consoleLabelStyle = new GUIStyle(GUI.skin.label);
                                consoleLabelStyle.margin = new RectOffset(0, 0, 0, 0);
                                GUILayout.BeginVertical(consoleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(140));

                                
                                     

                                int count = 0;
                                foreach(string message in Log.console)
                                {
                                        count++;

                                        if (count != Log.console.Count)
                                        {
                                                GUILayout.Label(" Console Count: "+Log.console.Count + " count:"+ count, consoleLabelStyle, GUILayout.ExpandWidth(true));
                                        }
                                        else
                                        {
                                                GUILayout.Label("F: Console Count: " + Log.console.Count + " count:" + count + cursorChar, consoleLabelStyle, GUILayout.ExpandWidth(true));
                                        }

                                        
                                }



                                GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        /*
                        var txtcolor = GUI.skin.label.normal.textColor;
                        var bkgcolor = GUI.skin.label.normal.background;
                        GUI.skin.label.normal.background = labeltex;
                        GUI.skin.label.normal.textColor = new Color(255, 114, 0);
                        GUI.skin.label.alignment = TextAnchor.UpperLeft;
                        GUILayout.Label( terminal + cursorChar, GUILayout.ExpandWidth(true), GUILayout.Height(140));
                        GUI.skin.label.normal.background = bkgcolor;
                        GUI.skin.label.normal.textColor = txtcolor;
                        */
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
                        if (Time.time - m_TimeStamp >= 0.5)
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










        }
}
