using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        class TelemetryStation : MonoBehaviour
        {
                List<string> FlightLog = new List<string>();
                Dictionary<double, int> signalDelay = new Dictionary<double, int>();


                internal bool Receive(double signaldelay ,FlightRecorder flightrecorder)
                {


                        FlightLog.AddRange(flightrecorder.FlightLog.GetRange(FlightLog.Count + 1, flightrecorder.FlightLog.Count));
                        


                        return false;
                }



                void Update()
                {

                }

        }
}
