using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        interface IControl
        {
                protected AscentProAPGCSModule module;

                internal bool Enabled;


                void Process();

        }
}
