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
                Dictionary<int, Trigger> dictTrigger;

                AscentProAPGCSModule module;

                internal FlightProfile(Dictionary<int, Trigger> triggerdictionary, ActionExecutor actionloop)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        this.actionExecutor = actionloop;
                        this.dictTrigger = triggerdictionary;
                }

                public void TriggerLoop()
                {
                        
                        foreach (KeyValuePair<int, Trigger> trigger in dictTrigger.Where(pair => pair.Value.activated == false))
                        { 
                                
                                if(trigger.Value.Evaluate(module))
                                {
                                        Debug.Log(trigger.Key);
                                        actionExecutor.ExecuteActions(trigger.Key);
                                }
                        
                        }

                        
                }

                internal void AssignToModule(AscentProAPGCSModule module)
                {
                        this.module = module;
                }



        }




}

