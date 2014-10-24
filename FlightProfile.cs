using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{       
        
        class FlightProfile 
        {

                internal ActionExecutor actionExecutor;
                internal TriggerGuardian triggerGuardian;
                //internal FlightController flightController;
                internal FlightLog flightLog;
                internal Vessel vessel = null;

                public void OnAwake()
                {

                }

                public void OnStart()
                {
                        
                }

                public void OnUpdate()
                { 
                      
                }

                public void OnFixedUpdate()
                {
                        triggerGuardian.TriggerLoop();
                }



        }
}
