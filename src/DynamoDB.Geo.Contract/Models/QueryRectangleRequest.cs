using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class QueryRectangleRequest : GeoQueryRequest
    {
        public GeoPoint MinPoint { get; private set; }
        public GeoPoint MaxPoint { get; private set; }

        public QueryRectangleRequest(GeoPoint minPoint, GeoPoint maxPoint, DataRegion dataRegion)
            : base(dataRegion)
        {
            MinPoint = minPoint;
            MaxPoint = maxPoint;
        }
    }
}
