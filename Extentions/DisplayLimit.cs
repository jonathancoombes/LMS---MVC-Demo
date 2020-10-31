using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
    public static class DisplayLimit
    {

        public static string StringLength(string field, int lenght) {         

            var result = field.Count() > lenght ? field.Substring(0, lenght) + "..": field;

            return result;
            }


        }

    }

