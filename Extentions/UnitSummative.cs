using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
    public static class UnitSummative
    {

        public static bool InCompleteCheck(string unitSaList, string completedSaList)
        {

            var unitSums = new List<int>();
            var completedSums = new List<int>();

            bool inComplete = false;

            if (unitSaList != null && unitSaList != "")
            {

                unitSums = OrderList.ItemIdOrder(unitSaList);

                if (completedSaList != null && completedSaList != "")
                {

                    completedSums = OrderList.ItemIdOrder(completedSaList);

                    foreach (var sum in unitSums)
                    {

                        inComplete = (!completedSums.Contains(sum)) ? true : inComplete = false;

                    }
                }

            }

            return inComplete;

        }



    }
}
