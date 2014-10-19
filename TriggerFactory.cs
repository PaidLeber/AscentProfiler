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
                internal Dictionary<string, string> regexDict = new Dictionary<string, string>();
                Dictionary<TriggerType, string> triggerRegex = new Dictionary<TriggerType, String>();
                Dictionary<TriggerType, Func<Trigger>> triggerProduct = new Dictionary<TriggerType, Func<Trigger>>();

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

                internal TriggerFactory()
                {
                        regexDict.Add("START", @"^START\s*$");
                        regexDict.Add("END", @"^END\s*$");
                        regexDict.Add("CMDBEGIN", @"^\t*");
                        regexDict.Add("CMDEND", @"\s*.*$");
                        regexDict.Add("oneParamFromMaxValRegex", @"^\t*\w+\s+(\d+)\s*(\w*)\s*$");
                        regexDict.Add("oneWordRegex", @"^\w+\s*");
                        regexDict.Add("tabcount", @"^(\t)+\w+");

                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(TRIGGERINDEX, TRIGGERTYPE, DESCRIPTION, ASCENDING, FROMMAXVAL, TRIGGERVALUE); });


                        triggerRegex.Add(TriggerType.ASCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.DESCENT, regexDict["oneWordRegex"]);
                        triggerRegex.Add(TriggerType.ALTITUDE, regexDict["oneParamFromMaxValRegex"]);
                        
                
                }

                void ClearTriggerValues()
                {
                        TRIGGERTYPE = TriggerType.None;
                        DESCRIPTION = null;
                        FROMMAXVAL = false;
                        TRIGGERVALUE = 0;
                
                }

                internal int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
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
                        Debug.Log("trigger is: "+trigger.ToString()+" commandline is: "+commandLine);
                        //Check command line for valid syntax, if true then parse it
                        Match triggerParse = Regex.Match(commandLine, triggerRegex[trigger]);

                        Debug.Log("post parse");
                        if (triggerParse.Success)
                        {
                                Debug.Log("trigger parse successful");

                                
                                if (!String.IsNullOrEmpty(triggerParse.Groups[2].Value))
                                {
                                        // Check command line for optional trigger switches if so enable
                                        switch ((TriggerModifier)Enum.Parse(typeof(TriggerModifier), triggerParse.Groups[2].Value))
                                        {
                                                case TriggerModifier.FROMMAXVAL:
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
                        // If (tabcount - tabCountStack.Count) == 0 then Peek index value put it in the new trigger's index and push new trigger on stack
                        // If tabcount < tabCountStack.Count then pop triggers off stack until current trigger is pushed on top of it's corresponding chained trigger
                        // If (tabcount - tabCountStack.Count) > 0 Catch unchained trigger and throw error



                        if (tabcount == 0)
                        {
                                TRIGGERINDEX = 0; // This is an unchained (root) trigger.
                                tabCountStack.Clear();
                                tabCountStack.Push(currentIndex);
                                
                        }
                        else if ((tabcount - tabCountStack.Count) == 0)
                        {
                                Debug.Log("elseif 3: ");
                                Debug.Log("tabcountstack: " + Convert.ToString(tabCountStack.Count));
                                Debug.Log("tabcountstack -1: " + Convert.ToString(tabCountStack.Count - 1));

                                TRIGGERINDEX = tabCountStack.Peek();
                                tabCountStack.Push(currentIndex);
                        }
                        else if (tabcount < tabCountStack.Count)
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
                        else if ((tabcount - tabCountStack.Count) > 0)
                        {
                                //Create loading error in flightlog window 
                                Debug.Log("UNCHAINED TRIGGER ERROR: CHECK TAB STRUCTURE");
                        }



                        
                        

                        //triggerIndex = currentIndex;
                        TRIGGERTYPE = trigger;
                        TRIGGERVALUE = Convert.ToDouble(triggerParse.Groups[1].Value);
                        DESCRIPTION = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trigger.ToString());

                        Debug.Log("NEW ALTITUDE VARIABLES: " + " index: " + TRIGGERINDEX + " type: " + TRIGGERTYPE + " desc: " + DESCRIPTION + " ascentmode: " + ASCENDING + " value: " + TRIGGERVALUE + " frommaxval: " + FROMMAXVAL);
                        Debug.Log("CURRENT INDEX: " + currentIndex);
                        Debug.Log("TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, triggerProduct[trigger]());
                        Debug.Log("TRIGGER DICTIONARY COUNT: " + AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Count);
                        



                        return currentIndex;
                }




                int GetTabCount(string commandLine)
                {
                        return Regex.Match(commandLine, regexDict["tabcount"]).Groups[1].Captures.Count;
                        
                }



                

        }
}
