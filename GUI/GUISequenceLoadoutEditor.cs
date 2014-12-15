using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using KSP.IO;


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
                string currentPath = AscentProfiler.sequenceDir;

                List<ControlType> sequenceRightList = new List<ControlType>();



                //Styles
                GUIStyle labelStyle = new GUIStyle();
                

                internal void InitWindow(AscentProAPGCSModule module)
                {
                        this.module = module;

                }

                void Start()
                {
                        
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
                                directoryLeftList = new List<string>(sequenceLoader.GetDirectoryNames(currentPath));
                                fileLeftList = new List<string>(sequenceLoader.GetFileNames(currentPath));
                                sequences = new Dictionary<string, string>(sequenceLoader.GetFileContents(currentPath));
                                


                                
                                directoryLoaded = true;
                        }


                        GUILayout.BeginVertical();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button("<- Directory", GUILayout.Width(160)))
                        {
                              currentPath = Directory.GetParent(currentPath).ToString();
                              directoryLoaded = false;
                        }
                        var origfont = labelStyle.fontSize;
                        labelStyle.fontSize = 9;
                        labelStyle.alignment = TextAnchor.LowerLeft;
                        GUILayout.Label(currentPath, labelStyle);
                        labelStyle.fontSize = origfont;
                        GUILayout.EndHorizontal();

                        labelStyle.alignment = TextAnchor.MiddleCenter;

                        GUILayout.Space(10);
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
                                                                currentPath = currentPath + "/" + folder;
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
                                        GUILayout.Label("Name", GUILayout.Width(40));
                                        GUILayout.TextField(sequencename);
                                        GUILayout.Button("Save", GUILayout.Width(50));
                                        GUILayout.EndHorizontal();
                                        editorLeftScrollPosition = GUILayout.BeginScrollView(editorLeftScrollPosition);

                                                stringToEdit = GUILayout.TextArea( stringToEdit, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                                        GUILayout.EndScrollView();


                                        GUILayout.EndVertical();

                                        GUILayout.BeginVertical(GUILayout.Width(160));

                                                labelStyle.normal.textColor = Color.green;
                                                GUILayout.Label("Sequences Loaded", labelStyle);

                                                GUILayout.Space(5);

                                                sequenceRightScrollPosition = GUILayout.BeginScrollView(sequenceRightScrollPosition, false, false);

     




















                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        




                        if (GUILayout.Button("Save Configuration"))
                        {

                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUISequenceLoadoutEditor>());
                                
                        
                        }

                        mainWindowRect = ResizeWindow(id, mainWindowRect, minProfileWindowSize);
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
