using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class SASController : AttitudeController
        {
                Vector3d north;
                Vector3d up;
                Quaternion rotationtarget;
                Quaternion lastrotation;

                internal new bool isEngaged
                {
                        get { return _isEngaged; }

                        set
                        {
                                if (value && !_isEngaged)
                                {
                                        if (flybywireVessel == null)
                                        {
                                                flybywireVessel = module.vessel;
                                        }
                                        else
                                        {
                                                module.flightTelemetry.AddLog("Vessel has changed. Handle it.");
                                                Debug.Log("Vessel has changed. Handle it.");
                                                _isEngaged = false;
                                        }

                                        flybywireVessel.OnFlyByWire -= new FlightInputCallback(ActiveController);
                                        flybywireVessel.OnFlyByWire += new FlightInputCallback(ActiveController);
                                        _isEngaged = true;
                                }
                                else if (!value && _isEngaged)
                                {
                                        _isEngaged = false;
                                        module.vessel.OnFlyByWire -= new FlightInputCallback(ActiveController);
                                        module.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                                }

                        }
                }


                internal SASController(AscentProAPGCSModule module)
                {
                        this.module = module;
                }


                public override void ActiveController(FlightCtrlState s)
                {
                        Quaternion attitudetarget = 
                                Quaternion.AngleAxis(heading, Vector3.up) * 
                                Quaternion.AngleAxis(-pitch, Vector3.right) * 
                                Quaternion.AngleAxis(-roll, Vector3.forward);

                        north = Vector3d.Exclude(up, (module.vessel.mainBody.position + module.vessel.mainBody.transform.up * (float)module.vessel.mainBody.Radius) - module.vessel.findWorldCenterOfMass()).normalized;
                        up = (module.vessel.findWorldCenterOfMass() - module.vessel.mainBody.position).normalized;

                        rotationtarget = Quaternion.LookRotation(north, up) * attitudetarget * Quaternion.Euler(90, 0, 0);

                        if (!module.vessel.ActionGroups[KSPActionGroup.SAS])
                        {
                                module.vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                        }

                        if (Quaternion.Angle(lastrotation, rotationtarget) > 10)
                        {
                                module.vessel.VesselSAS.LockHeading(rotationtarget);
                                lastrotation = rotationtarget;
                        }
                        else
                        {
                                module.vessel.VesselSAS.LockHeading(rotationtarget, true);
                        }




                }
                
        }
}
