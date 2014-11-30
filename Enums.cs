using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        enum SensorType
        {
                TRANSMIT,
                OFF,
                ON,
                TIME,
                ALTITUDE,
                GFORCE

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
                THROTTLE
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
