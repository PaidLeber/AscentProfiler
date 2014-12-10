using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ControlSensors : ControlModule
        {
                SensorPackage sensorsSuite = new SensorPackage();

                Dictionary<SensorType, List<double>> sensorsOnBoard = new Dictionary<SensorType, List<double>>();
                internal bool isSensorsDataReadyToTransmit = false;
                int sensorstep = 0;
                int sensorrate = 1; //per second
                double sensorlast;

                internal ControlSensors()
                {
                        sensorsSuite = new SensorPackage();
                }

                /*
                internal abstract T GetTypes<T>();
                internal abstract bool AddType<T>(T enumvalue);
                internal abstract bool RemoveType<T>(T enumvalue);
                internal abstract void Process(AscentProAPGCSModule module);
                */

                internal override T GetLoadedTypes<T>()
                {
                        List<SensorType> temp = sensorsOnBoard.Select(x => x.Key).ToList();

                        return (T) Convert.ChangeType(sensorsOnBoard.Select(x => x.Key).ToList(), typeof(T));
                }

                internal override bool AddType<T>(T enumvalue)
                {
                        throw new NotImplementedException();
                }

                internal override bool RemoveType<T>(T enumvalue)
                {
                        throw new NotImplementedException();
                }

                internal override void Process(AscentProAPGCSModule module)
                { 
                
                }

                internal bool AddSensor(SensorType sensor)
                {

                        sensorsOnBoard.Add(sensor, new List<double> { });
                        return true;
                }

                internal void ReadSensors()
                {
                        if (!Enabled || module.vessel.missionTime == 0)
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
