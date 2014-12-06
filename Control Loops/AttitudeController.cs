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

                protected Vector3d attitude;
                protected Quaternion rotation;
                protected bool _isEngaged;
                
                internal bool isEngaged
                {
                        get { return _isEngaged; }

                        set 
                        {
                                if (value && !_isEngaged)
                                {
                                        _isEngaged = true;
                                        module.vessel.OnFlyByWire += new FlightInputCallback(Controller);
                                }
                                else
                                {
                                        _isEngaged = false;
                                        module.vessel.OnFlyByWire -= new FlightInputCallback(Driver);
                                }
                        
                        }
                }


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

                public void ActiveController(FlightCtrlState s)
                {

                }


        }
}
