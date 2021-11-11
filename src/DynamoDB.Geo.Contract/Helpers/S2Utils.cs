using DynamoDB.Geo.Contract.Models;
using Google.Common.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace DynamoDB.Geo.Contract.Helpers
{
    public static class S2Utils
    {
        /// <summary>
        /// An utility method to get a bounding box of latitude and longitude from a given GeoQueryRequest.
        /// </summary>
        /// <param name="geoQueryRequest">It contains all of the necessary information to form a latitude and longitude box.</param>
        /// <returns></returns>
        public static S2LatLngRect GetBoundingLatLngRect(GeoQueryRequest geoQueryRequest)
        {
            if (geoQueryRequest is QueryRectangleRequest)
            {
                var queryRectangleRequest = (QueryRectangleRequest)geoQueryRequest;

                var minPoint = queryRectangleRequest.MinPoint;
                var maxPoint = queryRectangleRequest.MaxPoint;

                var latLngRect = default(S2LatLngRect);

                if (minPoint != null && maxPoint != null)
                {
                    var minLatLng = S2LatLng.FromDegrees(minPoint.Latitude, minPoint.Longitude);
                    var maxLatLng = S2LatLng.FromDegrees(maxPoint.Latitude, maxPoint.Longitude);

                    latLngRect = new S2LatLngRect(minLatLng, maxLatLng);
                }

                return latLngRect;
            }
            else if (geoQueryRequest is QueryRadiusRequest)
            {
                var queryRadiusRequest = (QueryRadiusRequest)geoQueryRequest;

                var centerPoint = queryRadiusRequest.CenterPoint;
                var radiusInMeter = queryRadiusRequest.RadiusInMeter;

                var centerLatLng = S2LatLng.FromDegrees(centerPoint.Latitude, centerPoint.Longitude);

                var latReferenceUnit = centerPoint.Latitude > 0.0 ? -1.0 : 1.0;
                var latReferenceLatLng = S2LatLng.FromDegrees(centerPoint.Latitude + latReferenceUnit,
                                                              centerPoint.Longitude);
                var lngReferenceUnit = centerPoint.Longitude > 0.0 ? -1.0 : 1.0;
                var lngReferenceLatLng = S2LatLng.FromDegrees(centerPoint.Latitude, centerPoint.Longitude
                                                                                    + lngReferenceUnit);

                var latForRadius = radiusInMeter / centerLatLng.GetEarthDistance(latReferenceLatLng);
                var lngForRadius = radiusInMeter / centerLatLng.GetEarthDistance(lngReferenceLatLng);

                var minLatLng = S2LatLng.FromDegrees(centerPoint.Latitude - latForRadius,
                                                     centerPoint.Longitude - lngForRadius);
                var maxLatLng = S2LatLng.FromDegrees(centerPoint.Latitude + latForRadius,
                                                     centerPoint.Longitude + lngForRadius);

                return new S2LatLngRect(minLatLng, maxLatLng);
            }

            return S2LatLngRect.Empty;
        }

        public static S2CellUnion FindCellIds(S2LatLngRect latLngRect)
        {
            var queue = new ConcurrentQueue<S2CellId>();

            var cellIds = new List<S2CellId>();

            for (var c = S2CellId.Begin(0); !c.Equals(S2CellId.End(0)); c = c.Next)
            {
                if (ContainsGeodataToFind(c, latLngRect))
                {
                    queue.Enqueue(c);
                }
            }

            ProcessQueue(queue, cellIds, latLngRect);
            Debug.Assert(queue.Count == 0);

            queue = null;

            if (cellIds.Count > 0)
            {
                var cellUnion = new S2CellUnion();
                cellUnion.InitFromCellIds(cellIds); // This normalize the cells.
                // cellUnion.initRawCellIds(cellIds); // This does not normalize the cells.
                cellIds = null;

                return cellUnion;
            }

            return null;
        }

        /// <summary>
        ///     Merge continuous cells in cellUnion and return a list of merged GeohashRanges.
        /// </summary>
        /// <param name="cellUnion">Container for multiple cells.</param>
        /// <returns>A list of merged GeohashRanges.</returns>
        public static List<GeohashRange> MergeCells(S2CellUnion cellUnion)
        {
            var ranges = new List<GeohashRange>();
            foreach (var c in cellUnion.CellIds)
            {
                var range = new GeohashRange(c.RangeMin.Id, c.RangeMax.Id);

                var wasMerged = false;
                foreach (var r in ranges)
                {
                    if (r.TryMerge(range))
                    {
                        wasMerged = true;
                        break;
                    }
                }

                if (!wasMerged)
                {
                    ranges.Add(range);
                }
            }

            return ranges;
        }

        private static bool ContainsGeodataToFind(S2CellId c, S2LatLngRect latLngRect)
        {
            return latLngRect.Intersects(new S2Cell(c));
        }

        private static void ProcessQueue(ConcurrentQueue<S2CellId> queue, List<S2CellId> cellIds,
                                         S2LatLngRect latLngRect)
        {
            S2CellId cell;
            while (queue.TryDequeue(out cell))
            {
                if (!cell.IsValid)
                {
                    break;
                }

                ProcessChildren(cell, latLngRect, queue, cellIds);
            }
        }

        private static void ProcessChildren(S2CellId parent, S2LatLngRect latLngRect,
                                            ConcurrentQueue<S2CellId> queue, List<S2CellId> cellIds)
        {
            var children = new List<S2CellId>(4);

            for (var c = parent.ChildBegin; !c.Equals(parent.ChildEnd); c = c.Next)
            {
                if (ContainsGeodataToFind(c, latLngRect))
                {
                    children.Add(c);
                }
            }

            /*
		 * TODO: Need to update the strategy!
		 * 
		 * Current strategy:
		 * 1 or 2 cells contain cellIdToFind: Traverse the children of the cell.
		 * 3 cells contain cellIdToFind: Add 3 cells for result.
		 * 4 cells contain cellIdToFind: Add the parent for result.
		 * 
		 * ** All non-leaf cells contain 4 child cells.
		 */
            switch (children.Count)
            {
                case 1:
                case 2:
                    {
                        foreach (var child in children)
                        {
                            if (child.IsLeaf)
                            {
                                cellIds.Add(child);
                            }
                            else
                            {
                                queue.Enqueue(child);
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        cellIds.AddRange(children);
                    }
                    break;
                case 4:
                    {
                        cellIds.Add(parent);
                    }
                    break;
                default:
                    {
                        Debug.Assert(false); // This should not happen.
                    }
                    break;
            }
        }

        public static ulong GenerateGeohash(GeoPoint geoPoint)
        {
            var latLng = S2LatLng.FromDegrees(geoPoint.Latitude, geoPoint.Longitude);
            var cell = new S2Cell(latLng);
            var cellId = cell.Id;

            return cellId.Id;
        }

        public static ulong GenerateHashKey(ulong geohash, int hashKeyLength)
        {
            var geohashString = geohash.ToString(CultureInfo.InvariantCulture);
            var denominator = (ulong)Math.Pow(10, geohashString.Length - hashKeyLength);
            return geohash / denominator;
        }
    }
}
