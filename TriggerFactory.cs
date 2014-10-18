using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;

namespace AscentProfiler
{

        class TriggerFactory
        {
                Dictionary<TriggerType, Func<Trigger>> triggerProduct = new Dictionary<TriggerType, Func<Trigger>>();
                Dictionary<TriggerType, string> triggerRegex = new Dictionary<TriggerType, String>();

                // Use a LIFO stack to convert, track and chain tabs (\t) to trigger indexes.
                Stack<int> tabCountStack = new Stack<int>();

                //variables used for trigger contructors
                int TRIGGERINDEX;
                TriggerType TRIGGERTYPE;
                string DESCRIPTION;
                double TRIGGERVALUE;
                bool ASCENDING = true;
                bool FROMMAXVAL;

                int currentIndex = 0;

                // Regex patterns
                private string oneParamFromMaxValRegex = @"^\t*\w+\s+(\d+)\s*(\w*)\s*$";
                private string oneWordRegex = @"^\w+\s*";


                public TriggerFactory()
                {
                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, ASCENDING, FROMMAXVAL, TRIGGERVALUE); });


                        triggerRegex.Add(TriggerType.ASCENT, oneWordRegex);
                        triggerRegex.Add(TriggerType.DESCENT, oneWordRegex);
                        triggerRegex.Add(TriggerType.ALTITUDE, oneParamFromMaxValRegex);
                        
                
                }

                void ClearTriggerValues()
                {
                        TRIGGERTYPE = TriggerType.None;
                        DESCRIPTION = null;
                        FROMMAXVAL = false;
                        TRIGGERVALUE = 0;
                
                }

                public int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {
                        ClearTriggerValues();
                        Debug.Log("Cleared trigger values");
                        // Check for trigger mode switches
                        if (trigger == TriggerType.ASCENT)
                        {
                                ASCENDING = true;
                                return -1;
                        }
                        else if (trigger == TriggerType.DESCENT)
                        {
                                ASCENDING = false;
                                return -1;
                        }
                        Debug.Log("pre parse");
                        //Check command line for valid syntax, if true then parse it
                        Match triggerParse = Regex.Match(commandLine, triggerRegex[trigger]);
                        Debug.Log("post parse");
                        if (triggerParse.Success)
                        {
                                Debug.Log("trigger parse successful");


                                if (!String.IsNullOrEmpty(triggerParse.Groups[2].Value))
                                {
                                        // Check command line for optional trigger switches if so enable
                                        switch ((TriggerSwitch)Enum.Parse(typeof(TriggerSwitch), triggerParse.Groups[2].Value))
                                        {
                                                case TriggerSwitch.FROMMAXVAL:
                                                        FROMMAXVAL = true;
                                                        break;


                                        }                          
                                }

                        }
                        else
                        {
                                Debug.Log("Profile Loader: Invalid Syntax!: Line #" + lineNumber + ": " + commandLine);
                                //Create error log with linenumber, and commandline to flightlog window
                                return -1;
                        
                        }


                        currentIndex++;

                        Debug.Log("IsValidSyntax: " + trigger.ToString());

                        int tabcount = GetTabCount(commandLine);

                        Debug.Log("tabcount: " + tabcount);


                        // If tabcount == 0 then it is a first level trigger
                        // tabCountStack.Count - 1 = Previous trigger tab
                        // If tabcount == tabCountStack.Count - 1 then pop off trigger on stack, peep previous trigger and put it in new trigger's index, then replace with popped trigger
                        // If tabcount > tabCountStack.Count then Peek index value put it in the new trigger's index and push new trigger on stack
                        // If tabcount < tabCountStack.Count - 1 then pop triggers off stack until current trigger is pushed on top of it's corresponding chained trigger

                        if (tabcount == 0)
                        {
                                TRIGGERINDEX = -1; //This is an unchained trigger.
                                tabCountStack.Clear();
                                tabCountStack.Push(currentIndex);
                                
                        }
                        else if (tabcount == tabCountStack.Count - 1)
                        {
                                Debug.Log("elseif 2: ");
                                Debug.Log("tabcountstack: " + Convert.ToString(tabCountStack.Count));
                                Debug.Log("tabcountstack -1: "+Convert.ToString(tabCountStack.Count - 1));

                                tabCountStack.Pop();
                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else if (tabcount > tabCountStack.Count - 1)
                        {
                                Debug.Log("elseif 3: ");
                                Debug.Log("tabcountstack: " + Convert.ToString(tabCountStack.Count));
                                Debug.Log("tabcountstack -1: " + Convert.ToString(tabCountStack.Count - 1));

                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else if (tabcount < tabCountStack.Count - 1)
                        {
                                Debug.Log("tabcountstack prior: " + Convert.ToString(tabCountStack.Count));
                                for (int i = tabCountStack.Count; i > tabcount; i--)
                                {
                                        Debug.Log("elseif 4 POP: " + i);
                                        tabCountStack.Pop();
                                        Debug.Log("tabcountstack POP during: " + Convert.ToString(tabCountStack.Count));
                                }
                                Debug.Log("elseif 4: ");
                                Debug.Log("tabcountstack: " + Convert.ToString(tabCountStack.Count));
                                Debug.Log("tabcountstack -1: " + Convert.ToString(tabCountStack.Count - 1));
                                Debug.Log("tabcountstack Peek: " + Convert.ToString(tabCountStack.Peek()));

                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else
                        {
                                //Create loading error in flightlog window 
                                Debug.Log("UNCHAINED TRIGGER ERROR: CHECK TAB STRUCTURE");
                        }





                        //triggerIndex = currentIndex;
                        TRIGGERTYPE = trigger;
                        TRIGGERVALUE = Convert.ToDouble(triggerParse.Groups[1].Value);
                        DESCRIPTION = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trigger.ToString());

                        Debug.Log("NEW ALTITUDE VARIABLES: " + " index: " + TRIGGERINDEX + " type: " + TRIGGERTYPE + " desc: " + DESCRIPTION + " ascentmode: " + ASCENDING + " value: " + TRIGGERVALUE + " frommaxval: " + FROMMAXVAL);
                        Debug.Log("TRIGGER INDEX: " + currentIndex);
                        Debug.Log("TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, triggerProduct[trigger]());
                        Debug.Log("TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        


                        //just use string.contains()!!!
                        //keep track of # of trigger \t tabs
                        //keep List<int> of tabbed triggers
                        //Place triggers in indexed list by # of tabs eg. no tabs List index 0, \t List index 1, \t\t list index 2
                        // when new line read tab # goes down an index, add new tab entry and clear all indexed list entries above it
                        //use dictionary for triggerproducts



                        return currentIndex;
                }




                public int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, @"^(\t)+\w+").Groups[1].Captures.Count;
                        
                }


                public void Clear()
                {



                }

                

        }
}
