using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        class ActionExecutor
        {
                internal List<Action> actionlist;

                internal ActionExecutor(List<Action> newactionlist)
                {
                        Log.Level(LogType.Verbose, "Action Executor constructor!");
                        this.actionlist = newactionlist;
                }

                internal void ExecuteActions(int index)
                {

                        foreach (var action in actionlist.Where(action => action.activated == false && action.index == index))
                        {
                                if ( action.Execute() )
                                {
                                        Debug.Log("ActionExecutor.ExecuteActions " + index);
                                      

                                }

                        }


                }



        }
}
