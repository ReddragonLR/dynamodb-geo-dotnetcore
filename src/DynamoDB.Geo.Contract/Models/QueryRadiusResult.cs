namespace DynamoDB.Geo.Contract.Models
{
    public sealed class QueryRadiusResult : GeoQueryResult
    {
        public QueryRadiusResult(GeoQueryResult result)
            : base(result)
        {
            
        }
    }
}
