using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        public class ActionExecutor
        {
                public List<Action> actionlist = new List<Action>();

                public ActionExecutor()
                {
                }

                public void ExecuteActions(int index)
                {

                        foreach (var action in actionlist.Where(action => action.state == false && action.index == index))
                        {
                                if ( action.Execute() )
                                {
                                        Debug.Log("ActionExecutor.ExecuteActions " + index);
                                      

                                }

                        }


                }



        }
}
