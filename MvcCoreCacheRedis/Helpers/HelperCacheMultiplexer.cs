using StackExchange.Redis;

namespace MvcCoreCacheRedis.Helpers
{
    public static class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer> CreateConnection;

        public static void Initialize(string cacheRedisKeys)
        {
            CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(cacheRedisKeys);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
