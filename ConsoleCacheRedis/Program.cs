using ConsoleCacheRedis;

Console.WriteLine("Testing Cache Redis");

ServiceCacheRedis service = new ServiceCacheRedis();
List<Producto> favoritos = await service.GetProductosFavoritosAsync();

if (favoritos == null)
{
    Console.WriteLine("No tenemos favoritos");
}
else
{
    int i = 1;
    foreach (Producto p in favoritos)
    {
        Console.WriteLine(i + " - " + p.Nombre);
        i += 1;
    }
}
Console.ReadLine();