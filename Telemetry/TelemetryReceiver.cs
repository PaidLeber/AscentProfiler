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

                //On RX Sequence of New Sequence
                private List<object[]> listRXReceiverMessage = new List<object[]>();
                private Sequence newSequence;
                private double sequenceTransmissionTime = 0;
                private int sequenceMessageOrder = 0;

                public TelemetryReceiver()
                {
                        InitNewSequenceMessage();
                }

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
                        telemetryDataInTransit = new Dictionary<SensorType,List<double>>(sensorsData);
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
                                        telemetryData = new Dictionary<SensorType,List<double>>(telemetryDataInTransit);
                                        telemetryDataInTransit = null;
                                        Debug.Log("NEW TELEMETRY DATA LOADED!");

                                }
                        }


                }



                void LoadNewSequence(Sequence newsequence)
                {
                        //telemetryController.sensorsOnBoard.Clear();
                        //sequence = newsequence;
                        //sequence.AssignToModule(this);
                        //sequence.Enabled = true;
                        //sequence.ExecuteActions(0);

                }

                internal bool RXNewSequence(Sequence newsequence)
                {
                        if (AscentProfiler.listRegisteredAddons.Contains(RegisteredAddons.RemoteTech))
                        {
                                sequenceMessageOrder = 0;
                                sequenceTransmissionTime = 0;
                                newSequence = newsequence;
                                Debug.Log("RX Sequence Transfer successful");
                        }
                        else
                        {
                                LoadNewSequence(newsequence);

                        }



                        return true;
                }

                void RXSequenceReceiver()
                {

                        if (sequenceTransmissionTime == 0)
                        { sequenceTransmissionTime = Planetarium.GetUniversalTime(); }


                        if (sequenceMessageOrder == 6)
                        {
                                newSequence = null;
                                return;
                        }


                        var messagearray = listRXReceiverMessage[sequenceMessageOrder];

                        /*
                        if (Planetarium.GetUniversalTime() > sequenceTransmissionTime + Convert.ToDouble(messagearray[1]) + RemoteTech.API.GetSignalDelayToKSC(vessel.id))
                        {
                                Debug.Log("sequence#: " + sequenceMessageOrder + " seqdelaytime: " + messagearray[1]);
                                if (sequenceMessageOrder == 4)
                                {
                                        LoadNewSequence(newSequence);
                                        //telemetryController.AddLog("Reconfiguration successful: sequence loaded");
                                        Log.Level(LogType.Verbose, "Sequence Loaded");
                                        //Log.Level(LogType.Verbose, "this module enabled: " + this.isEnabled);
                                }

                                ScreenMessages.PostScreenMessage(new ScreenMessage(Convert.ToString(messagearray[2]), (float)messagearray[0], ScreenMessageStyle.UPPER_LEFT));
                                sequenceTransmissionTime = Planetarium.GetUniversalTime();
                                sequenceMessageOrder++;

                        }
                        */
                }


                void InitNewSequenceMessage()
                {
                        /*
                        listRXReceiverMessage.Add(new object[] { 4.0f, 0, "RX: AEDL Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });
                        listRXReceiverMessage.Add(new object[] { 3.0f, 1, "RX: Reconfiguration packets received from frame" });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 4, "RX: Checksum verification in progress, please standby " });
                        listRXReceiverMessage.Add(new object[] { 6.0f, 6, "RX: Telemetry data buffer: Reset" });
                        listRXReceiverMessage.Add(new object[] { 8.0f, 4, "RX: Reconfiguration successful: sequence loaded" });
                        listRXReceiverMessage.Add(new object[] { 7.0f, 2, "RX: AEDL Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });
                        */

                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: AEDL Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });
                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: Reconfiguration packets received from frame" });
                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: Checksum verification in progress, please standby " });
                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: Telemetry data buffer: Reset" });
                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: Reconfiguration successful: sequence loaded" });
                        listRXReceiverMessage.Add(new object[] { 0f, 0, "RX: AEDL Telecommand Sequencing Receiver Version " + AscentProfiler.version + " Ready" });


                }



                public void Update()
                {

                        RXSequenceReceiver();
                        //Debug.Log("TELEMETRY RECEIVE UPDATE");
                        CheckForMissionLogsInTransit();
                        CheckForTelemetryDataInTransit();

                }
                

        }
}
