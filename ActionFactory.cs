﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AscentProfiler
{

        class ActionFactory
        {
                List<Action> actionList = new List<Action>();



                public void CreateAction(ActionType action, string commandLine, int lineNumber)
                {
                        /*
                        actionlist.Clear();



                        actionlist.Add(new ActionGroup (1, 1));
                        actionlist.Add(new Throttle(1, 100));
                        actionlist.Add(new ActionGroup(2, 2));
                        actionlist.Add(new ActionGroup(2, 3));
                        actionlist.Add(new ActionGroup(3, 4));
                        actionlist.Add(new ActionGroup(3, 5));
                        actionlist.Add(new ActionGroup(4, 6));
                        actionlist.Add(new ActionGroup(5, 7));
                        actionlist.Add(new StageNext(10));
                        */

                }





                public void Clear()
                {
                        //actionlist.Clear();
                }


        }
}
