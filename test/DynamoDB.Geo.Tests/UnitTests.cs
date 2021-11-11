using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Models;
using Geolocation;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DynamoDB.Geo.Tests
{
    [TestClass]
    public class UnitTests : TestBase
    {
        private readonly IGeoDataClient _client;
        public UnitTests()
        {
            var factory = new UnitTestDynamoDBRepositoryClientFactory();
            var options = new GeoDataClientOptions()
            {
                TableName = "Stores"
            };
            _client = new GeoDataClient(Options.Create(options), factory);
        }

        [TestMethod]
        public async Task QueryRadiusWithin10KM_Success()
        {
            // ARRANGE
            double latitude = 47.65017;
            double longitude = -117.20632;
            Coordinate origin = new Coordinate(latitude, longitude);

            var point = new GeoPoint(latitude, longitude);
            double radiusInMeters = 10000; // 10 KM

            var request = new QueryRadiusRequest(point, radiusInMeters, Contract.Enums.DataRegion.AF_SOUTH_1);

            // ACT
            var result = await _client.QueryRadiusAsync(request);

            // ASSERT
            Assert.IsTrue(result.Items.Count == 43);
            var searchResults = GetResultsFromQuery(result);
            foreach (var searchResult in searchResults)
            {
                Coordinate destination = new Coordinate(searchResult.Latitude, searchResult.Longitude);
                var distanceInMeters = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Meters);
                if (distanceInMeters > radiusInMeters)
                    Assert.Fail();
            }
        }
    }
}
