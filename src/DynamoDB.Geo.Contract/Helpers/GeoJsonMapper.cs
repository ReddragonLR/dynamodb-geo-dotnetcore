using DynamoDB.Geo.Contract.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DynamoDB.Geo.Contract.Helpers
{
    public static class GeoJsonMapper
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        static GeoJsonMapper()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public static GeoPoint GeoPointFromString(string jsonString)
        {
            return JsonConvert.DeserializeObject<GeoPoint>(jsonString, JsonSerializerSettings);
        }

        public static string StringFromGeoObject(GeoObject geoObject)
        {
            return JsonConvert.SerializeObject(geoObject, JsonSerializerSettings);
        }
    }
}
