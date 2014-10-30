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
                public Queue<double> transmitDelay = new Queue<double>();
                public Queue<int> delayedFlightLogReadCount = new Queue<int>();
                public int currentflightlogReadCount = 0;
                

                internal bool ReceiveFlightLog(double transmitdelay, FlightRecorder flightrecorder)
                {
                        Debug.Log("Received Flight Log");
                        transmitDelay.Enqueue(transmitdelay);
                        Debug.Log("Transmit delay: "+ transmitdelay);
                        
                        delayedFlightLogReadCount.Enqueue(flightrecorder.FlightLog.Count);
                        Debug.Log("Transmit delay count: " + transmitDelay.Count);
                        Debug.Log("TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        Debug.Log("flightrecorder.flightlog Count: " + flightrecorder.FlightLog.Count);
                        int tempflcount;
                        int tempfrcount;
                        if (FlightLog.Count > 0)
                        {
                                tempflcount = FlightLog.Count - 1;
                                tempfrcount = flightrecorder.FlightLog.Count - FlightLog.Count;
                        }
                        else 
                        {
                                tempflcount = FlightLog.Count;
                                tempfrcount = flightrecorder.FlightLog.Count;
                        }
                        FlightLog.AddRange(flightrecorder.FlightLog.GetRange(tempflcount, tempfrcount));
                        Debug.Log("NEW TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        return true;
                }



                void Update()
                {
                        Debug.Log("TELEMETRY RECEIVE UPDATE");
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
