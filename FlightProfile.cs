using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        internal class FlightProfile
        {
                internal bool isEnabled = false;

                ActionExecutor actionExecutor;
                internal Dictionary<int, Trigger> tdictionary;

                AscentProAPGCSModule module;

                internal FlightProfile(Dictionary<int, Trigger> triggerdictionary, ActionExecutor actionloop)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        this.actionExecutor = actionloop;
                        this.tdictionary = triggerdictionary;
                }

                internal void AssignToVessel(AscentProAPGCSModule module)
                {
                        this.module = module;
                }
                public void TriggerLoop()
                {
                        
                        foreach (KeyValuePair<int, Trigger> trigger in tdictionary.Where(pair => pair.Value.activated == false))
                        { 
                                
                                if(trigger.Value.Evaluate(module))
                                {
                                        Debug.Log(trigger.Key);
                                        actionExecutor.ExecuteActions(trigger.Key);
                                }
                        
                        }


                }




        }




}

