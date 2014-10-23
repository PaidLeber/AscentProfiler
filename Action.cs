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
                ActionGroup,
                Stage
        }

        enum ActionModifier
        { 
        
        }

        public abstract class Action
        {
                public abstract string type { get; }
                public abstract string desc { get; }
                public int index = -1;
                public int value = -1;
                public bool state = false;

                public abstract bool Execute(); 

        }

        public sealed class ActionGroup : Action
        {
                public override string type { get { return "ACTIONGROUP"; } }
                public override string desc { get { return "Action Group"; } }


                public ActionGroup(int index, int value)
                {
                        this.index = index;
                        this.value = value;
                
                }

                public override bool Execute()
                {
                        //FlightLog.Log(desc.ToUpper() + " " + value +" TRIGGERED");
                        FlightGlobals.ActiveVessel.ActionGroups.SetGroup(Util.SetActionGroup(value), true);
                        return state = true;
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
                        return state = true;

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
                        return state = true;
                        
                        
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
                        return state = true;

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
                        return state = true;
                        

                }
        }




}
