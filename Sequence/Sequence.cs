using System;
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

                internal Dictionary<string, List<Command>> sequenceBlock = new Dictionary<string, List<Command>>();
                internal Dictionary<ControlType, ControlModule> ControllerModules = new Dictionary<ControlType, ControlModule>();

                internal Dictionary<string, List<Trigger>> triggerBlockBuffer     = new Dictionary<string,List<Trigger>>();
                internal Dictionary<string, List<Action>>  actionBlockBuffer      = new Dictionary<string,List<Action>>();


                internal Sequence()
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

                internal void Process(AscentProAPGCSModule module)
                {
                        if (!Enabled)
                                return;

                        
                        foreach (Trigger trigger in triggerBlockBuffer[ActiveSequence].Where(trigger => trigger.activated == false && trigger.linkedIndex == 0))
                        {

                                if (trigger.Evaluate(module))                                                                   // Check & Execute Trigger(s)
                                {
                                        foreach (Trigger linkedtrigger in triggerBlockBuffer[ActiveSequence].Where(linkedtrigger => linkedtrigger.activated == false && linkedtrigger.linkedIndex > 0))
                                        {
                                                if(linkedtrigger.linkedIndex == trigger.index)
                                                {
                                                        linkedtrigger.linkedIndex = 0;
                                                }

                                        }
                                        Debug.Log(trigger.type);

                                        
                                        foreach (var action in actionBlockBuffer[ActiveSequence].Where(action => action.activated == false && action.linkedIndex == trigger.index))           
                                        {
                                                if (action.Evaluate(module))                                                     // Check & Execute Action(s)
                                                {
                                                        Debug.Log("Flight Sequence ExecuteActions " + trigger.index);


                                                }

                                        }


                                }

                        }
                        

                }





        }




}

