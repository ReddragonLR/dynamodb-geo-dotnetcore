namespace DynamoDB.Geo.Contract
{
    public abstract class GeoDataModelBase
    {
        public ulong HashKey { get; set; }
        public string RangeKey { get; set; }
        public string GeoJson { get; set; }
        public string GeoHash { get; set; }
    }
}
