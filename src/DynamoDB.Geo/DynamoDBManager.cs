using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Geo.Contract;
using DynamoDB.Geo.Contract.Enums;
using DynamoDB.Geo.Contract.Helpers;
using DynamoDB.Geo.Contract.Models;
using Google.Common.Geometry;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDB.Geo
{
    internal class DynamoDBManager
    {
        private readonly IRepositoryClientFactory<IAmazonDynamoDB> _repositoryClientFactory;
        private readonly IOptions<GeoDataClientOptions> _options;

        public DynamoDBManager(IOptions<GeoDataClientOptions> options, IRepositoryClientFactory<IAmazonDynamoDB> clientFactory)
        {
            _options = options;
            _repositoryClientFactory = clientFactory;
        }

        public async Task<GeoQueryResult> DispatchQueries(IEnumerable<GeohashRange> ranges, GeoQueryRequest geoQueryRequest, CancellationToken cancellationToken)
        {
            var geoQueryResult = new GeoQueryResult();


            var futureList = new List<Task>();

            var internalSource = new CancellationTokenSource();
            var internalToken = internalSource.Token;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, internalToken);


            foreach (var outerRange in ranges)
            {
                foreach (var range in outerRange.TrySplit(_options.Value.HashKeyLength))
                {
                    var task = RunGeoQuery(geoQueryRequest, geoQueryResult, range, cts.Token);
                    futureList.Add(task);
                }
            }

            Exception inner = null;
            try
            {
                for (var i = 0; i < futureList.Count; i++)
                {
                    try
                    {
                        await futureList[i].ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        inner = e;
                        // cancel the others
                        internalSource.Cancel(true);
                    }
                }
            }
            catch (Exception ex)
            {
                inner = inner ?? ex;
                throw new GeoClientException("Querying Amazon DynamoDB failed.", inner);
            }

            return geoQueryResult;
        }

        public async Task<UpdatePointResult> UpdatePointAsync(UpdatePointRequest updatePointRequest, DataRegion dataRegion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updatePointRequest == null) throw new ArgumentNullException("updatePointRequest");

            var geohash = S2Utils.GenerateGeohash(updatePointRequest.GeoPoint);
            var hashKey = S2Utils.GenerateHashKey(geohash, _options.Value.HashKeyLength);

            var updateItemRequest = updatePointRequest.UpdateItemRequest;
            updateItemRequest.TableName = _options.Value.TableName;

            var hashKeyValue = new AttributeValue
            {
                N = hashKey.ToString(CultureInfo.InvariantCulture)
            };

            updateItemRequest.Key[_options.Value.HashKeyAttributeName] = hashKeyValue;
            updateItemRequest.Key[_options.Value.RangeKeyAttributeName] = updatePointRequest.RangeKeyValue;

            // Geohash and geoJson cannot be updated.
            updateItemRequest.AttributeUpdates.Remove(_options.Value.GeohashAttributeName);
            updateItemRequest.AttributeUpdates.Remove(_options.Value.GeoJsonAttributeName);
            try
            {
                using (var dynamoDBClient = _repositoryClientFactory.BuildRepositoryClient(dataRegion))
                {
                    UpdateItemResponse updateItemResult = await dynamoDBClient.UpdateItemAsync(updateItemRequest, cancellationToken).ConfigureAwait(false);
                    var updatePointResult = new UpdatePointResult(updateItemResult);

                    return updatePointResult;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<PutPointResult> PutPointAsync(PutPointRequest putPointRequest, DataRegion dataRegion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (putPointRequest == null) throw new ArgumentNullException("putPointRequest");

            var geohash = S2Utils.GenerateGeohash(putPointRequest.GeoPoint);
            var hashKey = S2Utils.GenerateHashKey(geohash, _options.Value.HashKeyLength);
            var geoJson = GeoJsonMapper.StringFromGeoObject(putPointRequest.GeoPoint);

            var putItemRequest = putPointRequest.PutItemRequest;
            putItemRequest.TableName = _options.Value.TableName;

            var hashKeyValue = new AttributeValue
            {
                N = hashKey.ToString(CultureInfo.InvariantCulture)
            };
            putItemRequest.Item[_options.Value.HashKeyAttributeName] = hashKeyValue;
            putItemRequest.Item[_options.Value.RangeKeyAttributeName] = putPointRequest.RangeKeyValue;


            var geohashValue = new AttributeValue
            {
                N = geohash.ToString(CultureInfo.InvariantCulture)
            };

            putItemRequest.Item[_options.Value.GeohashAttributeName] = geohashValue;

            var geoJsonValue = new AttributeValue
            {
                S = geoJson
            };

            putItemRequest.Item[_options.Value.GeoJsonAttributeName] = geoJsonValue;
            try
            {
                using (var dynamoDBClient = _repositoryClientFactory.BuildRepositoryClient(dataRegion))
                {
                    PutItemResponse putItemResult = await dynamoDBClient.PutItemAsync(putItemRequest, cancellationToken).ConfigureAwait(false);
                    var putPointResult = new PutPointResult(putItemResult);

                    return putPointResult;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<DeletePointResult> DeletePointAsync(DeletePointRequest deletePointRequest, DataRegion dataRegion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (deletePointRequest == null) throw new ArgumentNullException("deletePointRequest");

            var geohash = S2Utils.GenerateGeohash(deletePointRequest.GeoPoint);
            var hashKey = S2Utils.GenerateHashKey(geohash, _options.Value.HashKeyLength);

            var deleteItemRequest = deletePointRequest.DeleteItemRequest;

            deleteItemRequest.TableName = _options.Value.TableName;

            var hashKeyValue = new AttributeValue
            {
                N = hashKey.ToString(CultureInfo.InvariantCulture)
            };

            deleteItemRequest.Key[_options.Value.HashKeyAttributeName] = hashKeyValue;
            deleteItemRequest.Key[_options.Value.RangeKeyAttributeName] = deletePointRequest.RangeKeyValue;

            try
            {
                using (var dynamoDBClient = _repositoryClientFactory.BuildRepositoryClient(dataRegion))
                {
                    DeleteItemResponse deleteItemResult = await dynamoDBClient.DeleteItemAsync(deleteItemRequest, cancellationToken).ConfigureAwait(false);
                    var deletePointResult = new DeletePointResult(deleteItemResult);

                    return deletePointResult;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        public async Task<IReadOnlyList<QueryResponse>> QueryGeohashAsync(QueryRequest queryRequest, ulong hashKey, GeohashRange range, DataRegion dataRegion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (queryRequest == null) throw new ArgumentNullException("queryRequest");
            if (range == null) throw new ArgumentNullException("range");

            var queryResults = new List<QueryResponse>();
            IDictionary<string, AttributeValue> lastEvaluatedKey = null;
            do
            {
                var keyConditions = new Dictionary<string, Condition>();

                var hashKeyCondition = new Condition
                {
                    ComparisonOperator = ComparisonOperator.EQ,
                    AttributeValueList = new List<AttributeValue>
                    {
                        new AttributeValue
                        {
                            N = hashKey.ToString(CultureInfo.InvariantCulture)
                        }
                    }
                };

                keyConditions.Add(_options.Value.HashKeyAttributeName, hashKeyCondition);

                var minRange = new AttributeValue
                {
                    N = range.RangeMin.ToString(CultureInfo.InvariantCulture)
                };
                var maxRange = new AttributeValue
                {
                    N = range.RangeMax.ToString(CultureInfo.InvariantCulture)
                };

                var geohashCondition = new Condition
                {
                    ComparisonOperator = ComparisonOperator.BETWEEN,
                    AttributeValueList = new List<AttributeValue>
                    {
                        minRange,
                        maxRange
                    }
                };

                keyConditions.Add(_options.Value.GeohashAttributeName, geohashCondition);

                queryRequest.TableName = _options.Value.TableName;
                queryRequest.KeyConditions = keyConditions;
                queryRequest.IndexName = _options.Value.GeohashIndexName;
                queryRequest.ConsistentRead = true;
                queryRequest.ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL;

                if (lastEvaluatedKey != null && lastEvaluatedKey.Count > 0)
                {
                    queryRequest.ExclusiveStartKey[_options.Value.HashKeyAttributeName] = lastEvaluatedKey[_options.Value.HashKeyAttributeName];
                    queryRequest.ExclusiveStartKey[_options.Value.RangeKeyAttributeName] = lastEvaluatedKey[_options.Value.RangeKeyAttributeName];
                    queryRequest.ExclusiveStartKey[_options.Value.GeohashAttributeName] = lastEvaluatedKey[_options.Value.GeohashAttributeName];
                }

                try
                {
                    using (var dynamoDBClient = _repositoryClientFactory.BuildRepositoryClient(dataRegion))
                    {
                        QueryResponse queryResult = await dynamoDBClient.QueryAsync(queryRequest, cancellationToken).ConfigureAwait(false);
                        queryResults.Add(queryResult);
                        lastEvaluatedKey = queryResult.LastEvaluatedKey;
                    }
                }
                catch (Exception ex)
                { throw ex; }

            } while (lastEvaluatedKey != null && lastEvaluatedKey.Count > 0);

            return queryResults;
        }

        #region Helpers
        private async Task RunGeoQuery(GeoQueryRequest request, GeoQueryResult geoQueryResult, GeohashRange range, CancellationToken cancellationToken)
        {
            var queryRequest = request.QueryRequest.CopyQueryRequest();
            var hashKey = S2Utils.GenerateHashKey(range.RangeMin, _options.Value.HashKeyLength);

            var results = await QueryGeohashAsync(queryRequest, hashKey, range, request.DataRegion, cancellationToken).ConfigureAwait(false);

            foreach (var queryResult in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // This is a concurrent collection
                geoQueryResult.QueryResults.Add(queryResult);

                var filteredQueryResult = Filter(queryResult.Items, request);

                // this is a concurrent collection
                foreach (var r in filteredQueryResult)
                    geoQueryResult.Items.Add(r);
            }
        }

        /// <summary>
        ///     Filter out any points outside of the queried area from the input list.
        /// </summary>
        /// <param name="list">List of items return by Amazon DynamoDB. It may contains points outside of the actual area queried.</param>
        /// <param name="geoQueryRequest">List of items within the queried area.</param>
        /// <returns></returns>
        private IEnumerable<IDictionary<string, AttributeValue>> Filter(IEnumerable<IDictionary<string, AttributeValue>> list,
                                                                        GeoQueryRequest geoQueryRequest)
        {
            var result = new List<IDictionary<String, AttributeValue>>();

            S2LatLngRect? latLngRect = null;
            S2LatLng? centerLatLng = null;
            double radiusInMeter = 0;
            if (geoQueryRequest is QueryRectangleRequest)
            {
                latLngRect = S2Utils.GetBoundingLatLngRect(geoQueryRequest);
            }
            else if (geoQueryRequest is QueryRadiusRequest)
            {
                var centerPoint = ((QueryRadiusRequest)geoQueryRequest).CenterPoint;
                centerLatLng = S2LatLng.FromDegrees(centerPoint.Latitude, centerPoint.Longitude);

                radiusInMeter = ((QueryRadiusRequest)geoQueryRequest).RadiusInMeter;
            }

            foreach (var item in list)
            {
                var geoJson = item[_options.Value.GeoJsonAttributeName].S;
                var geoPoint = GeoJsonMapper.GeoPointFromString(geoJson);

                var latLng = S2LatLng.FromDegrees(geoPoint.Latitude, geoPoint.Longitude);
                if (latLngRect != null && latLngRect.Value.Contains(latLng))
                {
                    result.Add(item);
                }
                else if (centerLatLng != null && radiusInMeter > 0
                         && centerLatLng.Value.GetEarthDistance(latLng) <= radiusInMeter)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        #endregion
    }
}
