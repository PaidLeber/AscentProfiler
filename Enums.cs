using System;
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

        enum LoadoutType
        { 
                Sensor,
                Control,
                Sequence
        }

        enum AttitudeType
        { 
                OFF,
                ON,
                HPR,
                HEADING,
                PITCH,
                ROLL,
                CONTROL
        }

        enum ControlType
        { 
                SENSOR,
                TELEMETRY,
                MISSIONLOG,
                ATTITUDE

        }

        enum AttitudeControlType
        { 
                SAS,
                MECHJEB
        }

        enum SensorType
        {
                TIME,
                ALTITUDE,
                GFORCE,
                MAXQ,
                AOA
        }
        enum FARSensorType
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
