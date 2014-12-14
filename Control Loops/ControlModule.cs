using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        [Serializable]
        abstract class ControlModule
        {
                //protected AscentProAPGCSModule module;

                internal bool Enabled;

                internal abstract T GetLoadedTypes<T>();
                internal abstract bool AddType<T>(T enumvalue);
                internal abstract bool RemoveType<T>(T enumvalue);
                internal abstract void ClearTypes();
                internal abstract void Process(AscentProAPGCSModule module);

        }
}
