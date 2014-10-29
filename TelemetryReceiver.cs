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
                        Debug.Log("Received Flight Log");
                        transmitDelay.Enqueue(transmitdelay);
                        Debug.Log("Transmit delay: "+ transmitdelay);
                        
                        delayedFlightLogReadCount.Enqueue(flightrecorder.FlightLog.Count);
                        Debug.Log("Transmit delay count: " + transmitDelay.Count);
                        Debug.Log("TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        Debug.Log("flightrecorder.flightlog Count: " + flightrecorder.FlightLog.Count);
                        FlightLog.AddRange(flightrecorder.FlightLog.GetRange(FlightLog.Count, flightrecorder.FlightLog.Count));
                        Debug.Log("NEW TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        return true;
                }



                void Update()
                {
                        if (Planetarium.GetUniversalTime() > transmitDelay.Peek())
                        {
                                transmitDelay.Dequeue();
                                currentflightlogReadCount = delayedFlightLogReadCount.Peek();
                                delayedFlightLogReadCount.Dequeue();
                                Debug.Log("transmit delay count: "+transmitDelay.Count);

                        }

                }


        }
}
