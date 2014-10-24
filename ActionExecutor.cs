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

                internal ActionExecutor()
                {
                }

                public void ExecuteActions(int index)
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
