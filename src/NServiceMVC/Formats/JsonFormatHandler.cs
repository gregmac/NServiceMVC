using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using System.Globalization;


namespace NServiceMVC.Formats
{
    public class JsonFormatHandler : IFormatHandler
    {
        public JsonFormatHandler()
        {
            NormalSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = NServiceMVC.Configuration.JsonCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
            };

            HumanSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = NServiceMVC.Configuration.JsonCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
            };


            switch (NServiceMVC.Configuration.JsonDateFormat)
            {
                case NServiceMVC.NsConfiguration.JsonDateFormatType.IsoTime:
                    NormalSettings.Converters.Add(new IsoDateTimeConverter());
                    HumanSettings.Converters.Add(new IsoDateTimeConverter());
                    break;
            }


            NormalSettings.Converters.Add(new StringEnumConverter());
            HumanSettings.Converters.Add(new StringEnumConverter());
        }


        protected JsonSerializerSettings NormalSettings { get; set; }
        protected JsonSerializerSettings HumanSettings { get; set; }

        public string Serialize(object model, bool humanReadable = false)
        {
            if (humanReadable)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    model,
                    Newtonsoft.Json.Formatting.Indented,
                    HumanSettings
                );
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(
                    model,
                    Newtonsoft.Json.Formatting.None,
                    NormalSettings
                );
            }
        }

        public object Deserialize(string representation, Type modelType)
        {
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject(representation, modelType, NormalSettings);
            return model;
        }


    }
}
