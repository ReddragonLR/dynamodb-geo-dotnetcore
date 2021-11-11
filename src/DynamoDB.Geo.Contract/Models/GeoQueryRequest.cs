using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract.Models
{
    public class GeoQueryRequest : GeoDataRequest
    {
        public QueryRequest QueryRequest { get; private set; }

        public GeoQueryRequest(DataRegion dataRegion)
            : base(dataRegion)
        {
            QueryRequest = new QueryRequest();
        }

    }
}
