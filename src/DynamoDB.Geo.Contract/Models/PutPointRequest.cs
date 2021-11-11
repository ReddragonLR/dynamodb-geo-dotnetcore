using System;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    /// <summary>
    /// Put point request. The request must specify a geo point and a range key value. You can modify PutItemRequest to
    /// customize the underlining Amazon DynamoDB put item request, but the table name, hash key, geohash, and geoJson
    /// attribute will be overwritten by GeoDataManagerConfiguration.
    /// </summary>
    public sealed class PutPointRequest : GeoDataRequest
    {
        public GeoPoint GeoPoint { get; private set; }
        public AttributeValue RangeKeyValue { get; private set; }

        public PutItemRequest PutItemRequest { get; private set; }

        public PutPointRequest(GeoPoint geoPoint, AttributeValue rangeKeyValue, DataRegion dataRegion)
            : base(dataRegion)
        {
            if (geoPoint == null) throw new ArgumentNullException("geoPoint");
            if (rangeKeyValue == null) throw new ArgumentNullException("rangeKeyValue");

            PutItemRequest = new PutItemRequest();
            GeoPoint = geoPoint;
            RangeKeyValue = rangeKeyValue;
        }
    }
}
