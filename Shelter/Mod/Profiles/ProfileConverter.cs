using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mod.Profiles
{
    public class ProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Profile);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var profile = (Profile) value;
            serializer.Serialize(writer, profile.Properties);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var properties = serializer.Deserialize<Dictionary<string, string>>(reader); // TODO: Manual deserialize using reader and specify dictionary capacity
            return new Profile
            {
                Properties = properties
            };
        }
    }
}