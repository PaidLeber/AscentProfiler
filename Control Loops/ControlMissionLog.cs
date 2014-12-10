using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class ControlMissionLog : ControlModule
        {

                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;


                internal override T GetLoadedTypes<T>()
                {
                        throw new NotImplementedException();
                }

                internal override bool AddType<T>(T enumvalue)
                {
                        throw new NotImplementedException();
                }

                internal override bool RemoveType<T>(T enumvalue)
                {
                        throw new NotImplementedException();
                }

                internal override void Process(AscentProAPGCSModule module)
                {
                        throw new NotImplementedException();
                }
                internal override void ClearTypes()
                {
                        throw new NotImplementedException();
                }

                internal void AddLog(string log)
                {
                        if (!Enabled)
                        { return; }

                        var transferlog = timeStamp(module.vessel.missionTime) + " - " + log;
                        missionLog.Add(transferlog);
                        Log.Level(LogType.Verbose, transferlog);

                }

                string timeStamp(double secs)
                {
                        TimeSpan t = TimeSpan.FromSeconds(secs);

                        return string.Format("T+{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                }



        }
}
