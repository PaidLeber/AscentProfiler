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
                Queue<double> transmitDelay = new Queue<double>();
                Queue<int> delayedFlightLogReadCount = new Queue<int>();


                internal bool Receive(double tranmitime ,FlightRecorder flightrecorder)
                {
                        transmitDelay.Enqueue(tranmitime);
                        delayedFlightLogReadCount.Enqueue(flightrecorder.FlightLog.Count);

                        FlightLog.AddRange(flightrecorder.FlightLog.GetRange(FlightLog.Count + 1, flightrecorder.FlightLog.Count));
                        


                        return false;
                }



                void Update()
                {
                        if(transmitDelay.Peek() )


                }

        }
}
