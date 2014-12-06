﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{

        enum RegisteredAddons
        {
                FerramAerospaceResearch,
                RemoteTech
        }

        enum AttitudeType
        { 
                HPR,
                HEADING,
                PITCH,
                ROLL,
                CONTROL
        }

        enum ControlType
        { 
                ATTITUDE
        }

        enum ControllerType
        { 
                SAS
        }

        enum SensorType
        {
                TIME,
                ALTITUDE,
                GFORCE,
                MAXQ,
                AOA
        }
        enum FARAPIType
        { 
                MAXQ,
                AOA
        }

        enum TelemetryType
        {
                TRANSMIT,
                MISSIONLOG,
                SENSORS
        }

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

        enum ActionType
        {
                ACTIONGROUP,
                STAGE,
                THROTTLE,
                TELEMETRY,
                SENSORS,
                CONTROL
        }

        enum TriggerModifier
        {
                FROMMAXVAL,
                HOLD

        }

        enum ActionModifier
        {
                ACTIVATE,
                DEACTIVATE,
                ON,
                OFF,
                TOGGLE,
                NEXT
        }




}
