using Dapper;
using System.Data.SqlClient;
using Adapar.dtos;

namespace Adapar.useCases.TargetCulture
{
    public class TargetCultureCreate
    {
        public static async Task Async(SqlConnection connection, Guid productId, TargetCultureDto targetCulture)
        {
            string query = @"
            INSERT INTO product_target_culture (
                id, created_at, updated_at, product, culture, status_culture, target,
                status_target, detail_link, scientific_name, common_name, dosage,
                safety_range, observation, deleted_at, synced_at
            ) VALUES (
                NEWID(),
                dbo.GetUtc(),
                dbo.GetUtc(),
                @Product,
                @Culture,
                @StatusCulture,
                @Target,
                @StatusTarget,
                @DetailLink,
                @ScientificName,
                @CommonName,
                @Dosage,
                @SafetyRange,
                @Observation,
                NULL,
                dbo.GetUtc()
            );";

            // Função para cortar as strings para o tamanho máximo permitido
            string Truncate(string value, int maxLength) =>
                string.IsNullOrEmpty(value) ? null : value.Length <= maxLength ? value : value.Substring(0, maxLength);

            var parameters = new
            {
                Product = productId,
                Culture = Truncate(targetCulture.culture, 64),
                StatusCulture = Truncate(targetCulture.status_culture, 32),
                Target = Truncate(targetCulture.target, 94),
                StatusTarget = Truncate(targetCulture.status_target, 16),
                DetailLink = Truncate(targetCulture.detail_link, 64),
                ScientificName = Truncate(targetCulture.scientific_name, 72),
                CommonName = Truncate(targetCulture.common_name, 212),
                Dosage = Truncate(targetCulture.dosage, 120),
                SafetyRange = Truncate(targetCulture.safety_range, 212),
                Observation = Truncate(targetCulture.observation, 500)
            };

            // Usando Dapper para executar a inserção
            await connection.ExecuteAsync(query, parameters);
        }
    }
}
