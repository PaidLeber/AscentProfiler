using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ControlSensors : ControlModule
        {
                SensorPackage sensorsSuite;

                internal bool sensorsEnabled = false;
                internal Dictionary<SensorType, List<double>> sensorsOnBoard = new Dictionary<SensorType, List<double>>();
                internal bool isSensorsDataReadyToTransmit = false;
                int sensorstep = 0;
                int sensorrate = 1; //per second
                double sensorlast;

                internal ControlSensors()
                {
                        sensorsSuite = new SensorPackage();
                }

                internal override void Process()
                { 
                
                }

                internal bool AddSensor(SensorType sensor)
                {

                        sensorsOnBoard.Add(sensor, new List<double> { });
                        return true;
                }

                internal void ReadSensors()
                {
                        if (!sensorsEnabled || module.vessel.missionTime == 0)
                                return;

                        if (Planetarium.GetUniversalTime() > sensorlast + sensorrate)
                        {
                                foreach (SensorType sensor in sensorsOnBoard.Keys)
                                {
                                        sensorsOnBoard[sensor].Add(sensorsSuite.GetSensorData(module, sensor));

                                        Debug.Log(sensor.ToString() + ": COUNT: " + (sensorsOnBoard[sensor].Count - 1));
                                        Debug.Log(sensor.ToString() + ": VALUE: " + (sensorsOnBoard[sensor][sensorsOnBoard[sensor].Count - 1]));

                                }

                                sensorlast = Planetarium.GetUniversalTime();
                                sensorstep++;
                        }




                }


        }
}
