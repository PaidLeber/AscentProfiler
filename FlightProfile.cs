using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        class FlightProfile
        {
                internal bool isEnabled = false;

                ActionExecutor actionExecutor;
                List<Trigger> listTrigger;

                AscentProAPGCSModule module;

                internal FlightProfile(List<Trigger> triggerlist, ActionExecutor actionloop)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        this.actionExecutor = actionloop;
                        this.listTrigger = triggerlist;
                }

                public void TriggerLoop()
                {
                        
                        foreach (Trigger trigger in listTrigger.Where(trigger => trigger.activated == false && trigger.linkedIndex == 0))
                        { 
                                
                                if(trigger.Evaluate(module))
                                {
                                        Debug.Log(trigger.type);
                                        actionExecutor.ExecuteActions(trigger.index);
                                }
                        
                        }

                        
                }

                internal void AssignToModule(AscentProAPGCSModule module)
                {
                        this.module = module;
                }



        }




}

