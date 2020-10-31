using LMS.Data;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{
    public static class DeleteFile
    {
     
        public static void Remove(string itemdbAddressField, string webRootPath)
        {
                 var filePath = Path.Combine(webRootPath, itemdbAddressField.TrimStart('\\'));
            
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

            }



        }

    }

