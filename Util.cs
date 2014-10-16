using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{

        public static class Util
        {
                
                public static KSPActionGroup SetActionGroup(int index)
                {
                        switch (index)
                        {
                                case 1:
                                        return KSPActionGroup.Custom01;
                                case 2:
                                        return KSPActionGroup.Custom02;
                                case 3:
                                        return KSPActionGroup.Custom03;
                                case 4:
                                        return KSPActionGroup.Custom04;
                                case 5:
                                        return KSPActionGroup.Custom05;
                                case 6:
                                        return KSPActionGroup.Custom06;
                                case 7:
                                        return KSPActionGroup.Custom07;
                                case 8:
                                        return KSPActionGroup.Custom08;
                                case 9:
                                        return KSPActionGroup.Custom09;
                                case 0:
                                        return KSPActionGroup.Custom10;
                                default:
                                        throw new ArgumentException("Action group value must be between 0 and 9");
                        }
                }



        }
}
