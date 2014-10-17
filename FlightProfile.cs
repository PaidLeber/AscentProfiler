using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class FlightProfile : MonoBehaviour
        {

                internal ActionExecutor actionExecutor;
                internal TriggerGuardian triggerGuardian;
                //internal FlightController flightController;
                internal FlightLog flightLog;
                internal Vessel vessel = null;

                void Awake()
                {
                        this.Reset();
                }

                void Update()
                { 
                      
                }
                
                void FixedUpdate()
                {
                        triggerGuardian.TriggerLoop();
                }

                void OnDestroy()
                {

                        triggerGuardian = null;
                        actionExecutor = null;
                        //flightController = null;
                        flightLog = null;

                }

                public void Reset()
                {
                        enabled = false;
                        actionExecutor = new ActionExecutor();
                        triggerGuardian = new TriggerGuardian(actionExecutor);
                        //flightController = new FlightController();
                        flightLog = new FlightLog();
                        vessel = null;
                }

        }
}
