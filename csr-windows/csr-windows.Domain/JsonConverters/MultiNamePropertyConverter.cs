using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain
{
    public class MultiNamePropertyConverter<T> : JsonConverter
    {
        private readonly string[] _propertyNames;

        public MultiNamePropertyConverter(params string[] propertyNames)
        {
            _propertyNames = propertyNames;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            foreach (var name in _propertyNames)
            {
                if (jo[name] != null)
                {
                    return jo[name].ToObject<T>(serializer);
                }
            }

            return default(T);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not necessary for this example.");
        }
    }
}
