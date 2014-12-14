using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        [Serializable]
        class ControlAttitude : ControlModule
        {
                protected AttitudeControlType ActiveController;

                protected float heading;
                protected float pitch;
                protected float roll;

                internal ControlAttitude()
                {

                }

                internal override T GetLoadedTypes<T>()
                {
                        throw new NotImplementedException();
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
                        throw new NotImplementedException();
                }

                internal override void ClearTypes()
                {
                        throw new NotImplementedException();
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




        }
}
