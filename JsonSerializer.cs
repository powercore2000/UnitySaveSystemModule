using System.Collections.Generic;
using Newtonsoft.Json;

namespace SaveSystem
{
    public class JsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            /// Embeds "$type" metadata only on fields declared as a base/interface type,
            /// allowing StatModifier subclasses (Perk, EmotionPerk, etc.) to round-trip correctly.
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new Vector3Converter(),
                new Vector4Converter(),
                new ColorConverter()
            }
        };

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}