using System;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class GetPointResult : GeoDataResult
    {
        public GetItemResponse GetItemResult { get; private set; }

        public GetPointResult(GetItemResponse getItemResult)
        {
            if (getItemResult == null) throw new ArgumentNullException("getItemResult");
            GetItemResult = getItemResult;
        }
    }
}
