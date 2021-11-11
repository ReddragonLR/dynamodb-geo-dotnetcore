using DynamoDB.Geo.Contract.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DynamoDB.Geo.Tests
{
    public abstract class TestBase
    {
        protected IEnumerable<SchoolSearchResult> GetResultsFromQuery(GeoQueryResult result)
        {
            var dtos = from item in result.Items
                       let geoJsonString = item["GeoJson"].S
                       let point = JsonConvert.DeserializeObject<GeoPoint>(geoJsonString)
                       select new SchoolSearchResult
                       {
                           Latitude = point.Latitude,
                           Longitude = point.Longitude,
                           RangeKey = item["RangeKey"].S,
                           SchoolName = item.ContainsKey("SchoolName") ? item["SchoolName"].S : string.Empty
                       };

            return dtos;
        }
    }

    public class SchoolSearchResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string RangeKey { get; set; }
        public string SchoolName { get; set; }
    }
}
