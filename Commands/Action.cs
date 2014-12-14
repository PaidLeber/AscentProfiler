using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        [Serializable]
        abstract class Action
        {
                internal ActionType type;
                internal ActionModifier modifier;

                internal bool activated = false;
                internal int index = -1;

                protected int value;
                protected bool state = false;

                internal string displayvalue = "";
                internal string description;

                internal abstract bool Execute(AscentProAPGCSModule module);

                protected bool SetModifierState(ActionModifier modifier)
                {
                        switch (modifier)
                        {
                                case ActionModifier.ACTIVATE:
                                        return true;
                                case ActionModifier.ON:
                                        return true;
                                case ActionModifier.DEACTIVATE:
                                        return false;
                                case ActionModifier.OFF:
                                        return false;
                                default:
                                        return false;
                        }

                }

                protected string StateToString(bool state)
                {
                        return state ? "Active" : "Inactive";
                
                }


        }

        [Serializable]
        class Control : Action
        {
                ControlType control;
                AttitudeControlType controller;
                string error;

                internal Control(int index, ActionType type, ControlType control, AttitudeControlType controller, ActionModifier modifier)
                {
                        this.index = index;
                        this.type = type;
                        this.state = SetModifierState(modifier);
                        this.control = control;
                        this.controller = controller;
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {
                        /*
                        switch(control)
                        {
                                case ControlType.TELEMETRY:

                                        module.telemetryController = new ControlTelemetry(module);
                                        module.telemetryController.Enabled = state;
                                        break;

                                case ControlType.ATTITUDE:

                                        switch (controller)
                                        {
                                                case ControllerType.SAS:
                                                        module.attitudeController = new SASController(module);
                                                        module.attitudeController.Enabled = state;
                                                        break;

                                                default:
                                                        error = " : Not  a valid attitude controller";
                                                        break;
                                        }
                                        break;
                        
                        }
                        */
                        //module.telemetryController.AddLog(type.ToString() + " -> " + control.ToString() + " : " + controller.ToString() + " : " + StateToString(state) + error);
                        return activated = true;
                }


        }

        [Serializable]
        class Telemetry : Action
        {
                TelemetryType telemetry;

                internal Telemetry(int index, ActionType type, TelemetryType telemetry, ActionModifier modifier)
                {
                        this.index = index;
                        this.type = type;
                        this.state = SetModifierState(modifier);
                        this.telemetry = telemetry;
                        this.modifier = modifier;
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {
                                
                        switch (telemetry)
                        {
                                case TelemetryType.MISSIONLOG:
                                        //module.telemetryController.isMissionLogEnabled = state;
                                        break;
                                
                                case TelemetryType.SENSORS:
                                       // module.telemetryController.sensorsEnabled = state;
                                        Debug.Log("SENSORS STATE: " + state);
                                        break;

                                case TelemetryType.TRANSMIT:
                                        //module.telemetryController.isSensorsDataReadyToTransmit = state;
                                        Debug.Log("TRANSMIT STATE: " + state);
                                        break;

                        }
                        //module.telemetryController.AddLog("Telemetry -> " + telemetry.ToString() + " : " + StateToString(state));
                        return activated = true;
                }
        
        }

        [Serializable]
        class Sensors : Action
        {
                SensorType sensor;

                internal Sensors(int index, ActionType type, SensorType sensor)
                {
                        this.index = index;
                        this.type = type;
                        this.sensor = sensor;
                        this.state = true;
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {

                        if (!AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.FerramAerospaceResearch))
                        {
                                foreach (FARSensorType farsensor in (FARSensorType[])Enum.GetValues(typeof(FARSensorType)))
                                {
                                        if (farsensor.ToString() == sensor.ToString())
                                        {
                                                //module.telemetryController.AddLog("Sensors -> " + sensor.ToString() + " : " + StateToString(state) + " : FAR not installed");
                                                return activated = true;
                                        }


                                }

                        }

                        //module.telemetryController.AddSensor(sensor);
                        //module.telemetryController.AddLog("Sensors -> " + sensor.ToString() + " : " + StateToString(state));

                        return activated = true;
                }


        }

        [Serializable]
        class ActionGroup : Action
        {
                KSPActionGroup actiongroupValue;

                internal ActionGroup(int index, ActionType type, ActionModifier modifier, KSPActionGroup kspactiongroupEnum, string description)
                {
                        this.index = index;
                        this.type = type;
                        this.state = SetModifierState(modifier);
                        this.modifier = modifier;
                        this.actiongroupValue = kspactiongroupEnum;
                        this.description = description;
                
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {
                        switch(modifier)
                        {

                                case ActionModifier.TOGGLE:

                                        FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(actiongroupValue);
                                        return activated = true;

                                default:

                                        FlightGlobals.ActiveVessel.ActionGroups.SetGroup(actiongroupValue, state);
                                        return activated = true;
                        }

                        
                        
                        //FlightGlobals.ActiveVessel.ActionGroups.SetGroup(Util.SetActionGroup(value), true);
                        
                        //KSPActionGroup.Brakes
                        //KSPActionGroup.Stage 
                        // etc...
                        //v.ActionGroups.SetGroup(KSPActionGroup.SAS, false);

                        // Modifiers: ActionGroup: Activate, Deactivate, Toggle
                        
                }

        }

        [Serializable]
        class ActionGroupToggle : Action
        {


                internal ActionGroupToggle(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }


                internal override bool Execute(AscentProAPGCSModule module)
                {
                        //FlightLog.Log(desc.ToUpper() + " " + value);
                        FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(Util.SetActionGroup(value));
                        return activated = true;

                }
        }

        [Serializable]
        class StageNext : Action
        {


                internal StageNext(int index)
                {
                        this.index = index;
                
                }


                internal override bool Execute(AscentProAPGCSModule module)
                {
                        //FlightLog.Log("STAGE SEPARATION");
                        Staging.ActivateNextStage();
                        return activated = true;
                        
                        
                }
        }

        [Serializable]
        class ActivateStage : Action
        {

                internal ActivateStage(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {
                        //FlightLog.Log(value + " " + desc.ToUpper());
                        Staging.ActivateStage(value);
                        return activated = true;

                }
        }


        [Serializable]
        class Throttle : Action
        {
                float floatvalue;

                internal Throttle(int index, int value)
                {
                        this.index = index;
                        this.value = value;

                        if (value < 100) //move this convert code to action factory
                        {
                                this.floatvalue = (float)(value / 100);
                        }
                        else
                        {
                                this.floatvalue = 1F;
                        }

                }

                internal override bool Execute(AscentProAPGCSModule module)
                {

                        FlightInputHandler.state.mainThrottle = floatvalue;
                        //FlightLog.Log(value + "THROTTLE SET "+ value + "%");
                        return activated = true;
                        

                }
        }




}
