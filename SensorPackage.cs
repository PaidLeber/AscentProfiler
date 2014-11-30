using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{

        enum SensorType
        { 
                TIME,
                ALTITUDE,
                GFORCE

        }


        class SensorPackage
        {
                AscentProAPGCSModule module;

                internal SensorPackage(AscentProAPGCSModule module)
                {
                        this.module = module;
                }


                double GetSensorData(SensorType sensor)
                {
                        switch(sensor)
                        {
                                case SensorType.TIME:
                                        return module.vessel.missionTime;
                                case SensorType.ALTITUDE:
                                        return module.vessel.altitude;
                                case SensorType.GFORCE:
                                        return module.vessel.geeForce;

                                default:
                                        return 0;
                        
                        }
                }




        }
}
