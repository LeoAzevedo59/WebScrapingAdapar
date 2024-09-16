using Dapper;
using System.Data.SqlClient;
using Adapar.sqls;

namespace Adapar.useCases.TargetCulture
{
    public class TargetCultureGet
    {
        public static async Task<List<ProductTargetCultureSql>> Async(SqlConnection connection)
        {
            string sql = @"
            SELECT
                id,
                created_at,
                updated_at,
                product,
                culture,
                status_culture as statusCulture,
                target,
                status_target as statusTarget,
                detail_link as detailLink,
                scientific_name as scientificName,
                common_name as commonName,
                dosage,
                safety_range as safetyRange,
                observation,
                deleted_at,
                synced_at
            FROM 
                product_target_culture";

            // Usando Dapper para consultar os dados
            var productTargetCultures = (await connection.QueryAsync<ProductTargetCultureSql>(sql)).ToList();

            return productTargetCultures;
        }
    }
}