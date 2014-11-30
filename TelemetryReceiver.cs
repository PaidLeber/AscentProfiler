﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class TelemetryReceiver : MonoBehaviour
        {

                internal List<string> FlightLog = new List<string>();
                internal int currentflightlogReadCount = 0;
                Queue<double> transmitDelay = new Queue<double>();
                Queue<int> delayedFlightLogReadCount = new Queue<int>();

                internal bool ReceiveFlightLog(double transmitdelay, FlightTelemetry telemetryData)
                {
                        Debug.Log("Received Flight Log");
                        transmitDelay.Enqueue(transmitdelay);
                        Debug.Log("Transmit delay: "+ transmitdelay);
                        
                        delayedFlightLogReadCount.Enqueue(telemetryData.missionLog.Count);
                        Debug.Log("Transmit delay count: " + transmitDelay.Count);
                        Debug.Log("TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        Debug.Log("telemetrydata.flightlog Count: " + telemetryData.missionLog.Count);

                        FlightLog.AddRange(telemetryData.missionLog.GetRange(FlightLog.Count, telemetryData.missionLog.Count - FlightLog.Count));
                        Debug.Log("NEW TelemetryReceiver.flightlog Count: " + FlightLog.Count);
                        return true;
                }

                void CheckDataInTransit()
                {
                        if (transmitDelay.Count != 0)
                        {
                                if (Planetarium.GetUniversalTime() > transmitDelay.Peek())
                                {
                                        transmitDelay.Dequeue();
                                        currentflightlogReadCount = delayedFlightLogReadCount.Peek();
                                        delayedFlightLogReadCount.Dequeue();
                                        Debug.Log("transmit delay count: " + transmitDelay.Count);

                                }
                        }

                }

                public void Update()
                {
                        //Debug.Log("TELEMETRY RECEIVE UPDATE");
                        CheckDataInTransit();

                }
                

        }
}
