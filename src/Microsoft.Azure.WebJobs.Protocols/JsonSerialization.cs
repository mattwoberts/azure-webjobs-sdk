﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if PUBLICPROTOCOL
namespace Microsoft.Azure.WebJobs.Protocols
#else
namespace Microsoft.Azure.WebJobs.Host.Protocols
#endif
{
    /// <summary>Provides the standard <see cref="JsonSerializerSettings"/> used by protocol data.</summary>
#if PUBLICPROTOCOL
    public static class JsonSerialization
#else
    internal static class JsonSerialization
#endif
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            // The default value, DateParseHandling.DateTime, drops time zone information from DateTimeOffets.
            // This value appears to work well with both DateTimes (without time zone information) and DateTimeOffsets.
            DateParseHandling = DateParseHandling.DateTimeOffset,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        private static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault(JsonSerializerSettings);

        /// <summary>Gets the standard <see cref="JsonSerializerSettings"/> used by protocol data.</summary>
        public static JsonSerializerSettings Settings
        {
            get { return JsonSerializerSettings; }
        }

        internal static JsonSerializer Serializer
        {
            get { return JsonSerializer; }
        }

        internal static void ApplySettings(JsonReader reader)
        {
            if (reader == null)
            {
                return;
            }

            reader.Culture = JsonSerializerSettings.Culture;
            reader.DateFormatString = JsonSerializerSettings.DateFormatString;
            reader.DateParseHandling = JsonSerializerSettings.DateParseHandling;
            reader.DateTimeZoneHandling = JsonSerializerSettings.DateTimeZoneHandling;
            reader.FloatParseHandling = Serializer.FloatParseHandling;
        }

        internal static void ApplySettings(JsonWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.Culture = JsonSerializerSettings.Culture;
            writer.DateFormatHandling = JsonSerializerSettings.DateFormatHandling;
            writer.DateFormatString = JsonSerializerSettings.DateFormatString;
            writer.DateTimeZoneHandling = JsonSerializerSettings.DateTimeZoneHandling;
            writer.FloatFormatHandling = JsonSerializerSettings.FloatFormatHandling;
            writer.Formatting = JsonSerializerSettings.Formatting;
            writer.StringEscapeHandling = JsonSerializerSettings.StringEscapeHandling;
        }

        internal static JsonTextReader CreateJsonTextReader(TextReader reader)
        {
            JsonTextReader jsonReader = new JsonTextReader(reader);
            ApplySettings(jsonReader);
            return jsonReader;
        }

        internal static JsonTextWriter CreateJsonTextWriter(TextWriter textWriter)
        {
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter);
            ApplySettings(jsonWriter);
            return jsonWriter;
        }

        internal static JObject ParseJObject(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            var parsed = JsonConvert.DeserializeObject(json, JsonSerializerSettings);

            if (parsed.GetType() == typeof(JObject))
            {
                return (JObject)parsed;
            }

            return null;
        }
    }
}
