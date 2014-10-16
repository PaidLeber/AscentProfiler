using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                private int index;
                private string type;
                private string desc;
                private bool ascending;
                private double value;

                // Regex patterns
                private string oneParam = @"^\t*\w+\s+(\d+)\s*$";



                public TriggerFactory()
                {
                        triggerProduct.Add(TriggerType.ALTITUDE, () => { return new Altitude(index, type, desc, ascending, value); });



                        triggerRegex.Add(TriggerType.ALTITUDE, oneParam);
                        //carBuilder.Add("van", () => { return new Van(); });
                
                }

                public int CreateTrigger(TriggerType trigger, string commandLine, int lineNumber)
                {

                        if (IsValidSyntax(trigger, commandLine, lineNumber))
                        {
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

                        //Trigger temp = new Altitude(1, TriggerType.ALTITUDE.ToString(), "Altitude", true, 10000);

                        //AscentProfiler.ActiveProfile.triggerGuardian.tdictionary.Add(currentIndex, temp);


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

                        if (Regex.IsMatch(commandLine, triggerRegex[TriggerType.ALTITUDE]))
                        {
                                Debug.Log(TriggerType.ALTITUDE.ToString() + "Is Match!");
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
