using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        public class TriggerGuardian
        {
                ActionExecutor actionExecutor;
                internal Dictionary<int, Trigger> tdictionary = new Dictionary<int, Trigger>();


                bool isascending = false;
                double lastaltitude;

                public TriggerGuardian(ActionExecutor ae)
                {
                        this.actionExecutor = ae;

                }

                public void TriggerLoop()
                {
                        
                        isascending = (FlightGlobals.ship_altitude > lastaltitude ? true : false);

                        lastaltitude = FlightGlobals.ship_altitude;

                        foreach (KeyValuePair<int, Trigger> trigger in tdictionary.Where(pair => pair.Value.activated == false))
                        { 
                                if(trigger.Value.Evaluate(isascending))
                                {
                                        Debug.Log(trigger.Key);
                                        actionExecutor.ExecuteActions(trigger.Key);
                                }
                        
                        }


                }




        }




}

