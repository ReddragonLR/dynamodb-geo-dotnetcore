using System;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    /// <summary>
    /// Update point request. The request must specify a geo point and a range key value. You can modify UpdateItemRequest to
    /// customize the underlining Amazon DynamoDB update item request, but the table name, hash key, geohash, and geoJson
    /// attribute will be overwritten by GeoDataManagerConfiguration.
    /// </summary>
    public sealed class UpdatePointRequest : GeoDataRequest
    {
        public GeoPoint GeoPoint { get; private set; }
        public AttributeValue RangeKeyValue { get; private set; }
        public UpdateItemRequest UpdateItemRequest { get; private set; }

        public UpdatePointRequest(GeoPoint geoPoint, AttributeValue rangeKeyValue, DataRegion dataRegion)
            : base(dataRegion)
        {
            if (geoPoint == null) throw new ArgumentNullException("geoPoint");
            if (rangeKeyValue == null) throw new ArgumentNullException("rangeKeyValue");

            UpdateItemRequest = new UpdateItemRequest();
            GeoPoint = geoPoint;
            RangeKeyValue = rangeKeyValue;
        }
    }
}
