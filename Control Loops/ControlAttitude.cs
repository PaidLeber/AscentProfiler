using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ControlAttitude : ControlModule
        {

                protected float heading;
                protected float pitch;
                protected float roll;

                private bool _enabled = false;
                internal bool Enabled
                {
                        get { return _enabled; }

                        set
                        { 
                                if (!value)
                                {
                                        module.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false); 
                                }
                                
                                _enabled = value;
                        }

                }




                internal ControlAttitude()
                {
                }

                internal void OnUpdate()
                {


                }


                internal void SetAttitude(Vector3 attitude)
                {
                        SetHeading(attitude.x);
                        SetPitch(attitude.y);
                        SetRoll(attitude.z);

                }

                internal void SetHeading(float heading)
                {
                        this.heading = heading;

                }

                internal void SetPitch(float pitch)
                {
                        this.pitch = pitch;
                }

                internal void SetRoll(float roll)
                {
                        this.roll = roll;
                }

                public virtual void ActiveController()
                {

                }



        }
}
