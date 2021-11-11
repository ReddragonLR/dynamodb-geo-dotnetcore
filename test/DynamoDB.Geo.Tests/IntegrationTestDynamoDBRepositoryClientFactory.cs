using Amazon;
using Amazon.DynamoDBv2;
using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Tests
{
    public class IntegrationTestDynamoDBRepositoryClientFactory : IRepositoryClientFactory<IAmazonDynamoDB>
    {
        const string AwsAccessId = "<AWS ACCESS ID>";
        const string AwsSecretKey = "<AWS SECRET KEY>";

        public IAmazonDynamoDB BuildRepositoryClient(DataRegion dataRegion)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(AwsAccessId, AwsSecretKey);
            return new AmazonDynamoDBClient(awsCredentials, RegionEndpoint.AFSouth1);
        }
    }
}
