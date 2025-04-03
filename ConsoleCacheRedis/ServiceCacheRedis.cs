using Newtonsoft.Json;
using StackExchange.Redis;

namespace ConsoleCacheRedis
{
    public class ServiceCacheRedis
    {
        private IDatabase database;

        public ServiceCacheRedis()
        {
            this.database = HelperCacheMultiplexer.Connection.GetDatabase();
        }

        //METODO PARA ALMACENAR PRODUCTOS
        //LAS CLAVES DEBEN SER UNICAS POR USUARIO
        public async Task AddProductoFavoritoAsync(Producto producto)
        {
            //REDIS TRABAJA CON KEYS/VALUES PARA RECUPERAR LOS ELEMENTOS O ALMACENAR LOS ELEMENTOS EN FORMATO JSON
            //ALMACENAREMOS EN REDIS UN CONJUNTO DE PRODUCTOS FAVORITOS
            string jsonProductos = await this.database.StringGetAsync("favoritos");
            List<Producto> productosList;
            if (jsonProductos == null)
            {
                productosList = new List<Producto>();
            }
            else
            {
                //RECUPERAMOS LA COLECCION DE CACHE REDIS
                productosList = JsonConvert.DeserializeObject<List<Producto>>(jsonProductos);
            }

            //AGREGAMOS EL PRODUCTO NUEVO
            productosList.Add(producto);
            //CON EL PRODUCTO AGREGADO, VOLVEMOS A GENERAR EL JSON DE PRODUCTOS
            jsonProductos = JsonConvert.SerializeObject(productosList);
            //ALMACENAMOS LOS DATOS DE NUEVO EN CACHE REDIS
            await this.database.StringSetAsync("favoritos", jsonProductos);
        }

        //REALIZAMOS UN METODO PARA RECUPERAR LOS PRODUCTOS FAVORITOS
        public async Task<List<Producto>> GetProductosFavoritosAsync()
        {
            string jsonProductos = await this.database.StringGetAsync("favoritos");
            if (jsonProductos == null)
            {
                return null;
            }
            else
            {
                List<Producto> productos = JsonConvert.DeserializeObject<List<Producto>>(jsonProductos);
                return productos;
            }
        }

        public async Task DeleteProductoFavoritoAsync(int idProducto)
        {
            //ESTO NO ES UNA BASE DE DATOS, NO PODEMOS FILTRAR NI BUSCAR, SOLAMENTE EXTRAER
            List<Producto> favoritos = await this.GetProductosFavoritosAsync();
            //SI EXISTEN FAVORITOS, ELIMINAMOS
            if (favoritos != null)
            {
                //BUSCAMOS EL PRODUCTO A ELIMNAR
                Producto productoDelete = favoritos.FirstOrDefault(x => x.IdProducto == idProducto);
                //ELIMINAMOS EL PRODUCTO DE LA COLECCION
                favoritos.Remove(productoDelete);
                //SI ELIMINAMOS TODOS LOS PRODUCTOS, DEBEMOS ELIMINAR CACHE REDIS
                if (favoritos.Count == 0)
                {
                    await this.database.KeyDeleteAsync("favoritos");
                }
                else
                {
                    //ALMACENAMOS LOS PRODUCTOS FAVORITOS DE NUEVO
                    string jsonProductos = JsonConvert.SerializeObject(favoritos);
                    //VOLVEMOS A GUARDAR NUESTRO CACHE REDIS
                    //VAMOS A DARLE TIEMPO AL GUARDAR UN ELEMENTO EN REDIS
                    //SI NO LE DECIMOS NADA, POR DEFECTO REDIS ALMACENA LOS DATOS 24 HORAS
                    await this.database.StringSetAsync("favoritos", jsonProductos, TimeSpan.FromMinutes(30));
                }
            }
        }
    }
}
