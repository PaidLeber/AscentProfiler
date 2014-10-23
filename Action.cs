using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        enum ActionType
        {
                None,
                ACTIONGROUP,
                STAGE,
                THROTTLE
        }

        enum ActionModifier
        { 
                None,
                ACTIVATE,
                DEACTIVATE,
                ON,
                OFF,
                TOGGLE,
                NEXT
        }

       

        public abstract class Action
        {
                internal bool activated = false;
                internal int index = -1;

                protected ActionType type;

                internal string displayvalue;
                internal string description;

                protected int value;
                protected bool state = false;

                internal abstract bool Execute(); 

        }

        class ActionGroup : Action
        {
                enum ActionGroupObject
                { 
                        Stage,
                        Gear,
                        Light,
                        RCS,
                        SAS,
                        Brakes,
                        Abort,
                        Custom01,
                        Custom02,
                        Custom03,
                        Custom04,
                        Custom05,
                        Custom06,
                        Custom07,
                        Custom08,
                        Custom09,
                        Custom10
                }

                internal ActionGroup(int index, ActionType type, ActionGroupObject obj, ActionModifier modifier, string description, int value)
                {
                        this.index = index;
                        this.state = state;
                        this.description = description;
                        this.value = value;
                
                }

                public override bool Execute()
                {
                        //FlightLog.Log(desc.ToUpper() + " " + value +" TRIGGERED");
                        FlightGlobals.ActiveVessel.ActionGroups.SetGroup(Util.SetActionGroup(value), true);
                        return activated = true;
                        //KSPActionGroup.Brakes
                        //KSPActionGroup.Stage 
                        // etc...
                        //v.ActionGroups.SetGroup(KSPActionGroup.SAS, false);

                        // Modifiers: ActionGroup: Activate, Deactivate, Toggle
                        
                }
        }

        public sealed class ActionGroupToggle : Action
        {
                public override string type { get { return "AGTOGGLE"; } }
                public override string desc { get { return "Action Group Toggle"; } }

                public ActionGroupToggle(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }


                public override bool Execute()
                {
                        //FlightLog.Log(desc.ToUpper() + " " + value);
                        FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(Util.SetActionGroup(value));
                        return activated = true;

                }
        }

        public sealed class StageNext : Action
        {
                public override string type { get { return "STAGENEXT"; } }
                public override string desc { get { return "Stage Separation"; } }

                public StageNext(int index)
                {
                        this.index = index;
                
                }


                public override bool Execute()
                {
                        //FlightLog.Log("STAGE SEPARATION");
                        Staging.ActivateNextStage();
                        return activated = true;
                        
                        
                }
        }

        public sealed class ActivateStage : Action
        {
                public override string type { get { return "STAGE"; } }
                public override string desc { get { return "Stage Separation"; } }

                public ActivateStage(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }

                public override bool Execute()
                {
                        //FlightLog.Log(value + " " + desc.ToUpper());
                        Staging.ActivateStage(value);
                        return activated = true;

                }
        }


        public sealed class Throttle : Action
        {
                public override string type { get { return "THROTTLE"; } }
                public override string desc { get { return "Throttle Set"; } }
                public float floatvalue;

                public Throttle(int index, int value)
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

                public override bool Execute()
                {

                        FlightInputHandler.state.mainThrottle = floatvalue;
                        //FlightLog.Log(value + "THROTTLE SET "+ value + "%");
                        return activated = true;
                        

                }
        }




}
