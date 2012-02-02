using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace NServiceMVC.Formats.Json
{
    abstract class JsonNetSerializerSettings
    {
        public static JsonSerializerSettings CreateResponseSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = NServiceMVC.Configuration.JsonCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
            };
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return jsonSerializerSettings;
        }

        public static JsonSerializerSettings CreateHumanReadableSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = NServiceMVC.Configuration.JsonCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
            };
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            
            return jsonSerializerSettings;
        }
    }
}
