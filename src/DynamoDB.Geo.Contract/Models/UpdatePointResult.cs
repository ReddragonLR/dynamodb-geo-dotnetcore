using System;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class UpdatePointResult : GeoDataResult
    {
        public UpdateItemResponse UpdateItemResult { get; private set; }

        public UpdatePointResult(UpdateItemResponse updateItemResult)
        {
            if (updateItemResult == null) throw new ArgumentNullException("updateItemResult");
            UpdateItemResult = updateItemResult;
        }
    }
}
