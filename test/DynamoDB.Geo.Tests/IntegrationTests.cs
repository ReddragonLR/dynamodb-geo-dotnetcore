using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Models;
using Geolocation;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DynamoDB.Geo.Tests
{
    [TestClass]
    [Ignore("These are integration tests that were used as part of development")]
    public class IntegrationTests : TestBase
    {
        private readonly IGeoDataClient _client;
        public IntegrationTests()
        {
            var factory = new IntegrationTestDynamoDBRepositoryClientFactory();
            var options = new GeoDataClientOptions()
            {
                TableName = "Stores"
            };
            _client = new GeoDataClient(Options.Create(options), factory);
        }

        [TestInitialize]
        public async Task Setup()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\school_list_wa.txt");

                if (!File.Exists(path))
                    throw new Exception("File not found");

                foreach (var line in File.ReadLines(path))
                {
                    var columns = line.Split('\t');
                    var schoolId = columns[0];
                    var schoolName = columns[1];
                    var latitude = double.Parse(columns[2], CultureInfo.InvariantCulture);
                    var longitude = double.Parse(columns[3], CultureInfo.InvariantCulture);

                    var point = new GeoPoint(latitude, longitude);

                    var rangeKeyVal = new AttributeValue { S = schoolId };
                    var schoolNameVal = new AttributeValue { S = schoolName };

                    var req = new PutPointRequest(point, rangeKeyVal, Contract.Enums.DataRegion.AF_SOUTH_1);
                    req.PutItemRequest.Item["SchoolName"] = schoolNameVal;

                    await _client.PutPointAsync(req);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

        [TestCleanup]
        public async Task CleanUp()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\school_list_wa.txt");
                if (!File.Exists(path))
                    throw new Exception("File not found");

                foreach (var line in File.ReadLines(path))
                {
                    var columns = line.Split('\t');
                    var schoolId = columns[0];
                    var schoolName = columns[1];
                    var latitude = double.Parse(columns[2], CultureInfo.InvariantCulture);
                    var longitude = double.Parse(columns[3], CultureInfo.InvariantCulture);

                    var point = new GeoPoint(latitude, longitude);
                    var rangeKeyVal = new AttributeValue { S = schoolId };

                    var req = new DeletePointRequest(point, rangeKeyVal, Contract.Enums.DataRegion.AF_SOUTH_1);

                    await _client.DeletePointAsync(req);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
