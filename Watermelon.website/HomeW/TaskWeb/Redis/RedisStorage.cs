using StackExchange.Redis;
namespace Watermelon.website.Redis;

public static class RedisStorage
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new (
        () => ConnectionMultiplexer.Connect(new ConfigurationOptions 
        {
            EndPoints =
            {
                "localhost:6379"
            }
        }));
    public static ConnectionMultiplexer Connection => LazyConnection.Value;
    public static IDatabase RedisCache => Connection.GetDatabase();
}