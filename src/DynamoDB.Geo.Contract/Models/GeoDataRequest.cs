using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    public abstract class GeoDataRequest
    {
        public GeoDataRequest(DataRegion dataRegion)
        {
            DataRegion = dataRegion;
        }

        public DataRegion DataRegion { get; }
    }
}
