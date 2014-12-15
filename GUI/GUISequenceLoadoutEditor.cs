using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                Rect WindowRect = new Rect(200, 100, 450, 600);

                string windowTitle = "Sequence Upload Window";



                Vector2 sequenceLeftScrollPosition;
                Vector2 sequenceRightScrollPosition;
                List<string> directoryLeftList;
                List<string> fileLeftList;

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
                        labelStyle.alignment = TextAnchor.MiddleCenter;

                }

                void OnGUI()
                {
                        WindowRect = GUILayout.Window(windowId, WindowRect, DrawLoadoutEditor, windowTitle);
                }





                void DrawLoadoutEditor(int id)
                {

                        if(!directoryLoaded)
                        {
                                sequenceLoader = new SequenceLoader();
                                directoryLeftList = new List<string>(sequenceLoader.GetDirectoryNames(currentPath));
                                fileLeftList = new List<string>(sequenceLoader.GetFileNames(currentPath));



                                directoryLoaded = true;
                        }



                        GUILayout.BeginVertical();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();

       
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.yellow;

                                                GUILayout.Label("Sequences Available", labelStyle);

                                                GUILayout.Space(5);


                                                sequenceLeftScrollPosition = GUILayout.BeginScrollView(sequenceLeftScrollPosition);


                                                foreach (string directory in directoryLeftList)
                                                {

                                                        if (GUILayout.Button(directory.ToString()))
                                                        {





                                                        }


                                                }















                        
                                                GUILayout.EndScrollView();


                                        GUILayout.EndVertical();


                        

                                        GUILayout.FlexibleSpace();

                                        GUILayout.BeginVertical();

                                                GUILayout.Space(70);
                                                labelStyle.normal.textColor = Color.cyan;
                                                GUILayout.Label(">", labelStyle);
                                                GUILayout.Space(70);
                                                GUILayout.Label("<", labelStyle);

                                        GUILayout.EndVertical();

                                        GUILayout.FlexibleSpace();
                                        
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.green;
                                                GUILayout.Label("Sequences Loaded", labelStyle);

                                                GUILayout.Space(5);

                                                sequenceRightScrollPosition = GUILayout.BeginScrollView(sequenceRightScrollPosition, false, false);

     




















                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        




                        if (GUILayout.Button("Save Loadout"))
                        {


                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUIControlLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }



                











        }
}
