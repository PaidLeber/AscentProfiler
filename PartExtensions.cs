﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        public static class PartExtensions
        {
                //From Mechjeb source GPL3
                public static bool HasModule<T>(this Part p) where T : PartModule
                {
                        return p.Modules.OfType<T>().Any();
                }

                public static float TotalMass(this Part p)
                {
                        return p.mass + p.GetResourceMass();
                }

                public static bool EngineHasFuel(this Part p)
                {
                        if (p is LiquidEngine || p is LiquidFuelEngine || p is AtmosphericEngine)
                        {
                                //I don't really know the details of how you're supposed to use RequestFuel, but this seems to work to
                                //test whether something can get fuel.
                                return p.RequestFuel(p, 0, Part.getFuelReqId());
                        }
                        else if (p.HasModule<ModuleEngines>())
                        {
                                return !p.Modules.OfType<ModuleEngines>().First().getFlameoutState;
                        }
                        else if (p.HasModule<ModuleEnginesFX>())
                        {
                                return !p.Modules.OfType<ModuleEnginesFX>().First(e => e.isEnabled).getFlameoutState;
                        }
                        else return false;
                }
                public static bool IsDecoupler(this Part p)
                {
                        return (p is Decoupler ||
                        p is DecouplerGUI ||
                        p is RadialDecoupler ||
                        p.HasModule<ModuleDecouple>() ||
                        p.HasModule<ModuleAnchoredDecoupler>());
                }
                public static bool IsUnfiredDecoupler(this Part p)
                {
                        return p.FindModulesImplementing<ModuleDecouple>().Any(m => !m.isDecoupled) ||
                        p.FindModulesImplementing<ModuleAnchoredDecoupler>().Any(m => !m.isDecoupled);
                }
                //Any engine that is decoupled in the same stage in
                //which it activates we call a sepratron.
                public static bool IsSepratron(this Part p)
                {
                        return p.ActivatesEvenIfDisconnected
                        && p.IsEngine()
                        && p.IsDecoupledInStage(p.inverseStage)
                        && !p.isControlSource;
                }
                public static bool IsSRB(this Part p)
                {
                        if (p is SolidRocket) return true;
                        //new-style SRBs:
                        if (p.HasModule<ModuleEngines>()) //sepratrons are motors
                                return p.Modules.OfType<ModuleEngines>().First().throttleLocked; //throttleLocked signifies an SRB
                        if (p.HasModule<ModuleEnginesFX>())
                                return p.Modules.OfType<ModuleEnginesFX>().First(e => e.isEnabled).throttleLocked; // Will fail if they are all !isEnabled. Can this happend ?
                        return false;
                }
                public static bool IsEngine(this Part p)
                {
                        return (p is SolidRocket ||
                        p is LiquidEngine ||
                        p is LiquidFuelEngine ||
                        p is AtmosphericEngine ||
                        p.HasModule<ModuleEngines>() ||
                        p.HasModule<ModuleEnginesFX>());
                }


                public static bool IsDecoupledInStage(this Part p, int stage)
                {
                        if ((p.IsUnfiredDecoupler() || p.IsLaunchClamp()) && p.inverseStage == stage) return true;
                        if (p.parent == null) return false;
                        return p.parent.IsDecoupledInStage(stage);
                }

                public static bool IsParachute(this Part p)
                {
                        return p is Parachutes ||
                        p is HParachutes ||
                        p.HasModule<ModuleParachute>();
                }


                public static bool IsLaunchClamp(this Part p)
                {
                        return p.HasModule<LaunchClamp>();
                }



                public static bool IsPhysicallySignificant(this Part p)
                {
                        bool physicallySignificant = (p.physicalSignificance != Part.PhysicalSignificance.NONE);
                        // part.PhysicsSignificance is not initialized in the Editor for all part. but physicallySignificant is useful there.
                        if (HighLogic.LoadedSceneIsEditor)
                        {
                                physicallySignificant = physicallySignificant && p.PhysicsSignificance != 1;
                                // Testing for launch clamp only in the Editor helps with the frame rate.
                                // TODO : cache which part are LaunchClamp ?
                                if (p.HasModule<ModuleLandingGear>() || p.HasModule<LaunchClamp>())
                                {
                                        //Landing gear set physicalSignificance = NONE when they enter the flight scene
                                        //Launch clamp mass should be ignored.
                                        physicallySignificant = false;
                                }
                        }
                        return physicallySignificant;
                }

        }



}
