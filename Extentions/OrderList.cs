using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
    public static class OrderList
    {
        public static List<int> ItemIdOrder(string currentOrder) {

            var elements = currentOrder?.Split(',').ToList();
            List<int> orderedItems = new List<int>();

            foreach (var mods in elements)
            {
                int result;
                int.TryParse(mods, out result);
                orderedItems.Add(result);
            }

            return orderedItems;

        }


    }
}
