using System;

namespace DynamoDB.Geo
{
    public class GeoClientException : Exception
    {
        internal GeoClientException(string message) : base(message)
        {
        }

        internal GeoClientException(string message, Exception e) : base(message, e)
        {
        }
    }
}
