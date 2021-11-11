using System;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    /// <summary>
    /// Delete point request. The request must specify a geo point and a range key value. You can modify DeleteItemRequest to
    /// customize the underlining Amazon DynamoDB delete item request, but the table name, hash key, geohash, and geoJson
    /// attribute will be overwritten by GeoDataManagerConfiguration.
    /// </summary>
    public sealed class DeletePointRequest : GeoDataRequest
    {

        public DeletePointRequest(GeoPoint geoPoint, AttributeValue rangeKeyValue, DataRegion dataRegion)
            : base(dataRegion)
        {
            if (geoPoint == null) throw new ArgumentNullException("geoPoint");
            if (rangeKeyValue == null) throw new ArgumentNullException("rangeKeyValue");

            DeleteItemRequest = new DeleteItemRequest();
            GeoPoint = geoPoint;
            RangeKeyValue = rangeKeyValue;
        }

        public DeleteItemRequest DeleteItemRequest { get; private set; }
        public GeoPoint GeoPoint { get; private set; }
        public AttributeValue RangeKeyValue { get; private set; }
    }
}
