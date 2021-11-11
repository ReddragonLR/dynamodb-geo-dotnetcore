using DynamoDB.Geo.Contract.Enums;

namespace DynamoDB.Geo.Contract
{
    public interface IRepositoryClientFactory<T>
        where T : class
    {
        T BuildRepositoryClient(DataRegion dataRegion);
    }
}
