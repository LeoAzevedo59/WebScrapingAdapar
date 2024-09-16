using System.Data.SqlClient;
using Adapar.dtos;
using Dapper;

namespace Adapar.useCases
{
  public static class ProductCreate
  {
    public static async Task<Guid> Async(SqlConnection connection, ProductDto product)
    {
      if (string.IsNullOrEmpty(product.product_description))
        return Guid.Empty;

      string insertProductQuery = @"
            INSERT INTO products (
                id, created_at, updated_at, description, code, status, full_category, category,
                unit, class_toxicological, enterprise, classification, flammability, formulation,
                action_form, bulletin_link, deleted_at, synced_at
            ) VALUES (
                @Id,
                dbo.GETUTC(),
                dbo.GETUTC(),
                @Description,
                @Code,
                @Status,
                @FullCategory,
                @Category,
                @Unit,
                @ClassToxicological,
                @Enterprise,
                @Classification,
                @Flammability,
                @Formulation,
                @ActionForm,
                @BulletinLink,
                NULL,
                dbo.GETUTC()
            )";

      // Função para truncar os valores conforme o limite máximo permitido
      string Truncate(string value, int maxLength) =>
          string.IsNullOrEmpty(value) ? null : value.Length <= maxLength ? value : value.Substring(0, maxLength);

      Guid id = Guid.NewGuid();

      var parameters = new
      {
        Id = id,
        Description = Truncate(product.product_description, 64),
        Code = Truncate(product.register_number, 16),
        Status = Truncate(product.status, 94),
        FullCategory = Truncate(product.product_class, 204),
        Category = ConvertCategory(product.product_class),
        Unit = ConvertUnit(product.active_ingredients),
        ClassToxicological = Truncate(product.category, 32),
        Enterprise = Truncate(product.registering_company, 104),
        Classification = Truncate(product.classification, 16),
        Flammability = Truncate(product.flammability, 32),
        Formulation = Truncate(product.formulation, 48),
        ActionForm = Truncate(product.action_form, 48),
        BulletinLink = Truncate(product.bulletin_link, 164)
      };

      // Usando Dapper para executar a inserção
      await connection.ExecuteAsync(insertProductQuery, parameters);

      Console.WriteLine($"{product.product_description} - CREATE");
      return id;
    }

    static string? ConvertUnit(List<ActiveIngredientDto>? ingredients)
    {
      if (ingredients is null || ingredients.Count == 0)
        return "477b6f6a-f280-4423-97b7-722537762512";

      if (string.IsNullOrEmpty(ingredients.First().concentration))
        return "477b6f6a-f280-4423-97b7-722537762512";

      string unitAdapar = ingredients.First().concentration.Trim().ToUpper();

      if (unitAdapar.Contains("KG"))
        return "2b98e37f-c7ba-4dbf-9868-1c9208435147";
      else if (unitAdapar.Trim().ToUpper().Contains("L"))
        return "477b6f6a-f280-4423-97b7-722537762512";
      else if (unitAdapar.Trim().ToUpper().Contains("%")) // LITROS
        return "477b6f6a-f280-4423-97b7-722537762512";

      return "477b6f6a-f280-4423-97b7-722537762512";
    }

    static string? ConvertCategory(string? category)
    {
      if (string.IsNullOrWhiteSpace(category))
        return "0f9add9e-c03b-497c-9125-078cd8099eb5";

      string categoryAdapar = category.Trim().ToLower();

      if (categoryAdapar.Contains("acaricida"))
        return "27d6a215-fd38-4eaf-8240-3c932fd59666";
      else if (categoryAdapar.Contains("fungicida"))
        return "b1c144b4-796b-4763-9a56-dbf9250902c1";
      else if (categoryAdapar.Contains("fungicida , herbicida"))
        return "6b448b09-eb33-4350-9f06-8c336b52b78e"; // herbicida
      else if (categoryAdapar.Contains("herbicida"))
        return "6b448b09-eb33-4350-9f06-8c336b52b78e";
      else if (categoryAdapar.Contains("inseticida"))
        return "3a2b9820-584a-403d-87ee-6ffa5c7e681b";
      else if (categoryAdapar.Contains("formicida"))
        return "3a2b9820-584a-403d-87ee-6ffa5c7e681b"; // inseticida
      else if (categoryAdapar.Contains("lesmicida"))
        return "92ed65b0-c572-4863-9650-d8756a5aad4d";
      else if (categoryAdapar.Contains("nematicida"))
        return "8b5c9053-44ed-40e2-9bb7-081521dad3fc";
      else
        return "0f9add9e-c03b-497c-9125-078cd8099eb5";
    }
  }
}
