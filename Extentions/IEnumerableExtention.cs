﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
    public static class IEnumerableExtention
    {

        public static IEnumerable<SelectListItem> ToSelectLisItem<T>(this IEnumerable<T> items, int selectedValue) {

            return from item in items

                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };

        }

    }
}
