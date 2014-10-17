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
                private int currentIndex = -1;

                private Dictionary<TriggerType, Func<Trigger>> triggerProduct = new Dictionary<TriggerType, Func<Trigger>>();
                private Dictionary<TriggerType, string> triggerRegex = new Dictionary<TriggerType, String>();

                // Use a LIFO stack to convert, track and chain tabs (\t) to trigger indexes.
                private Stack<int> tabCountStack = new Stack<int>();

                //trigger values
                private int triggerIndex;
                private TriggerType triggerType;
                private string description;
                private bool ascending = true;
                private double triggerValue;
                private bool triggerfromMax;

                // Regex patterns
                private string oneParamRegex = @"^\t*\w+\s+(\d+)\s*$";
                private string oneParamFromMaxValRegex = @"^\t*\w+\s+(\d+)\s*(\w*)\s*$";
                private string oneWordRegex = @"^\w+\s*";


                public TriggerFactory()
                {
                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(triggerIndex, triggerType, description, ascending, triggerfromMax, triggerValue); });


                        triggerRegex.Add(TriggerType.ASCENT, oneWordRegex);
                        triggerRegex.Add(TriggerType.DESCENT, oneWordRegex);
                        triggerRegex.Add(TriggerType.ALTITUDE, oneParamFromMaxValRegex);
                        
                
                }

                public int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {

                        if (IsValidSyntax(trigger, commandLine, lineNumber))
                        {
                                if(trigger == TriggerType.ASCENT)
                                {
                                        ascending = true;
                                        return -1;
                                }
                                else if (trigger == TriggerType.DESCENT)
                                {
                                        ascending = false;
                                        return -1;
                                }

                                currentIndex++;

                                Debug.Log("IsValidSyntax: "+trigger.ToString());

                                int tabcount = GetTabCount(commandLine);

                                Debug.Log("tabcount: " + tabcount);

                                if (tabcount == 0)
                                {
                                        tabCountStack.Clear();
                                        tabCountStack.Push(currentIndex);
                                }
                                else if (tabcount == tabCountStack.Count - 1)
                                {
                                        tabCountStack.Pop();
                                        tabCountStack.Push(currentIndex);
                                }
                                else if (tabcount > tabCountStack.Count - 1)
                                {
                                        tabCountStack.Push(currentIndex);
                                }
                                else if(tabcount < tabCountStack.Count - 1)
                                {
                                        for (int i = 0; i < (tabCountStack.Count - tabcount); i++)
                                        {
                                                tabCountStack.Pop();
                                        }

                                        tabCountStack.Push(currentIndex);
                                }


                        }
                        else 
                        {
                                Debug.Log("Profile Loader: Invalid Syntax!: Line #"+lineNumber+": "+commandLine);
                        }



                        triggerIndex = currentIndex;
                        triggerType = trigger;
                        description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trigger.ToString());

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


                public bool IsValidSyntax(TriggerType trigger, string commandLine, int lineNumber)
                {


                        Match match = Regex.Match(commandLine, triggerRegex[trigger]);

                        if (match.Success)
                        {
                                switch(trigger)
                                {
                                        case TriggerType.ALTITUDE:

                                                if (match.Groups[2].Value == "FROMMAXVAL")
                                                {
                                                        triggerfromMax = true;
                                                        triggerValue = Convert.ToDouble(match.Groups[1].Value);
                                                }
                                                else
                                                {
                                                        triggerfromMax = false;
                                                        triggerValue = Convert.ToDouble(match.Groups[1].Value);
                                                }

                                                //REMEMBER TO CREATE DICTIONARY FOR TRIGGER FUNCTIONS TO GET VALUES INTO CLASS // may not need this.. just create regular methods or put into Dict.Action

                                                Debug.Log("altitude captures count!: " + match.Groups[1].Captures.Count + " " + match.Captures.Count + " group 0: " + match.Groups[0].Value + "  value1: " + match.Groups[1].Value + "  value2: " + match.Groups[2].Value + "  value3: " + match.Groups[3].Value);

                                                return true;
                                
                                
                                }

                                return true;
                        }
  
   
                        return false;

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
