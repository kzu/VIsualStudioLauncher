// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using VisualStudioLauncher;
//
//    var setupInstance = SetupInstance.FromJson(jsonString);

namespace VisualStudioLauncher
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SetupInstance
    {
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("installDate")]
        public DateTimeOffset InstallDate { get; set; }

        [JsonProperty("installationName")]
        public string InstallationName { get; set; }

        [JsonProperty("installationPath")]
        public string InstallationPath { get; set; }

        [JsonProperty("installationVersion")]
        public string InstallationVersion { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("productPath")]
        public string ProductPath { get; set; }

        [JsonProperty("isPrerelease")]
        public bool IsPrerelease { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("channelUri")]
        public string ChannelUri { get; set; }

        [JsonProperty("enginePath")]
        public string EnginePath { get; set; }

        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("thirdPartyNotices")]
        public string ThirdPartyNotices { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset UpdateDate { get; set; }

        [JsonProperty("catalog")]
        public Catalog Catalog { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public partial class Catalog
    {
        [JsonProperty("_")]
        public long Empty { get; set; }

        [JsonProperty("buildBranch")]
        public string BuildBranch { get; set; }

        [JsonProperty("buildVersion")]
        public string BuildVersion { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("localBuild")]
        public string LocalBuild { get; set; }

        [JsonProperty("manifestName")]
        public string ManifestName { get; set; }

        [JsonProperty("manifestType")]
        public string ManifestType { get; set; }

        [JsonProperty("productDisplayVersion")]
        public string ProductDisplayVersion { get; set; }

        [JsonProperty("productLine")]
        public string ProductLine { get; set; }

        [JsonProperty("productLineVersion")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ProductLineVersion { get; set; }

        [JsonProperty("productMilestone")]
        public string ProductMilestone { get; set; }

        [JsonProperty("productMilestoneIsPreRelease")]
        public string ProductMilestoneIsPreRelease { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productPatchVersion")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ProductPatchVersion { get; set; }

        [JsonProperty("productPreReleaseMilestoneSuffix")]
        public string ProductPreReleaseMilestoneSuffix { get; set; }

        [JsonProperty("productRelease")]
        public string ProductRelease { get; set; }

        [JsonProperty("productSemanticVersion")]
        public string ProductSemanticVersion { get; set; }

        [JsonProperty("requiredEngineVersion")]
        public string RequiredEngineVersion { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("campaignId")]
        public string CampaignId { get; set; }

        [JsonProperty("canceled")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Canceled { get; set; }

        [JsonProperty("channelManifestId")]
        public string ChannelManifestId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("setupEngineFilePath")]
        public string SetupEngineFilePath { get; set; }
    }

    public partial class SetupInstance
    {
        public static SetupInstance[] FromJson(string json) => JsonConvert.DeserializeObject<SetupInstance[]>(json, VisualStudioLauncher.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SetupInstance[] self) => JsonConvert.SerializeObject(self, VisualStudioLauncher.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
