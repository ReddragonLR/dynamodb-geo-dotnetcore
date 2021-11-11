using System;
using Amazon.DynamoDBv2.Model;

namespace DynamoDB.Geo.Contract.Models
{
    public sealed class DeletePointResult : GeoDataResult
    {
        public DeletePointResult(DeleteItemResponse deleteItemResult)
        {
            if (deleteItemResult == null) throw new ArgumentNullException("deleteItemResult");

            DeleteItemResult = deleteItemResult;
        }

        public DeleteItemResponse DeleteItemResult { get; private set; }
    }
}
