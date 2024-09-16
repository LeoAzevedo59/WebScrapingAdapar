using Dapper;
using System.Data.SqlClient;
using Adapar.sqls;

namespace Adapar.useCases.Composition
{
    public class CompositionGet
    {
        public static async Task<List<ProductCompositionSql>> Async(SqlConnection connection)
        {
            string sql = @"
            SELECT 
                id,
                created_at,
                updated_at,
                product,
                concentration,
                ingredient,
                deleted_at,
                synced_at
            FROM 
                product_compositions";

            // Usando Dapper para consultar os dados
            var productCompositions = (await connection.QueryAsync<ProductCompositionSql>(sql)).ToList();

            return productCompositions;
        }
    }
}
