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

                Rect WindowRect = new Rect(200, 100, 450, 600);

                string windowTitle = "Control Loadout Window";

                Vector2 controlLeftScrollPosition;
                Vector2 controlRightScrollPosition;
                List<ControlType> controlLeftList = new List<ControlType>();
                List<ControlType> controlRightList = new List<ControlType>();

                Vector2 attitudeLeftScrollPosition;
                Vector2 attitudeRightScrollPosition;
                List<AttitudeControlType> attitudeLeftList = new List<AttitudeControlType>();
                List<AttitudeControlType> attitudeRightList = new List<AttitudeControlType>();

                Vector2 sensorLeftScrollPosition;
                Vector2 sensorRightScrollPosition;
                List<SensorType> sensorLeftList = new List<SensorType>();

                int maxAttitudeControllers = 1;

                //Styles
                GUIStyle labelStyle = new GUIStyle();


                internal GUIControlLoadoutEditor()
                {
                        

                }

                internal void InitWindow(AscentProAPGCSModule module)
                {
                        this.module = module;

                        LoadFromPartModule(module);
                        EnumTypes();


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

                                                GUILayout.Label("Controls Available", labelStyle);

                                                GUILayout.Space(5);


                                                controlLeftScrollPosition = GUILayout.BeginScrollView(controlLeftScrollPosition);

                                                foreach (ControlType control in controlLeftList.ToList())
                                                {
                                                        if(GUILayout.Button(control.ToString()))
                                                        {
                                                                controlRightList.Add(control);
                                                                controlRightList.Sort();

                                                                controlLeftList.Remove(control);
                                                                controlLeftList.Sort();

                                                                AddControl(control);


                                                                

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
                                                GUILayout.Label("Control Loadout", labelStyle);

                                                GUILayout.Space(5);

                                                controlRightScrollPosition = GUILayout.BeginScrollView(controlRightScrollPosition, false, false);

                                                foreach (ControlType control in controlRightList.ToList())
                                                {
                                                        if (GUILayout.Button(control.ToString()))
                                                        {
                                                                controlLeftList.Add(control);
                                                                controlLeftList.Sort();

                                                                controlRightList.Remove(control);
                                                                controlRightList.Sort();

                                                                RemoveControl(control);

                                                        }

                                                }
                        
                                                GUILayout.EndScrollView();

                                                GUILayout.Space(10);



                                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        
                        if (controlRightList.Contains(ControlType.ATTITUDE))
                        {
                                

                                GUILayout.BeginVertical();
                                GUILayout.Label("ATTITUDE", labelStyle);
                                GUILayout.EndVertical();

                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.yellow;

                                GUILayout.Label("Controllers Available", labelStyle);

                                GUILayout.Space(5);


                                attitudeLeftScrollPosition = GUILayout.BeginScrollView(attitudeLeftScrollPosition);

                                foreach (AttitudeControlType attitude in attitudeLeftList.ToList())
                                {

                                                if (GUILayout.Button(attitude.ToString()))
                                                {

                                                        if (attitudeRightList.Count < maxAttitudeControllers)
                                                        {
                                                                attitudeRightList.Add(attitude);
                                                                attitudeRightList.Sort();

                                                                attitudeLeftList.Remove(attitude);
                                                                attitudeLeftList.Sort();


                                                                AddAttitudeController(attitude);
                                                        
                                                        }


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

                                attitudeRightScrollPosition = GUILayout.BeginScrollView(attitudeRightScrollPosition, false, false);

                                foreach (AttitudeControlType attitude in attitudeRightList.ToList())
                                {
                                        if (GUILayout.Button(attitude.ToString()))
                                        {
                                                attitudeLeftList.Add(attitude);
                                                attitudeLeftList.Sort();

                                                attitudeRightList.Remove(attitude);
                                                attitudeRightList.Sort();

                                                RemoveAttitudeController();

                                        }

                                }

                                GUILayout.EndScrollView();

                                GUILayout.Space(5);



                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                        }



                        if (controlRightList.Contains(ControlType.SENSOR))
                        {
                                GUILayout.BeginVertical();
                                GUILayout.Label("SENSORS", labelStyle);
                                GUILayout.EndVertical();

                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical(GUILayout.Width(200));

                                labelStyle.normal.textColor = Color.yellow;

                                GUILayout.Label("Sensors Available", labelStyle);

                                GUILayout.Space(5);


                                sensorLeftScrollPosition = GUILayout.BeginScrollView(sensorLeftScrollPosition);

                                foreach (SensorType sensor in sensorLeftList.ToList())
                                {
                                        if (GUILayout.Button(sensor.ToString()))
                                        {
                                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(sensor);

                                                sensorLeftList.Remove(sensor);
                                                sensorLeftList.Sort();

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
                                GUILayout.Label("Sensors On Board", labelStyle);

                                GUILayout.Space(5);

                                sensorRightScrollPosition = GUILayout.BeginScrollView(sensorRightScrollPosition, false, false);

                                foreach (SensorType sensor in module.SequenceEngine.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>())
                                {

                                        if(sensor != SensorType.TIME)
                                                if (GUILayout.Button(sensor.ToString()))
                                                {
                                                        module.SequenceEngine.ControllerModules[ControlType.SENSOR].RemoveType<SensorType>(sensor);

                                                        sensorLeftList.Add(sensor);
                                                        sensorLeftList.Sort();

                                                }





                                }

                                GUILayout.EndScrollView();

                                GUILayout.Space(5);



                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                        }



                        if (GUILayout.Button("Save Configuration"))
                        {


                                UnityEngine.Object.Destroy(gameObject.GetComponent<GUIControlLoadoutEditor>());
                                
                        
                        }

                        GUILayout.EndVertical();


                        GUI.DragWindow(new Rect(0, 0, 10000, 20));
                }



                
                void LoadFromPartModule(AscentProAPGCSModule module)
                {
                        controlRightList = module.SequenceEngine.ControllerModules.Keys.ToList();
                        controlRightList.Sort();






                }

                void EnumTypes()
                {

                        foreach (ControlType control in (ControlType[])Enum.GetValues(typeof(ControlType)))
                        {
                                controlLeftList.Add(control);
                        }
                        controlLeftList = controlLeftList.Except(controlRightList).ToList();
                        
                        controlLeftList.Sort();



                        EnumAttitudeTypes();


                        EnumSensorTypes();



                }

                void EnumSensorTypes()
                {
                        sensorLeftList.Clear();
                        foreach (SensorType sensor in (SensorType[])Enum.GetValues(typeof(SensorType)))
                        {
                                sensorLeftList.Add(sensor);
                        }


                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.SENSOR))
                                sensorLeftList = sensorLeftList.Except(module.SequenceEngine.ControllerModules[ControlType.SENSOR].GetLoadedTypes<List<SensorType>>()).ToList();

                        sensorLeftList.Remove(SensorType.TIME);
                        sensorLeftList.Sort();
                
                }


                void EnumAttitudeTypes()
                {
                        attitudeLeftList.Clear();
                        attitudeRightList.Clear();
                        foreach (AttitudeControlType attitude in (AttitudeControlType[])Enum.GetValues(typeof(AttitudeControlType)))
                        {
                                attitudeLeftList.Add(attitude);
                        }

                        if (module.SequenceEngine.ControllerModules.ContainsKey(ControlType.ATTITUDE))
                        {
                                attitudeRightList.Add(module.SequenceEngine.ControllerModules[ControlType.ATTITUDE].GetLoadedTypes<AttitudeControlType>());
                                attitudeLeftList = attitudeLeftList.Except(attitudeRightList).ToList();
                        }

                        attitudeLeftList.Sort();
                
                }

                void AddControl(ControlType control)
                {
                        if (!module.SequenceEngine.ControllerModules.ContainsKey(control))
                        {
                                switch (control)
                                {
                                        case ControlType.ATTITUDE:
                                                //module.SequenceEngine.ControllerModules.Add(control, new ControlAttitude());
                                                EnumAttitudeTypes();
                                                break;
                                        case ControlType.SENSOR:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlSensors());
                                                module.SequenceEngine.ControllerModules[ControlType.SENSOR].AddType<SensorType>(SensorType.TIME);  
                                                EnumSensorTypes();
                                                break;
                                        case ControlType.MISSIONLOG:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlMissionLog());
                                                break;
                                        case ControlType.TELEMETRY:
                                                module.SequenceEngine.ControllerModules.Add(control, new ControlTelemetry());
                                                break;
                                }
                        }



                }

                void RemoveControl(ControlType control)
                {
                        module.SequenceEngine.ControllerModules.Remove(control);
                }

                void AddAttitudeController(AttitudeControlType controller)
                {
                        switch (controller)
                        {
                                case AttitudeControlType.SAS:
                                        module.SequenceEngine.ControllerModules.Add(ControlType.ATTITUDE, new SASController());
                                        break;


                        }


                }

                void RemoveAttitudeController()
                {
                        module.SequenceEngine.ControllerModules.Remove(ControlType.ATTITUDE);

                }

                void AddSensor()
                { 
                
                }

                void RemoveSensor()
                { 
                
                }

                void SaveLoadout()
                {

                        /*
                        module.SequenceEngine.ControllerModules.Clear();

                        foreach(ControlType control in controlRightList)
                        {

                                module.SequenceEngine.AddControl(control);

                                        
                        }

                        */

                        
                
                }






        }
}
