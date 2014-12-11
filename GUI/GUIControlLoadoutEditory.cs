using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class GUIControlLoadoutEditor : MonoBehaviour
        {
                AscentProAPGCSModule module;

                int windowId = 98473;

                Rect WindowRect = new Rect(200, 100, 450, 400);

                string windowTitle;
                LoadoutType LoadoutType;

                Vector2 leftScrollPosition;
                Vector2 rightScrollPosition;
                Vector2 leftattScrollPosition;
                Vector2 rightattScrollPosition;

                List<ControlType> leftList = new List<ControlType>();
                List<ControlType> rightList = new List<ControlType>();

                List<AttitudeControlType> leftattList = new List<AttitudeControlType>();
                List<AttitudeControlType> rightattList = new List<AttitudeControlType>();

                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal GUIControlLoadoutEditor()
                {
                        

                }

                internal void InitWindow(AscentProAPGCSModule module, LoadoutType loadouttype, string title)
                {
                        this.module = module;
                        this.LoadoutType = loadouttype;
                        this.windowTitle = title;

                        switch (loadouttype)
                        {
                                case LoadoutType.Control:

                                        InitController(module);
                                        LoadFromPartModule(module);
                                        EnumTypes();

                                        break;
                        }

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

                        GUILayout.BeginVertical();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();

       
                                        GUILayout.BeginVertical(GUILayout.Width(200));

                                                labelStyle.normal.textColor = Color.yellow;

                                                GUILayout.Label(LoadoutType + "s Available", labelStyle);

                                                GUILayout.Space(5);


                                                leftScrollPosition = GUILayout.BeginScrollView(leftScrollPosition);

                                                foreach (ControlType control in leftList.ToList())
                                                {
                                                        if(GUILayout.Button(control.ToString()))
                                                        {
                                                                rightList.Add(control);
                                                                rightList.Sort();

                                                                leftList.Remove(control);
                                                                leftList.Sort();


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
                                                GUILayout.Label(LoadoutType+" Loadout", labelStyle);

                                                GUILayout.Space(5);

                                                rightScrollPosition = GUILayout.BeginScrollView(rightScrollPosition, false, false);

                                                foreach (ControlType control in rightList.ToList())
                                                {
                                                        if (GUILayout.Button(control.ToString()))
                                                        {
                                                                leftList.Add(control);
                                                                leftList.Sort();

                                                                rightList.Remove(control);
                                                                rightList.Sort();


                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        
                        if (rightList.Contains(ControlType.ATTITUDE))
                        {
                                GUILayout.BeginVertical();
                                GUILayout.Label("ATTITUDE", labelStyle);
                                GUILayout.EndVertical();

                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.yellow;

                                GUILayout.Label("Controllers Available", labelStyle);

                                GUILayout.Space(5);


                                leftattScrollPosition = GUILayout.BeginScrollView(leftattScrollPosition);

                                foreach (AttitudeControlType attitudecontroller in leftattList.ToList())
                                {
                                        if (GUILayout.Button(attitudecontroller.ToString()))
                                        {
                                                rightattList.Add(attitudecontroller);
                                                rightattList.Sort();

                                                leftattList.Remove(attitudecontroller);
                                                leftattList.Sort();


                                        }

                                }

                                GUILayout.EndScrollView();


                                GUILayout.EndVertical();




                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical();

                                GUILayout.Space(1);
                                labelStyle.normal.textColor = Color.cyan;
                                GUILayout.Label(">", labelStyle);
                                GUILayout.Space(1);
                                GUILayout.Label("<", labelStyle);

                                GUILayout.EndVertical();

                                GUILayout.FlexibleSpace();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.green;
                                GUILayout.Label("Controller On Board", labelStyle);

                                GUILayout.Space(5);

                                rightattScrollPosition = GUILayout.BeginScrollView(rightattScrollPosition, false, false);

                                foreach (AttitudeControlType attitudecontroller in rightattList.ToList())
                                {
                                        if (GUILayout.Button(attitudecontroller.ToString()))
                                        {
                                                leftattList.Add(attitudecontroller);
                                                leftattList.Sort();

                                                rightattList.Remove(attitudecontroller);
                                                rightattList.Sort();


                                        }

                                }

                                GUILayout.EndScrollView();

                                GUILayout.Space(5);



                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                        }
                        


                        if (GUILayout.Button("Save Loadout"))
                        {

                                SaveLoadout();

                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUIControlLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }





                void InitController(AscentProAPGCSModule module)
                {
                        foreach (AttitudeControlType attitude in (AttitudeControlType[])Enum.GetValues(typeof(AttitudeControlType)))
                        {
                                leftattList.Add(attitude);
                        }

                }
                
                void LoadFromPartModule(AscentProAPGCSModule module)
                {
                        rightList = module.SequenceEngine.ControllerModules.Keys.ToList();
                        rightList.Sort();

                }

                void EnumTypes()
                {

                        foreach (ControlType control in (ControlType[])Enum.GetValues(typeof(ControlType)))
                        {
                                leftList.Add(control);
                        }
                        leftList = leftList.Except(rightList).ToList();
                        
                        leftList.Sort();
                }


                void SaveLoadout()
                {


                        module.SequenceEngine.ControllerModules.Clear();

                        foreach(ControlType control in rightList)
                        {

                                module.SequenceEngine.AddControl(control);

                                        
                        }

                        

                        
                
                }


        }
}
