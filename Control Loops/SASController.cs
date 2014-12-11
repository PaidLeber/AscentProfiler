using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class SASController : ControlAttitude
        {
                Quaternion lastrotation;

                internal SASController(AscentProAPGCSModule module)
                {
                        //this.module = module;
                        //module.telemetryController.AddLog("SAS Controller Loaded!");
                }


                internal override void Process(AscentProAPGCSModule module)
                {
                        if (!Enabled)
                                return;

                        Quaternion attitudetarget =
                                Quaternion.AngleAxis(heading, Vector3.up) *
                                Quaternion.AngleAxis(-pitch, Vector3.right) *
                                Quaternion.AngleAxis(-roll, Vector3.forward);

                        Vector3d up = (module.vessel.findWorldCenterOfMass() - module.vessel.mainBody.position).normalized;
                        Vector3d north = Vector3d.Exclude(up, (module.vessel.mainBody.position + module.vessel.mainBody.transform.up * (float)module.vessel.mainBody.Radius) - module.vessel.findWorldCenterOfMass()).normalized;


                        Quaternion rotationtarget = Quaternion.LookRotation(north, up) * attitudetarget * Quaternion.Euler(90, 0, 0);

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

                public void ActiveController0(AscentProAPGCSModule module, FlightCtrlState s)
                {
                        
                        Quaternion attitudetarget = 
                                Quaternion.AngleAxis(heading, Vector3.up) * 
                                Quaternion.AngleAxis(-pitch, Vector3.right) * 
                                Quaternion.AngleAxis(-roll, Vector3.forward);

                        Vector3d up = (module.vessel.findWorldCenterOfMass() - module.vessel.mainBody.position).normalized;
                        Vector3d north = Vector3d.Exclude(up, (module.vessel.mainBody.position + module.vessel.mainBody.transform.up * (float)module.vessel.mainBody.Radius) - module.vessel.findWorldCenterOfMass()).normalized;
                        

                        Quaternion rotationtarget = Quaternion.LookRotation(north, up) * attitudetarget * Quaternion.Euler(90, 0, 0);

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
