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

                private Vessel flybywireVessel;                               //vessel change test
                //internal bool Enabled;
                protected float heading;
                protected float pitch;
                protected float roll;

                private bool _isEngaged;
                internal bool isEngaged
                {
                        get { return _isEngaged; }

                        set
                        {
                                Debug.Log("isengaged: " + value + " :" + _isEngaged);
                                if (value && !_isEngaged)
                                {
                                        Debug.Log("enteriing isengaged");
                                        if (flybywireVessel == null)
                                        {
                                                Debug.Log("created flybywire vessel copy");
                                                flybywireVessel = module.vessel;
                                        }
                                        else if (flybywireVessel != module.vessel)
                                        {
                                                module.flightTelemetry.AddLog("Vessel has changed. Handle it.");
                                                Debug.Log("Vessel has changed. Handle it.");
                                                _isEngaged = false;
                                        }
                                        Debug.Log("turning on flybywire");
                                        flybywireVessel.OnFlyByWire -= ActiveController;
                                        flybywireVessel.OnFlyByWire += ActiveController;
                                        _isEngaged = true;
                                }
                                else if (!value && _isEngaged)
                                {
                                        Debug.Log("disable engage");
                                        _isEngaged = false;
                                        module.vessel.OnFlyByWire -= ActiveController;
                                        module.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                                }

                        }
                }


                internal AttitudeController()
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

                public virtual void ActiveController(FlightCtrlState s)
                {

                }



        }
}
