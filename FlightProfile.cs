using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{       
        
        class FlightProfile
        {
                internal TriggerGuardian triggerGuardian;
                //FlightController flightController;
                internal FlightLog flightLog;
                //internal Vessel vessel = null;

                internal FlightProfile(TriggerGuardian tGuard)
                {
                        this.triggerGuardian = tGuard;
                }

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
