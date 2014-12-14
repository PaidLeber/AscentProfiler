using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        [Serializable]
        abstract class Command
        {
                internal int index = 0;
                internal int linkedIndex;
                internal bool activated;

                protected bool state;

                internal string displayvalue;
                internal string description;

                internal abstract bool Evaluate(AscentProAPGCSModule module);

        }
}
