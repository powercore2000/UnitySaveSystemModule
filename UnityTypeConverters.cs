using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>Serializes Vector3 as {x, y, z} — avoids infinite loops from computed properties like 'normalized'.</summary>
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, Newtonsoft.Json.JsonSerializer serializer)
        {
            new JObject { { "x", value.x }, { "y", value.y }, { "z", value.z } }.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new Vector3(
                jo["x"]?.Value<float>() ?? 0f,
                jo["y"]?.Value<float>() ?? 0f,
                jo["z"]?.Value<float>() ?? 0f
            );
        }
    }

    /// <summary>Serializes Vector4 as {x, y, z, w} — avoids infinite loops from computed properties like 'normalized'.</summary>
    public class Vector4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, Newtonsoft.Json.JsonSerializer serializer)
        {
            new JObject { { "x", value.x }, { "y", value.y }, { "z", value.z }, { "w", value.w } }.WriteTo(writer);
        }

        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new Vector4(
                jo["x"]?.Value<float>() ?? 0f,
                jo["y"]?.Value<float>() ?? 0f,
                jo["z"]?.Value<float>() ?? 0f,
                jo["w"]?.Value<float>() ?? 0f
            );
        }
    }

    /// <summary>Serializes Color as {r, g, b, a} — avoids infinite loops from computed properties like 'gamma' and 'linear'.</summary>
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, Newtonsoft.Json.JsonSerializer serializer)
        {
            new JObject { { "r", value.r }, { "g", value.g }, { "b", value.b }, { "a", value.a } }.WriteTo(writer);
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new Color(
                jo["r"]?.Value<float>() ?? 0f,
                jo["g"]?.Value<float>() ?? 0f,
                jo["b"]?.Value<float>() ?? 0f,
                jo["a"]?.Value<float>() ?? 1f
            );
        }
    }
}
