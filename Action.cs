using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        enum ActionType
        {
                ACTIONGROUP,
                STAGE,
                THROTTLE
        }

        enum ActionModifier
        { 
                ACTIVATE,
                DEACTIVATE,
                ON,
                OFF,
                TOGGLE,
                NEXT
        }



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


        }

        class Telemetry : Action
        {
                SensorType sensor;

                internal Telemetry(int index, SensorType sensor)
                {
                        this.sensor = sensor;
                }

                internal override bool Execute(AscentProAPGCSModule module)
                {

                        switch (sensor)
                        {
                                case SensorType.OFF:
                                        module.flightTelemetry.isSensorsEnabled = false;
                                        break;

                                case SensorType.ON:
                                        module.flightTelemetry.isSensorsEnabled = true;
                                        break;

                                case SensorType.TRANSMIT:
                                        module.flightTelemetry.isSensorsDataReadyToTransmit = true;
                                        break;

                        }


                        return true;
                }
        
        }


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

                internal override bool Execute()
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

        class ActionGroupToggle : Action
        {


                internal ActionGroupToggle(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }


                internal override bool Execute()
                {
                        //FlightLog.Log(desc.ToUpper() + " " + value);
                        FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(Util.SetActionGroup(value));
                        return activated = true;

                }
        }

        class StageNext : Action
        {


                internal StageNext(int index)
                {
                        this.index = index;
                
                }


                internal override bool Execute()
                {
                        //FlightLog.Log("STAGE SEPARATION");
                        Staging.ActivateNextStage();
                        return activated = true;
                        
                        
                }
        }

        class ActivateStage : Action
        {

                internal ActivateStage(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }

                internal override bool Execute()
                {
                        //FlightLog.Log(value + " " + desc.ToUpper());
                        Staging.ActivateStage(value);
                        return activated = true;

                }
        }


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

                internal override bool Execute()
                {

                        FlightInputHandler.state.mainThrottle = floatvalue;
                        //FlightLog.Log(value + "THROTTLE SET "+ value + "%");
                        return activated = true;
                        

                }
        }




}
