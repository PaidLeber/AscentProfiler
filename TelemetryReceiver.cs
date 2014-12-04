using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class TelemetryReceiver
        {

                internal List<string> missionLog = new List<string>();
                internal int missionLogCurrentReadCount = 0;
                Queue<double> missionLogTransitDelay = new Queue<double>();
                Queue<int> missionLogDelayedReadCount = new Queue<int>();

                internal Dictionary<SensorType, List<double>> telemetryData;
                Dictionary<SensorType, List<double>> telemetryDataInTransit;
                Queue<double> telemetryTransitDelay = new Queue<double>();

                internal bool ReceiveMissionLog(double transmitdelay, List<string> remoteMissionLogs)
                {
                        Debug.Log("Received Flight Log");
                        missionLogTransitDelay.Enqueue(transmitdelay);
                        Debug.Log("Transmit delay: "+ transmitdelay);
                        
                        missionLogDelayedReadCount.Enqueue(remoteMissionLogs.Count);
                        Debug.Log("Transmit delay count: " + missionLogTransitDelay.Count);
                        Debug.Log("TelemetryReceiver.flightlog Count: " + missionLog.Count);
                        Debug.Log("telemetrydata.flightlog Count: " + remoteMissionLogs.Count);

                        missionLog.AddRange(remoteMissionLogs.GetRange(missionLog.Count, remoteMissionLogs.Count - missionLog.Count));
                        Debug.Log("NEW TelemetryReceiver.flightlog Count: " + missionLog.Count);
                        return true;
                }

                internal bool ReceiveTelemetryData(double transmitdelay, Dictionary<SensorType, List<double>> sensorsData)
                {
                        Debug.Log("Received Telemetry Data");
                        telemetryTransitDelay.Enqueue(transmitdelay);
                        telemetryDataInTransit = sensorsData;

                        return true;
                }

                void CheckForMissionLogsInTransit()
                {
                        if (missionLogTransitDelay.Count != 0)
                        {
                                if (Planetarium.GetUniversalTime() > missionLogTransitDelay.Peek())
                                {
                                        missionLogTransitDelay.Dequeue();
                                        missionLogCurrentReadCount = missionLogDelayedReadCount.Peek();
                                        missionLogDelayedReadCount.Dequeue();
                                        Debug.Log("transmit delay count: " + missionLogTransitDelay.Count);
                                }
                        }

                }

                void CheckForTelemetryDataInTransit()
                {
                        if (telemetryTransitDelay.Count != 0)
                        {
                                if (Planetarium.GetUniversalTime() > telemetryTransitDelay.Peek())
                                {
                                        telemetryTransitDelay.Dequeue();
                                        telemetryData = telemetryDataInTransit;
                                        telemetryDataInTransit.Clear();

                                }
                        }


                }





                public void Update()
                {
                        //Debug.Log("TELEMETRY RECEIVE UPDATE");
                        CheckForMissionLogsInTransit();
                        CheckForTelemetryDataInTransit();

                }
                

        }
}
