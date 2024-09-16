using System.Data.SqlClient;
using Adapar.dtos;
using Adapar.useCases.Cache;
using Dapper;

namespace Adapar.useCases.Composition
{
  public class CompositionUpdate
  {
    public static async Task Async(SqlConnection connection,
        List<ActiveIngredientDto> ingredientList,
        Guid id)
    {

      bool isUpdate = false;

      foreach (var ingredientNew in ingredientList)
      {
        string queryUpdate = "updated_at = dbo.GetUtc(), synced_at = dbo.GetUtc()";
        var ingredientListSql = CacheManager.compositionsSql
            .FirstOrDefault(composition => composition.Ingredient
            .Equals(ingredientNew.ingredient_description) && composition.Product == id);

        if (ingredientListSql is null)
        {
          await CompositionCreate.Async(connection, id, ingredientNew);
          continue; // created
        }

        if (ingredientNew.concentration is not null && !ingredientNew.concentration
             .Equals(ingredientListSql.Concentration))
        {
          isUpdate = true;
          queryUpdate += ",concentration = @concentration";
        }

        if (ingredientNew.ingredient_description is not null && !ingredientNew.ingredient_description
           .Equals(ingredientListSql.Ingredient))
        {
          isUpdate = true;
          queryUpdate += ",ingredient = @ingredient";
        }

        if (isUpdate)
        {
          string query = @$"UPDATE product_compositions
                    SET {queryUpdate} WHERE id = '{ingredientListSql.Id}'";

          var parameters = new
          {
            concentration = Truncate(ingredientNew.concentration, 64),
            ingredient_description = Truncate(ingredientNew.ingredient_description, 116)
          };

          await connection.ExecuteAsync(query, parameters);
          Console.WriteLine($"ingredient UPDATE");
        }
      }
      string Truncate(string? value, int maxLength) =>
                      string.IsNullOrEmpty(value) ? null : value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

  }
}