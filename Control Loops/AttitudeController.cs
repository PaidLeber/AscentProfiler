using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class AttitudeController
        {
                protected AscentProAPGCSModule module;

                protected Vessel flybywireVessel;                               //vessel change test
                protected Vector3d attitude;
                protected Quaternion rotation;
                protected bool _isEngaged = false;
                internal bool isEngaged;
                protected float heading;
                protected float pitch;
                protected float roll;

                internal AttitudeController()
                {
                }


                internal void SetAttitude(Vector3d attitude)
                {
                        SetHeading(attitude.x);
                        SetPitch(attitude.y);
                        SetRoll(attitude.z);

                }

                internal void SetHeading(double heading)
                {
                        this.heading = Double.IsNaN(heading) ? 0f : (float) heading;

                }

                internal void SetPitch(double pitch)
                {
                        this.pitch = Double.IsNaN(pitch) ? 0f : (float)pitch;
                }

                internal void SetRoll(double roll)
                {
                        this.roll = Double.IsNaN(roll) ? 0f : (float)roll;
                }

                public virtual void ActiveController(FlightCtrlState s)
                {

                }



        }
}
