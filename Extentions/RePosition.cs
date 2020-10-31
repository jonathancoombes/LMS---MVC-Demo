using LMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
       
    public static class RePosition 

    {
        private static int _moveValue;
        private static int _moveup = -1;
        private static int _movedown = +1;
        
        public static string MoveUp(string currentOrder, int Id)
        {
            return Move(currentOrder, Id, "up");
        }

        public static string MoveDown(string currentOrder, int Id)
        {
            return Move(currentOrder, Id, "down");
        }


        private static string Move(string currentOrder, int Id, string direction)
        {   
            if (direction == "up") {
                _moveValue = _moveup;
            }
            else if (direction == "down") {
                _moveValue = _movedown;
            }

            var elements = currentOrder?.Split(',').ToList();
            int[] sortedElements = new int[elements.Count()];

            // Selected Items's current element position
            int selectedItemLocation = elements.IndexOf(Id.ToString());
            var elementValue = -1;
            int.TryParse(Id.ToString(), out elementValue);

            // Existing item into Array
            sortedElements[selectedItemLocation + _moveValue] = elementValue;
           
            // Remaining items into Array
            foreach (var item in elements)
            {
                var otherItemsLocation = elements.IndexOf(item);

                if ((item != sortedElements[selectedItemLocation + _moveValue].ToString()) && (otherItemsLocation == selectedItemLocation + _moveValue))
                {
                    int.TryParse(item, out elementValue);
                    sortedElements[selectedItemLocation] = elementValue;
                }

                else if (item != Id.ToString() && item != sortedElements[selectedItemLocation].ToString())
                {
                    var remItemsLocation = elements.IndexOf(item);
                   
                    int.TryParse(item, out elementValue);
                    sortedElements[remItemsLocation] = elementValue;
                }
            }
            return string.Join(",", sortedElements);
        }

        public static bool CanMoveDownCheck(string currentOrder, int Id)
        {
            var elements = currentOrder?.Split(',').ToList();
            bool possible;
            if (elements == null || Id.ToString() == elements[elements.Count() - 1])
            {
                possible = false;
            }
            else
            {
                possible = true;
            }
            return possible;
        }
        

        public static bool CanMoveUpCheck(string currentOrder, int Id)
        {
            var elements = currentOrder?.Split(',').ToList();
            bool possible;

            if (elements == null || Id.ToString() == elements[0])
            {
                possible = false;
            }
            else
            {
                possible = true;
            }
            return possible;
        }


        public static string Delete(string currentOrder, int Id)
        {
            var elements = currentOrder?.Split(',').ToList();

            int[] sortedElements = new int[elements.Count() - 1];

            elements.Remove(Id.ToString());

            foreach (var item in elements)
            {
                    var remItemsLocation = elements.IndexOf(item);
                    var itemValue = -1;
                    int.TryParse(item, out itemValue);
                    sortedElements[remItemsLocation] = itemValue;
                
            }
            return string.Join(",", sortedElements);
        }
           
 
    }
}
