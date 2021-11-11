/*
 * See explanation for HashKeyLength vs search resolution: 
 * https://acloudguru.com/blog/engineering/location-based-search-results-with-dynamodb-and-geohash
 */

using Microsoft.Extensions.Options;

namespace DynamoDB.Geo
{
    public sealed class GeoDataClientOptions : IOptions<GeoDataClientOptions>
    {
        // Default values
        private const string DefaultHashkeyAttributeName = "HashKey";
        private const string DefaultRangekeyAttributeName = "RangeKey";
        private const string DefaultGeohashAttributeName = "GeoHash";
        private const string DefaultGeojsonAttributeName = "GeoJson";
        private const string DefaultGeohashIndexAttributeName = "Geohash-Index";
        private const int DefaultHashkeyLength = 5;

        public GeoDataClientOptions()
        {
            HashKeyAttributeName = DefaultHashkeyAttributeName;
            RangeKeyAttributeName = DefaultRangekeyAttributeName;
            GeohashAttributeName = DefaultGeohashAttributeName;
            GeoJsonAttributeName = DefaultGeojsonAttributeName;
            GeohashIndexName = DefaultGeohashIndexAttributeName;
            HashKeyLength = DefaultHashkeyLength;
        }
        public string TableName { get; set; }
        public string HashKeyAttributeName { get; set; }
        public string RangeKeyAttributeName { get; set; }
        public string GeohashAttributeName { get; set; }
        public string GeoJsonAttributeName { get; set; }
        public string GeohashIndexName { get; set; }
        public int HashKeyLength { get; set; }
        public GeoDataClientOptions Value => this;
    }
}
