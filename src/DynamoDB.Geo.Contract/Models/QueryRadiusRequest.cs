using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class QueryRadiusRequest : GeoQueryRequest
    {
        public GeoPoint CenterPoint { get; private set; }
        public double RadiusInMeter { get; private set; }

        public QueryRadiusRequest(GeoPoint centerPoint, double radiusInMeter, DataRegion dataRegion)
            : base(dataRegion)
        {
            CenterPoint = centerPoint;
            RadiusInMeter = radiusInMeter;
        }
    }
}
