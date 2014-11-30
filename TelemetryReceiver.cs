using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class TelemetryReceiver : MonoBehaviour
        {

                internal List<string> missionLog = new List<string>();
                internal int missionLogCurrentReadCount = 0;
                Queue<double> missionLogTransmitDelay = new Queue<double>();
                Queue<int> missionLogDelayedReadCount = new Queue<int>();

                internal bool ReceiveMissionLog(double transmitdelay, List<string> remoteMissionLogs)
                {
                        Debug.Log("Received Flight Log");
                        missionLogTransmitDelay.Enqueue(transmitdelay);
                        Debug.Log("Transmit delay: "+ transmitdelay);
                        
                        missionLogDelayedReadCount.Enqueue(remoteMissionLogs.Count);
                        Debug.Log("Transmit delay count: " + missionLogTransmitDelay.Count);
                        Debug.Log("TelemetryReceiver.flightlog Count: " + missionLog.Count);
                        Debug.Log("telemetrydata.flightlog Count: " + remoteMissionLogs.Count);

                        missionLog.AddRange(remoteMissionLogs.GetRange(missionLog.Count, remoteMissionLogs.Count - missionLog.Count));
                        Debug.Log("NEW TelemetryReceiver.flightlog Count: " + missionLog.Count);
                        return true;
                }

                internal bool ReceiveTelemetryData(double transmitdelay, Dictionary<SensorType, double[]> sensorsData)
                {
                        Debug.Log("Received Telemetry Data");



                        return true;
                }

                void CheckForMissionLogsInTransit()
                {
                        if (missionLogTransmitDelay.Count != 0)
                        {
                                if (Planetarium.GetUniversalTime() > missionLogTransmitDelay.Peek())
                                {
                                        missionLogTransmitDelay.Dequeue();
                                        missionLogCurrentReadCount = missionLogDelayedReadCount.Peek();
                                        missionLogDelayedReadCount.Dequeue();
                                        Debug.Log("transmit delay count: " + missionLogTransmitDelay.Count);

                                }
                        }

                }

                public void Update()
                {
                        //Debug.Log("TELEMETRY RECEIVE UPDATE");
                        CheckForMissionLogsInTransit();

                }
                

        }
}
