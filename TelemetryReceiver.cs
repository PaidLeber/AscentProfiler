using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class TelemetryReceiver : MonoBehaviour
        {

                internal List<string> FlightLog = new List<string>();
                Queue<double> transmitDelay = new Queue<double>();
                Queue<int> delayedFlightLogReadCount = new Queue<int>();
                int currentflightlogReadCount = 0;


                internal bool ReceiveFlightLog(double transmitdelay, FlightRecorder flightrecorder)
                {

                        transmitDelay.Enqueue(transmitdelay);
                        delayedFlightLogReadCount.Enqueue(flightrecorder.FlightLog.Count);

                        FlightLog.AddRange(flightrecorder.FlightLog.GetRange(FlightLog.Count + 1, flightrecorder.FlightLog.Count));

                        return true;
                }



                void Update()
                {
                        if (Planetarium.GetUniversalTime() > transmitDelay.Peek())
                        {
                                transmitDelay.Dequeue();
                                currentflightlogReadCount = delayedFlightLogReadCount.Peek();
                                delayedFlightLogReadCount.Dequeue();

                        }

                }


        }
}
