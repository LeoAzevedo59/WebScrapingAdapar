using Dapper;
using System.Data.SqlClient;
using Adapar.sqls;

namespace Adapar.useCases.Products
{
    public class ProductsGet
    {
        public static async Task<List<ProductSql>> Async(SqlConnection connection)
        {
            string sql = @"
            SELECT 
                p.id,
                p.created_at,
                p.updated_at,
                p.description,
                p.code,
                p.status,
                p.full_category as fullCategory,
                p.category AS category_id,
                pc.description AS category,
                p.unit AS unit_id,
                pu.description AS unit,
                p.class_toxicological as classToxicological,
                p.enterprise,
                p.classification,
                p.flammability,
                p.formulation,
                p.action_form as actionForm,
                p.bulletin_link as bulletinLink,
                p.deleted_at,
                p.synced_at
            FROM 
                products p
            LEFT JOIN 
                product_categories pc ON p.category = pc.id
            LEFT JOIN 
                product_units pu ON p.unit = pu.id";

            // Usando Dapper para consultar os dados
            var products = (await connection.QueryAsync<ProductSql>(sql)).ToList();

            return products;
        }
    }
}
