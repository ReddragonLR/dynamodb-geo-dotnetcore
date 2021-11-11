using System.ComponentModel;

namespace DynamoDB.Geo.Contract.Enums
{
    public enum DataRegion
    {
        [Description("UK")] EU_WEST_1 = 1,
        [Description("SouthAfrica")] AF_SOUTH_1 = 2,
        [Description("Europe")] EU_CENTRAL_1 = 3,
        [Description("USA")] US_WEST_1 = 4,
        [Description("Canada")] CA_CENTRAL_1 = 5,
        [Description("Australia")] AP_SOUTHEAST_2 = 6,
        [Description("India")] AP_SOUTH_1 = 7,
        [Description("Asia")] AP_SOUTHEAST_1 = 8,
    }
}
