using Dapper;
using System.Data.SqlClient;
using Adapar.dtos;

namespace Adapar.useCases.Composition
{
    public class CompositionCreate
    {
        public static async Task Async(SqlConnection connection, Guid productId, ActiveIngredientDto ingredient)
        {
            string insertCompositionQuery = @"
            INSERT INTO product_compositions (
                id,
                created_at,
                updated_at,
                product,
                concentration,
                ingredient,
                deleted_at,
                synced_at
            ) VALUES (
                NEWID(),
                dbo.getUtc(),
                dbo.getUtc(),
                @Product,
                @Concentration,
                @Ingredient,
                NULL,
                dbo.getUtc()
            )";

            // Função para truncar os valores conforme o limite máximo permitido
            string Truncate(string value, int maxLength) =>
                string.IsNullOrEmpty(value) ? null : value.Length <= maxLength ? value : value.Substring(0, maxLength);

            var parameters = new
            {
                Product = productId,
                Concentration = Truncate(ingredient.concentration, 64),
                Ingredient = Truncate(ingredient.ingredient_description, 116)
            };

            // Usando Dapper para executar a inserção
            await connection.ExecuteAsync(insertCompositionQuery, parameters);
        }
    }
}
