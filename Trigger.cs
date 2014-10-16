using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace AscentProfiler
{
        enum TriggerType
        {
                None,
                ASCENT,
                DESCENT,
                ALTITUDE,
                COUNTDOWN,
                LIFTOFF,
                GFORCE,
                BURNOUT,
                ATTITUDE

        }

        public abstract class Trigger
        {
                public string type;
                public string desc;
                public int index = -1;
                public double value;
                public double fromaxval;
                public bool ascending;
                public bool state = false;
                public double countdown;
                public abstract bool Evaluate(bool isascending);

        }


        public class Altitude : Trigger
        {

                public Altitude(int index, string type, string desc, bool ascending, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.ascending = ascending;
                        this.value = value;
                }

                public override bool Evaluate(bool isascending) //do overide evaluate, change state then return bool
                {
                        if (FlightGlobals.ship_altitude > value && isascending && ascending)
                        {
                                //FlightLog.Log("ALTITUDE "+ value +"M ASCENDING");
                                return state = true;

                        }
                        else if ((FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude) < value && !isascending && !ascending)
                        {
                                //FlightLog.Log("ALTITUDE " + value + "M DESCENDING");
                                return state = true;
                        }

                       return false;    
                }       
        
        
        
        }


        public class Countdown : Trigger
        {

                public Countdown(int index, string type, string desc, double fromaxval)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.fromaxval = fromaxval;
                }

                public override bool Evaluate(bool isascending)
                {
                        double UT = Planetarium.GetUniversalTime();

                        if (countdown == 0)
                        { 
                                countdown = UT + fromaxval;
                        }

                        value = countdown - UT;

                        if (UT >= countdown && state == false)
                        {
                                value = 0;
                                //FlightLog.Log("COUNTDOWN TRIGGER EXECUTED");
                                return state = true;
                        }

                       return false;
                       
                }



        }



        public class Liftoff : Trigger
        {

                public Liftoff(int index, string type, string desc, double fromaxval)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.fromaxval = fromaxval;
                }

                public override bool Evaluate(bool isascending)
                {
                        double UT = Planetarium.GetUniversalTime();
                         //public static bool LiftedOff(this Vessel vessel)
                        //{
                        //return vessel.situation != Vessel.Situations.PRELAUNCH;
                        //}
                        if (countdown == 0)
                        {
                                countdown = UT + fromaxval;
                        }

                        value = countdown - UT;

                        if (UT >= countdown)
                        {
                                
                                value = 0;
                                Staging.ActivateNextStage(); //use triggerfactory to auto add action activatenextstage() when liftoff is created
                                //FlightLog.Log("# # # LIFTOFF # # #");
                                return state = true;
                        }

                        return false;
                }
        }


        public class Gforce : Trigger
        {
                public double gmax;
                public double gforce;

                public Gforce(int index, string type, string desc, bool ascending, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.value = value;
                        this.ascending = ascending;
                }

                public Gforce(int index, string type, string desc, bool ascending, double value, double fromaxval)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.ascending = ascending;
                        this.value = value;
                        this.fromaxval = fromaxval;
                }

                public override bool Evaluate(bool isascending)
                {
                        gforce = FlightGlobals.ship_geeForce;

                        if (gforce > gmax)
                        {
                                gmax = gforce;
                        }

                        if(isascending == ascending)
                        {
                                if (fromaxval == 0 && gforce > value)
                                {
                                        //FlightLog.Log(value + "G EXCEEDED DURING ASCENT");
                                        return state = true;
                                }
                        }
                        else if(isascending != ascending)
                        {
                                if(fromaxval > 0 && fromaxval < gmax)
                                {
                                        //FlightLog.Log(value + "G EXCEEDED DURING DESCENT");
                                        return state = true;
                                }
                        
                        }
                        
                        return false;
                }
        }



        
        public class Burnout : Trigger
        {

                public Burnout(int index, string type, string desc)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        //Vessel v = FlightGlobals.ActiveVessel;
                }

                public override bool Evaluate(bool isascending)
                {
                        //remove this if statement later
                        if (FlightGlobals.ActiveVessel.situation != Vessel.Situations.PRELAUNCH)
                        {
                                //don't decouple active or idle engines or tanks
                                List<int> burnedResources = FindBurnedResources(FlightGlobals.ActiveVessel);
                                if (!InverseStageDecouplesActiveOrIdleEngineOrTank(Staging.CurrentStage - 1, FlightGlobals.ActiveVessel, burnedResources))
                                { 
                                        //Don't fire a stage that will activate a parachute, unless that parachute gets decoupled:
                                        if (!HasStayingChutes(Staging.CurrentStage - 1, FlightGlobals.ActiveVessel))
                                        {
                                                //only fire decouplers to drop deactivated engines or tanks
                                                bool firesDecoupler = InverseStageFiresDecoupler(Staging.CurrentStage - 1, FlightGlobals.ActiveVessel);
                                                if (!firesDecoupler || InverseStageDecouplesDeactivatedEngineOrTank(Staging.CurrentStage - 1, FlightGlobals.ActiveVessel))
                                                {
                                                        //FlightLog.Log("STAGE BURNOUT");
                                                        return state = true;

                                                }

                                                
                                        }
                        
                        
                                }
                        }

                        return false;
                }



                //Mechjeb methods gpl3
                public static bool HasActiveOrIdleEngineOrTankDescendant(Part p, List<int> tankResources)
                {
                        if ((p.State == PartStates.ACTIVE || p.State == PartStates.IDLE)
                        && p.IsEngine() && !p.IsSepratron() && p.EngineHasFuel())
                        {
                                return true; // TODO: properly check if ModuleEngines is active
                        }
                        if ((p is FuelTank) && (((FuelTank)p).fuel > 0)) return true;
                        if (!p.IsSepratron())
                        {
                                foreach (PartResource r in p.Resources)
                                {
                                        if (r.amount > 0 && r.info.name != "ElectricCharge" && tankResources.Contains(r.info.id))
                                        {
                                                return true;
                                        }
                                }
                        }
                        foreach (Part child in p.children)
                        {
                                if (HasActiveOrIdleEngineOrTankDescendant(child, tankResources)) return true;
                        }
                        return false;
                }


                public List<int> FindBurnedResources(Vessel v)
                {
                        var activeEngines = v.parts.Where(p => p.inverseStage >= Staging.CurrentStage && p.IsEngine() && !p.IsSepratron());
                        HashSet<Propellant> burnedPropellants = new HashSet<Propellant>();
                        foreach (Part p in activeEngines)
                        {
                                foreach (ModuleEngines m in p.Modules.OfType<ModuleEngines>())
                                        if (!m.getFlameoutState)
                                                burnedPropellants.UnionWith(m.propellants);
                                foreach (ModuleEnginesFX m in p.Modules.OfType<ModuleEnginesFX>())
                                        if (m.isEnabled && !m.getFlameoutState)
                                                burnedPropellants.UnionWith(m.propellants);
                        }
                        List<int> propellantIDs = burnedPropellants.Select(prop => prop.id).ToList();
                        return propellantIDs;
                }


                //determine whether it's safe to activate inverseStage
                public static bool InverseStageDecouplesActiveOrIdleEngineOrTank(int inverseStage, Vessel v, List<int> tankResources)
                {
                        foreach (Part p in v.parts)
                        {
                                if (p.inverseStage == inverseStage && p.IsUnfiredDecoupler() && HasActiveOrIdleEngineOrTankDescendant(p, tankResources))
                                {
                                        return true;
                                }
                        }
                        return false;
                }



                //determine whether inverseStage sheds a dead engine
                public static bool InverseStageDecouplesDeactivatedEngineOrTank(int inverseStage, Vessel v)
                {
                        foreach (Part p in v.parts)
                        {
                                if (p.inverseStage == inverseStage && p.IsUnfiredDecoupler() && HasDeactivatedEngineOrTankDescendant(p)) return true;
                        }
                        return false;
                }


                //detect if a part is above a deactivated engine or fuel tank
                public static bool HasDeactivatedEngineOrTankDescendant(Part p)
                {
                        if ((p.State == PartStates.DEACTIVATED) && (p is FuelTank || p.IsEngine()) && !p.IsSepratron())
                        {
                                return true; // TODO: yet more ModuleEngine lazy checks
                        }
                        //check if this is a new-style fuel tank that's run out of resources:
                        bool hadResources = false;
                        bool hasResources = false;
                        foreach (PartResource r in p.Resources)
                        {
                                if (r.name == "ElectricCharge") continue;
                                if (r.maxAmount > 0) hadResources = true;
                                if (r.amount > 0) hasResources = true;
                        }
                        if (hadResources && !hasResources) return true;
                        if (p.IsEngine() && !p.EngineHasFuel()) return true;
                        foreach (Part child in p.children)
                        {
                                if (HasDeactivatedEngineOrTankDescendant(child)) return true;
                        }
                        return false;
                }

                //determine whether activating inverseStage will fire any sort of decoupler. This
                //is used to tell whether we should delay activating the next stage after activating inverseStage
                public static bool InverseStageFiresDecoupler(int inverseStage, Vessel v)
                {
                        foreach (Part p in v.parts)
                        {
                                if (p.inverseStage == inverseStage && p.IsUnfiredDecoupler()) return true;
                        }
                        return false;
                }

                //determine if there are chutes being fired that wouldn't also get decoupled
                public static bool HasStayingChutes(int inverseStage, Vessel v)
                {
                        var chutes = v.parts.FindAll(p => p.inverseStage == inverseStage && p.IsParachute());
                        foreach (Part p in chutes)
                        {
                                if (!p.IsDecoupledInStage(inverseStage)) { return true; }
                        }
                        return false;
                }

        
        }



        public class Attitude : Trigger
        {
                Vessel v;
                public double yaw = 0;
                public double pitch = 65;
                public double roll = 90;
                Vector3 up;
                Vector3 forward;

                public Attitude(int index, string type, string desc, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.value = value;
                        this.pitch = value;
                        
                }

                public override bool Evaluate(bool isascending)
                {


                        if (FlightGlobals.ActiveVessel != null)
                        {

                                
                                FlightCtrlState s = FlightInputHandler.state;

                                v = FlightGlobals.ActiveVessel;

                                Quaternion rotation = Quaternion.Euler(new Vector3d(Double.IsNaN(pitch) ? 0 : pitch, Double.IsNaN(yaw) ? 0 : -yaw, Double.IsNaN(roll) ? 0 : 180 - roll));

                                up = (v.mainBody.position - v.CoM);

                                forward = Vector3.Exclude(up, v.mainBody.position + v.mainBody.transform.up * (float)v.mainBody.Radius - v.CoM);

                                Vector3.OrthoNormalize(ref forward, ref up);

                                Quaternion rotationReference = Quaternion.LookRotation(forward, up);

                                rotationReference = rotationReference * rotation;

                                //AttitudeController.setAttitude(heading, pitch, roll);

                                AttitudeController.HoldAttitude(rotationReference, s, v);





                        }





                        return false;
                }

        }
}
