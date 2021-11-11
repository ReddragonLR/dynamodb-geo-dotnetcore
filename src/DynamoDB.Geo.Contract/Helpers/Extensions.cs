using Amazon.DynamoDBv2.Model;
using System.Linq;

namespace DynamoDB.Geo.Contract.Helpers
{
    public static class Extensions
    {
        public static QueryRequest CopyQueryRequest(this QueryRequest queryRequest)
        {
            var copiedRequest = new QueryRequest
            {
                AttributesToGet = queryRequest.AttributesToGet.ToList(), // deep copy
                ConsistentRead = queryRequest.ConsistentRead,
                ExclusiveStartKey = queryRequest.ExclusiveStartKey.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), // deep copy
                IndexName = queryRequest.IndexName,
                KeyConditions = queryRequest.KeyConditions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                ReturnConsumedCapacity = queryRequest.ReturnConsumedCapacity,
                ScanIndexForward = queryRequest.ScanIndexForward,
                Select = queryRequest.Select,
                TableName = queryRequest.TableName
            };

            // This is necessary because Limit is not a required parameter
            // But AWS's QueryRequest will always return 0 if you get it.
            // This IsLimitSet method is internal-only.
            if (queryRequest.Limit > 0)
                copiedRequest.Limit = queryRequest.Limit;

            return copiedRequest;
        }
    }
}
