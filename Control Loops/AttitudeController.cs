using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class AttitudeController
        {
                AttitudeControlType control;

                Vector3d attitude;
                Quaternion rotation;
                bool isEnabled;


                internal AttitudeController()
                { 
                
                
                }

                internal bool SetAttitude(Vector3d attitude, bool hold)
                {
                        
                        return true;
                }

                internal bool SetHeading(double heading, bool hold)
                {
                        return true;
                }

                internal bool SetPitch(double pitch, bool hold)
                {
                        return true;

                }

                internal bool SetRoll(double roll, bool hold)
                {
                        return true;
                }

        }
}
