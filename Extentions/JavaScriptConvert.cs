using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Support
{
    public static class JavaScriptConvert
    {
        public static HtmlString SerializeObject(object value)
        {

            using (var stringwriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringwriter))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                jsonWriter.QuoteName = false;
                serializer.Serialize(jsonWriter, value);

                return new HtmlString(stringwriter.ToString());

            }


        }
    }
}

