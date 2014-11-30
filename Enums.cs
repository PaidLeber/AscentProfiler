using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        enum SensorType
        {
                TIME,
                ALTITUDE,
                GFORCE
        }

        enum TelemetryType
        {
                TRANSMIT,
                TX,
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

        enum TriggerModifier
        {
                None,
                FROMMAXVAL

        }
        enum ActionType
        {
                ACTIONGROUP,
                STAGE,
                THROTTLE,
                TELEMETRY,
                SENSORS
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
