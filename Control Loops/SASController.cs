using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class SASController : AttitudeController
        {
                internal SASController(AscentProAPGCSModule module)
                {
                        this.module = module;
                }


                public void Controller(FlightCtrlState s)
                {

                }

        }
}
