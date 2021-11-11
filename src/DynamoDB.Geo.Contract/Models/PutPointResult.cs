using System;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class PutPointResult : GeoDataResult
    {
        public PutItemResponse PutItemResult { get; private set; }

        public PutPointResult(PutItemResponse putItemResult)
        {
            if (putItemResult == null) throw new ArgumentNullException("putItemResult");
            PutItemResult = putItemResult;
        }
    }
}
