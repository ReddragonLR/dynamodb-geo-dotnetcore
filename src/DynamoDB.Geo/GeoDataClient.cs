using Amazon.DynamoDBv2;
using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Helpers;
using DynamoDB.Geo.Contract.Models;
using Google.Common.Geometry;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDB.Geo
{
    public class GeoDataClient : IGeoDataClient
    {
        private readonly DynamoDBManager _manager;
        public GeoDataClient(IOptions<GeoDataClientOptions> options, IRepositoryClientFactory<IAmazonDynamoDB> clientFactory)
        {
            _manager = new DynamoDBManager(options, clientFactory);
        }

        public async Task<QueryRadiusResult> QueryRadiusAsync(QueryRadiusRequest queryRadiusRequest, CancellationToken cancellationToken = default)
        {
            if (queryRadiusRequest == null) throw new ArgumentNullException("queryRadiusRequest");
            if (queryRadiusRequest.RadiusInMeter <= 0 || queryRadiusRequest.RadiusInMeter > S2LatLng.EarthRadiusMeters)
                throw new ArgumentOutOfRangeException("queryRadiusRequest", "RadiusInMeter needs to be > 0  and <= " + S2LatLng.EarthRadiusMeters);

            var latLngRect = S2Utils.GetBoundingLatLngRect(queryRadiusRequest);

            var cellUnion = S2Utils.FindCellIds(latLngRect);

            var ranges = S2Utils.MergeCells(cellUnion);

            var result = await _manager.DispatchQueries(ranges, queryRadiusRequest, cancellationToken).ConfigureAwait(false);
            return new QueryRadiusResult(result);
        }

        public Task<PutPointResult> PutPointAsync(PutPointRequest putPointRequest, CancellationToken cancellationToken = default)
        {
            return _manager.PutPointAsync(putPointRequest, putPointRequest.DataRegion, cancellationToken);
        }

        public Task<UpdatePointResult> UpdatePointAsync(UpdatePointRequest updatePointRequest, CancellationToken cancellationToken = default)
        {
            return _manager.UpdatePointAsync(updatePointRequest, updatePointRequest.DataRegion, cancellationToken);
        }

        public Task<DeletePointResult> DeletePointAsync(DeletePointRequest deletePointRequest, CancellationToken cancellationToken = default)
        {
            return _manager.DeletePointAsync(deletePointRequest, deletePointRequest.DataRegion, cancellationToken);
        }
    }
}
