using System.Data.SqlClient;
using Adapar.sqls;
using Adapar.useCases.Composition;
using Adapar.useCases.Products;
using Adapar.useCases.TargetCulture;

namespace Adapar.useCases.Cache
{
    public class CacheManager
    {
        public static List<ProductSql> productsSql = new();
        public static List<ProductCompositionSql> compositionsSql = new();
        public static List<ProductTargetCultureSql> targetCulturesSql = new();

        public static async Task Init(SqlConnection connection)
        {
            var productsTask = ProductsGet.Async(connection);
            var compositionsTask = CompositionGet.Async(connection);
            var targetCulturesTask = TargetCultureGet.Async(connection);

            // Aguarde todas as tarefas serem concluídas
            await Task.WhenAll(productsTask, compositionsTask, targetCulturesTask);

            // Obtenha os resultados das tarefas
            productsSql = await productsTask;
            compositionsSql = await compositionsTask;
            targetCulturesSql = await targetCulturesTask;
        }
    }
}