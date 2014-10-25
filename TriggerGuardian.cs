using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        internal class TriggerGuardian
        {
                ActionExecutor actionExecutor;
                internal Dictionary<int, Trigger> tdictionary;
                Vessel vessel;

                bool isascending = false;
                double lastaltitude;

                internal TriggerGuardian(Dictionary<int, Trigger> triggerdictionary, ActionExecutor actionloop)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        this.actionExecutor = actionloop;
                        this.tdictionary = triggerdictionary;
                }

                internal void AssignToVessel(Vessel v)
                {
                        this.vessel = v;
                }
                public void TriggerLoop()
                {
                        
                        isascending = (vessel.altitude > lastaltitude ? true : false);

                        lastaltitude = vessel.altitude;

                        foreach (KeyValuePair<int, Trigger> trigger in tdictionary.Where(pair => pair.Value.activated == false))
                        { 
                                
                                if(trigger.Value.Evaluate(vessel))
                                {
                                        Debug.Log(trigger.Key);
                                        actionExecutor.ExecuteActions(trigger.Key);
                                }
                        
                        }


                }




        }




}

