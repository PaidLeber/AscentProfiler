﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{

        class SensorPackage
        {
                AscentProAPGCSModule module;

                internal SensorPackage(AscentProAPGCSModule module)
                {
                        this.module = module;
                }

                internal double GetSensorData(SensorType sensor)
                {
                        switch(sensor)
                        {
                                case SensorType.TIME:
                                        return module.vessel.missionTime;
                                case SensorType.ALTITUDE:
                                        return module.vessel.altitude;
                                case SensorType.GFORCE:
                                        return module.vessel.geeForce;
                                case SensorType.MAXQ:
                                        return ferram4.FARAPI.GetActiveControlSys_Q();
                                case SensorType.AOA:
                                        return ferram4.FARAPI.GetActiveControlSys_AoA();

                                default:
                                        return 0;
                        
                        }
                }




        }
}
