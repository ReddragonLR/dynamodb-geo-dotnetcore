using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Enums;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDB.Geo.Tests
{
    public sealed class UnitTestDynamoDBRepositoryClientFactory : IRepositoryClientFactory<IAmazonDynamoDB>
    {
        private Mock<IAmazonDynamoDB> _mockDynamoDBClient;
        public UnitTestDynamoDBRepositoryClientFactory()
        {
            _mockDynamoDBClient = new Mock<IAmazonDynamoDB>();
            List<QueryResponse> mockResponses = SetupMockResponses();

            _mockDynamoDBClient.SetupSequence(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockResponses[0]))
                .Returns(Task.FromResult(mockResponses[1]))
                .Returns(Task.FromResult(mockResponses[2]))
                .Returns(Task.FromResult(mockResponses[3]))
                .Returns(Task.FromResult(mockResponses[4]))
                .Returns(Task.FromResult(mockResponses[5]))
                .Returns(Task.FromResult(mockResponses[6]))
                .Returns(Task.FromResult(mockResponses[7]))
                .Returns(Task.FromResult(mockResponses[8]))
                .Returns(Task.FromResult(mockResponses[9]))
                .Returns(Task.FromResult(mockResponses[10]))
                .Returns(Task.FromResult(mockResponses[11]));
        }

        private List<QueryResponse> SetupMockResponses()
        {
            List<QueryResponse> mockResponses = new List<QueryResponse>();

            var path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data.json");
            if (!File.Exists(path1))
                throw new Exception("File not found");
            string text1 = File.ReadAllText(path1);

            var path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-2.json");
            if (!File.Exists(path2))
                throw new Exception("File not found");
            string text2 = File.ReadAllText(path2);

            var path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-3.json");
            if (!File.Exists(path3))
                throw new Exception("File not found");
            string text3 = File.ReadAllText(path3);

            var path4 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-4.json");
            if (!File.Exists(path4))
                throw new Exception("File not found");
            string text4 = File.ReadAllText(path4);

            var path5 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-5.json");
            if (!File.Exists(path5))
                throw new Exception("File not found");
            string text5 = File.ReadAllText(path5);

            var path6 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-6.json");
            if (!File.Exists(path6))
                throw new Exception("File not found");
            string text6 = File.ReadAllText(path6);

            var path7 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-7.json");
            if (!File.Exists(path7))
                throw new Exception("File not found");
            string text7 = File.ReadAllText(path7);

            var path8 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-8.json");
            if (!File.Exists(path8))
                throw new Exception("File not found");
            string text8 = File.ReadAllText(path8);

            var path9 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-9.json");
            if (!File.Exists(path9))
                throw new Exception("File not found");
            string text9 = File.ReadAllText(path9);

            var path10 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-10.json");
            if (!File.Exists(path10))
                throw new Exception("File not found");
            string text10 = File.ReadAllText(path10);

            var path11 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-11.json");
            if (!File.Exists(path11))
                throw new Exception("File not found");
            string text11 = File.ReadAllText(path11);

            var path12 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData\\mock-data-12.json");
            if (!File.Exists(path12))
                throw new Exception("File not found");
            string text12 = File.ReadAllText(path12);

            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text1));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text2));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text3));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text4));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text5));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text6));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text7));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text8));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text9));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text10));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text11));
            mockResponses.Add(JsonConvert.DeserializeObject<QueryResponse>(text12));
            return mockResponses;
        }

        public IAmazonDynamoDB BuildRepositoryClient(DataRegion dataRegion)
        {
            return _mockDynamoDBClient.Object;
        }
    }
}
