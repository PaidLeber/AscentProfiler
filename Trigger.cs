using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace AscentProfiler
{
        internal enum TriggerType
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
        internal enum TriggerModifier
        { 
                None,
                FROMMAXVAL
        
        }

        internal abstract class Trigger
        {

                internal bool state = false;
                internal int index;

                protected TriggerType type;
                protected bool ascentMode;
                protected bool fromaxval;

                protected double value;
                protected double maxval;

                internal string displayvalue;
                internal string description;

                internal abstract bool Evaluate(bool isascending);

                protected bool isIncreasing(bool isascending, double vVariable, double vStatic)
                {
                        return (vVariable > vStatic && isascending) ? true : false;
                }

                protected bool isDecreasing(bool isascending, double vVariable, double vStatic)
                {
                        return (vVariable < vStatic && !isascending) ? true : false;
                }

                protected double calcMaxVal(bool ascentmode, double currentValue, double maxValue)
                {
                        return ascentmode ? Math.Max(currentValue, maxValue) : Math.Min(currentValue, maxValue);
                }
        }


        internal class Altitude : Trigger
        {
                
                
                internal Altitude(int index, TriggerType type, string description, bool ascentMode, bool fromaxval, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.description = description;
                        this.ascentMode = ascentMode;
                        this.fromaxval = fromaxval;
                        this.value = value;
                        this.displayvalue = value.ToString();

                        Log.Level(LogType.Verbose, "constructor new trigger: index: "+ index +" trigger: "+ type +" description: "+ description +" ascentmode: "+ ascentMode+" value: "+ value+" maxval: "+ maxval +" fromaxval: "+ fromaxval);
                }

                internal override bool Evaluate(bool isascending)
                {

                        double currentAltitude = ascentMode ? FlightGlobals.ship_altitude : (FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude);

                        if (!fromaxval)
                        {
                                return state = ascentMode ?
                                        isIncreasing(isascending, currentAltitude, value) :
                                        isDecreasing(isascending, currentAltitude, value);
                        }
                        else
                        {
                                double delta;

                                maxval = calcMaxVal(ascentMode, currentAltitude, maxval);
                                delta  = ascentMode ? maxval - currentAltitude : maxval + currentAltitude;

                                return state = isIncreasing(isascending, delta, value);
                        }

                }

        }

        
        class Countdown : Trigger
        {

                public Countdown(int index, TriggerType type, string description, double value) // fix constructor
                {
                        this.index = index;
                        this.type = type;
                        this.description = description;
                        this.value = value;
                }

                internal override bool Evaluate(bool isascending)
                {
                        double UT = Planetarium.GetUniversalTime();

                        if (maxval == 0)
                        {
                                maxval = UT + value;
                        }

                        value = maxval - UT;

                        if (UT >= maxval && state == false)
                        {
                                value = 0;
                                //FlightLog.Log("COUNTDOWN TRIGGER EXECUTED");
                                return state = true;
                        }
                        
                       return false;
                       
                }
                


        }

        /*

        class Liftoff : Trigger
        {

                public Liftoff(int index, TriggerType type, string desc, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.fromaxval = fromaxval;
                }

                internal override bool Evaluate(bool isascending)
                {
                        double UT = Planetarium.GetUniversalTime();
                         //public static bool LiftedOff(this Vessel vessel)
                        //{
                        //return vessel.situation != Vessel.Situations.PRELAUNCH;
                        //}
                        if (countdown == 0)
                        {
                                countdown = UT + value;
                        }

                        value = countdown - UT;

                        if (UT >= countdown)
                        {
                                
                                //Value = 0;
                                Staging.ActivateNextStage(); //use triggerfactory to auto add action activatenextstage() when liftoff is created
                                //FlightLog.Log("# # # LIFTOFF # # #");
                                return state = true;
                        }

                        return false;
                }
        }


        class Gforce : Trigger
        {
                public double gmax;
                public double gforce;

                public Gforce(int index, TriggerType type, string desc, bool ascending, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.value = value;
                        this.ascentMode = ascending;
                }

                public Gforce(int index, TriggerType type, string desc, bool ascending, double value, bool fromaxval)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.ascentMode = ascending;
                        this.value = value;
                        this.fromaxval = fromaxval;
                }

                internal override bool Evaluate(bool isascending)
                {
                        gforce = FlightGlobals.ship_geeForce;

                        if (gforce > gmax)
                        {
                                gmax = gforce;
                        }
                        // FIX!!!
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



        
        class Burnout : Trigger
        {

                public Burnout(int index, TriggerType type, string desc)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        //Vessel v = FlightGlobals.ActiveVessel;
                }

                internal override bool Evaluate(bool isascending)
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
                static bool HasActiveOrIdleEngineOrTankDescendant(Part p, List<int> tankResources)
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


                List<int> FindBurnedResources(Vessel v)
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
                static bool InverseStageDecouplesActiveOrIdleEngineOrTank(int inverseStage, Vessel v, List<int> tankResources)
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
                static bool InverseStageDecouplesDeactivatedEngineOrTank(int inverseStage, Vessel v)
                {
                        foreach (Part p in v.parts)
                        {
                                if (p.inverseStage == inverseStage && p.IsUnfiredDecoupler() && HasDeactivatedEngineOrTankDescendant(p)) return true;
                        }
                        return false;
                }


                //detect if a part is above a deactivated engine or fuel tank
                static bool HasDeactivatedEngineOrTankDescendant(Part p)
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
                static bool InverseStageFiresDecoupler(int inverseStage, Vessel v)
                {
                        foreach (Part p in v.parts)
                        {
                                if (p.inverseStage == inverseStage && p.IsUnfiredDecoupler()) return true;
                        }
                        return false;
                }

                //determine if there are chutes being fired that wouldn't also get decoupled
                static bool HasStayingChutes(int inverseStage, Vessel v)
                {
                        var chutes = v.parts.FindAll(p => p.inverseStage == inverseStage && p.IsParachute());
                        foreach (Part p in chutes)
                        {
                                if (!p.IsDecoupledInStage(inverseStage)) { return true; }
                        }
                        return false;
                }

        
        }



        class Attitude : Trigger
        {
                Vessel v;
                public double yaw = 0;
                public double pitch = 65;
                public double roll = 90;
                Vector3 up;
                Vector3 forward;

                Attitude(int index, TriggerType type, string desc, double value)
                {
                        this.index = index;
                        this.type = type;
                        this.desc = desc;
                        this.value = value;
                        this.pitch = value;
                        
                }

                internal override bool Evaluate(bool isascending)
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


        */
}
