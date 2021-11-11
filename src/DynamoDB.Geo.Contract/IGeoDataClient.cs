using DynamoDB.Geo.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDB.Geo.Contract
{
    public interface IGeoDataClient
    {
        /// <summary>
        ///     <p>
        ///         Query a circular area constructed by a center point and its radius.
        ///     </p>
        ///     <b>Sample usage:</b>
        ///     <pre>
        ///         GeoPoint centerPoint = new GeoPoint(47.5, -122.3);
        ///         QueryRadiusRequest queryRadiusRequest = new QueryRadiusRequest(centerPoint, 100);
        ///         QueryRadiusResult queryRadiusResult = geoIndexManager.queryRadius(queryRadiusRequest);
        ///         for (Map&lt;String, AttributeValue&gt; item : queryRadiusResult.getItem()) {
        ///         System.out.println(&quot;item: &quot; + item);
        ///         }
        ///     </pre>
        /// </summary>
        /// <param name="queryRadiusRequest">Container for the necessary parameters to execute radius query request.</param>
        /// <returns>Result of radius query request.</returns>
        Task<QueryRadiusResult> QueryRadiusAsync(QueryRadiusRequest queryRadiusRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     <p>
        ///         Put a point into the Amazon DynamoDB table. Once put, you cannot update attributes specified in
        ///         GeoDataManagerConfiguration: hash key, range key, geohash and geoJson. If you want to update these columns, you
        ///         need to insert a new record and delete the old record.
        ///     </p>
        ///     <b>Sample usage:</b>
        ///     <pre>
        ///         GeoPoint geoPoint = new GeoPoint(47.5, -122.3);
        ///         AttributeValue rangeKeyValue = new AttributeValue().withS(&quot;a6feb446-c7f2-4b48-9b3a-0f87744a5047&quot;);
        ///         AttributeValue titleValue = new AttributeValue().withS(&quot;Original title&quot;);
        ///         PutPointRequest putPointRequest = new PutPointRequest(geoPoint, rangeKeyValue);
        ///         putPointRequest.getPutItemRequest().getItem().put(&quot;title&quot;, titleValue);
        ///         PutPointResult putPointResult = geoDataManager.putPoint(putPointRequest);
        ///     </pre>
        /// </summary>
        /// <param name="putPointRequest">Container for the necessary parameters to execute put point request.</param>
        /// <returns>Result of put point request.</returns>
        Task<PutPointResult> PutPointAsync(PutPointRequest putPointRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     <p>
        ///         Update a point data in Amazon DynamoDB table. You cannot update attributes specified in
        ///         GeoDataManagerConfiguration: hash key, range key, geohash and geoJson. If you want to update these columns, you
        ///         need to insert a new record and delete the old record.
        ///     </p>
        ///     <b>Sample usage:</b>
        ///     <pre>
        ///         GeoPoint geoPoint = new GeoPoint(47.5, -122.3);
        ///         String rangeKey = &quot;a6feb446-c7f2-4b48-9b3a-0f87744a5047&quot;;
        ///         AttributeValue rangeKeyValue = new AttributeValue().withS(rangeKey);
        ///         UpdatePointRequest updatePointRequest = new UpdatePointRequest(geoPoint, rangeKeyValue);
        ///         AttributeValue titleValue = new AttributeValue().withS(&quot;Updated title.&quot;);
        ///         AttributeValueUpdate titleValueUpdate = new AttributeValueUpdate().withAction(AttributeAction.PUT)
        ///         .withValue(titleValue);
        ///         updatePointRequest.getUpdateItemRequest().getAttributeUpdates().put(&quot;title&quot;, titleValueUpdate);
        ///         UpdatePointResult updatePointResult = geoIndexManager.updatePoint(updatePointRequest);
        ///     </pre>
        /// </summary>
        /// <param name="updatePointRequest">Container for the necessary parameters to execute update point request.</param>
        /// <returns>Result of update point request.</returns>
        Task<UpdatePointResult> UpdatePointAsync(UpdatePointRequest updatePointRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     <p>
        ///         Delete a point from the Amazon DynamoDB table.
        ///     </p>
        ///     <b>Sample usage:</b>
        ///     <pre>
        ///         GeoPoint geoPoint = new GeoPoint(47.5, -122.3);
        ///         String rangeKey = &quot;a6feb446-c7f2-4b48-9b3a-0f87744a5047&quot;;
        ///         AttributeValue rangeKeyValue = new AttributeValue().withS(rangeKey);
        ///         DeletePointRequest deletePointRequest = new DeletePointRequest(geoPoint, rangeKeyValue);
        ///         DeletePointResult deletePointResult = geoIndexManager.deletePoint(deletePointRequest);
        ///     </pre>
        /// </summary>
        /// <param name="deletePointRequest">Container for the necessary parameters to execute delete point request.</param>
        /// <returns>Result of delete point request.</returns>
        Task<DeletePointResult> DeletePointAsync(DeletePointRequest deletePointRequest, CancellationToken cancellationToken = default(CancellationToken));
    }
}
