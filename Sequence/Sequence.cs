﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{
        [Serializable]
        class Sequence
        {
                internal bool Enabled;

                internal string ActiveSequence;

                internal Dictionary<string, List<Trigger>> triggerBlockBuffer     = new Dictionary<string,List<Trigger>>();
                internal Dictionary<string, List<Action>>  actionBlockBuffer      = new Dictionary<string,List<Action>>();

                internal Dictionary<ControlType, ControlModule> ControllerModules = new Dictionary<ControlType, ControlModule>();


                AscentProAPGCSModule module;

                internal Sequence(List<Trigger> triggerlist, List<Action> actionlist)
                {
                        Log.Level(LogType.Verbose, "Trigger Guardian contructor!");
                        //listAction = actionlist;
                        //listTrigger = triggerlist;
                        /*
                        Debug.Log("FLIGHT SEQUENCE TRIGGER CONTRUCTOR!");
                        foreach(Trigger item in listTrigger)
                        {
                                Debug.Log("TRIGGER: activated: " + item.activated + " index: " + item.index + " linked index: " + item.linkedIndex + " type: " + item.type + " desc: " + item.description + " dispvalue: " + item.displayvalue);
                        }
                        Debug.Log("FLIGHT SEQUENCE ACTION CONTRUCTOR!");
                        foreach (Action item in listAction)
                        {
                                Debug.Log("ACTION: activated: " + item.activated + " linked index: " + item.index + " type: " + item.type + " desc: " + item.description + " dispvalue: " + item.displayvalue + " modifier: " + item.modifier);
                        }
                         */ 
                }

                internal void Process()
                {
                        if (!Enabled)
                                return;

                        
                        foreach (Trigger trigger in triggerBlockBuffer[ActiveSequence].Where(trigger => trigger.activated == false && trigger.linkedIndex == 0))
                        {

                                if (trigger.Evaluate(module))
                                {
                                        foreach (Trigger linkedtrigger in triggerBlockBuffer[ActiveSequence].Where(linkedtrigger => linkedtrigger.activated == false && linkedtrigger.linkedIndex > 0))
                                        {
                                                if(linkedtrigger.linkedIndex == trigger.index)
                                                {
                                                        linkedtrigger.linkedIndex = 0;
                                                }

                                        }
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

                        foreach (var action in actionBlockBuffer[ActiveSequence].Where(action => action.activated == false && action.index == index))
                        {
                                if (action.Execute(module))
                                {
                                        Debug.Log("Flight Sequence ExecuteActions " + index);


                                }

                        }


                }

        }




}
