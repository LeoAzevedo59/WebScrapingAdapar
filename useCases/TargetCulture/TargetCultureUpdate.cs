using System.Data.SqlClient;
using Adapar.dtos;
using Adapar.useCases.Cache;
using Dapper;

namespace Adapar.useCases.TargetCulture
{
    public class TargetCultureUpdate
    {

        public static async Task Async(SqlConnection connection,
            List<TargetCultureDto> targetCultures,
            Guid id)
        {
            bool isUpdate = false;

            foreach (var targetCultureNew in targetCultures)
            {
                string queryUpdate = "updated_at = dbo.GetUtc(), synced_at = dbo.GetUtc()";
                var targetCultureSql = CacheManager.targetCulturesSql
                    .FirstOrDefault(target => target.Culture == targetCultureNew.culture &&
                    target.Target == targetCultureNew.target && target.Product == id);

                if (targetCultureSql is null)
                {
                    await TargetCultureCreate.Async(connection, id, targetCultureNew);
                    continue;
                }

                if (targetCultureNew.status_culture is not null && !targetCultureNew.status_culture
                    .Equals(targetCultureSql.StatusCulture))
                {
                    isUpdate = true;
                    queryUpdate += ",status_culture = @status_culture";
                }
                if (targetCultureNew.status_target is not null && !targetCultureNew.status_target
                   .Equals(targetCultureSql.StatusTarget))
                {
                    isUpdate = true;
                    queryUpdate += ",status_target = @status_target";
                }
                if (targetCultureNew.detail_link is not null && !targetCultureNew.detail_link
                .Equals(targetCultureSql.DetailLink))
                {
                    isUpdate = true;
                    queryUpdate += ",detail_link = @detail_link";
                }
                if (targetCultureNew.common_name is not null && !targetCultureNew.common_name
                           .Equals(targetCultureSql.CommonName))
                {
                    isUpdate = true;
                    queryUpdate += ",common_name = @common_name";
                }
                if (targetCultureNew.dosage is not null && !targetCultureNew.dosage
                       .Equals(targetCultureSql.Dosage))
                {
                    isUpdate = true;
                    queryUpdate += ",dosage = @dosage";
                }
                if (targetCultureNew.safety_range is not null && !targetCultureNew.safety_range
                      .Equals(targetCultureSql.SafetyRange))
                {
                    isUpdate = true;
                    queryUpdate += ",safety_range = @safety_range";
                }

                if (targetCultureNew.observation is not null && !targetCultureNew.observation
                                      .Equals(targetCultureSql.Observation))
                {
                    isUpdate = true;
                    queryUpdate += ",observation = @observation";
                }

                if (isUpdate)
                {
                    string query = @$"UPDATE product_target_culture 
                    SET {queryUpdate} WHERE id = '{targetCultureSql.Id}'";

                    var parameters = new
                    {

                        status_culture = Truncate(targetCultureNew.status_culture, 32),
                        status_target = Truncate(targetCultureNew.status_target, 16),
                        detail_link = Truncate(targetCultureNew.detail_link, 64),
                        common_name = Truncate(targetCultureNew.common_name, 212),
                        dosage = Truncate(targetCultureNew.dosage, 120),
                        safety_range = Truncate(targetCultureNew.safety_range, 212),
                        observation = Truncate(targetCultureNew.observation, 500)
                    };

                    await connection.ExecuteAsync(query, parameters);
                    Console.WriteLine($"target update");
                }
            }
            string Truncate(string? value, int maxLength) =>
                    string.IsNullOrEmpty(value) ? null : value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
}