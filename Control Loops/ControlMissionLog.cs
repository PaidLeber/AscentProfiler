using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class ControlMissionLog : ControlModule
        {
                internal bool isMissionLogEnabled = true;
                internal List<string> missionLog = new List<string>();
                internal int lastMissionLogTransmitCount = 0;


                internal void AddLog(string log)
                {
                        if (!isMissionLogEnabled)
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

                internal override void Process()
                {

                }

        }
}
