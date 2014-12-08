using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        abstract class ControlModule
        {
                protected AscentProAPGCSModule module;

                internal bool Enabled;

                virtual void Process();

        }
}
