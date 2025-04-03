using StackExchange.Redis;

namespace ConsoleCacheRedis
{
    public static class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer>CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheRedisKeys = "";
            return ConnectionMultiplexer.Connect(cacheRedisKeys);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
