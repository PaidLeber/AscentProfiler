using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        class FlightProfile
        {
                internal bool isEnabled = false;

                List<Trigger> listTrigger;
                List<Action> listAction;

                AscentProAPGCSModule module;

                internal FlightProfile(List<Trigger> triggerlist, List<Action> actionlist)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        listAction = actionlist;
                        listTrigger = triggerlist;

                        Debug.Log("FLIGHT PROFILE CONTRUCTOR!");
                        foreach(Trigger item in listTrigger )
                        {
                                Debug.Log(item.activated + " index: " + item.index + " linked index: " + item.linkedIndex + " type: " + item.type + " desc: " + item.description + " dispvalue: " + item.displayvalue);
                        }

                        foreach (Action item in listAction)
                        {
                                Debug.Log(item.activated + " linked index: " + item.index + " type: " + item.type + " desc: " + item.description + " dispvalue: " + item.displayvalue +" modifier: "+ item.modifier);
                        }
                }

                public void TriggerLoop()
                {
                        if (!isEnabled)
                                { return; }

                        //Debug.Log("Trigger Loop test");
                        foreach (Trigger trigger in listTrigger.Where(trigger => trigger.activated == false && trigger.linkedIndex == 0))
                        {

                                if (trigger.Evaluate(module))
                                {
                                        Debug.Log(trigger.type);
                                        ExecuteActions(trigger.index);
                                }

                        }
                        

                }

                internal void AssignToModule(AscentProAPGCSModule module)
                {
                        this.module = module;
                }

                internal void ExecuteActions(int index)
                {

                        foreach (var action in listAction.Where(action => action.activated == false && action.index == index))
                        {
                                if (action.Execute())
                                {
                                        Debug.Log("Flight Profile ExecuteActions " + index);


                                }

                        }


                }

        }




}

