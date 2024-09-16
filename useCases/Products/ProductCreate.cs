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

            Console.WriteLine($"{product.product_description} - OK");
            return id;
        }

        static string? ConvertUnit(List<ActiveIngredientDto>? ingredients)
        {
            if (ingredients is null || ingredients.Count == 0)
                return "625e5253-43cf-49c8-8850-3b2672bb8039";

            if (string.IsNullOrEmpty(ingredients.First().concentration))
                return "625e5253-43cf-49c8-8850-3b2672bb8039";

            string unitAdapar = ingredients.First().concentration.Trim().ToUpper();

            if (unitAdapar.Contains("KG"))
                return "b1ccfcb3-db5b-4c3b-ac12-aff4f5591305";
            else if (unitAdapar.Trim().ToUpper().Contains("L"))
                return "17ad4f7e-be8d-43be-b67f-0b6bbc69327f";
            else if (unitAdapar.Trim().ToUpper().Contains("%")) // LITROS
                return "17ad4f7e-be8d-43be-b67f-0b6bbc69327f";

            return "625e5253-43cf-49c8-8850-3b2672bb8039";
        }

        static string? ConvertCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "34344133-3957-4d59-a584-7fdca1bbb092";

            string categoryAdapar = category.Trim().ToLower();

            if (categoryAdapar.Contains("acaricida"))
                return "45f068eb-54cc-45af-88c1-0cf598e48e54";
            else if (categoryAdapar.Contains("fungicida"))
                return "da22f615-8160-4041-9bc6-4b83a352ad19";
            else if (categoryAdapar.Contains("fungicida , herbicida"))
                return "47770bf5-eadb-459b-bc48-e2de0986eabe"; // herbicida
            else if (categoryAdapar.Contains("herbicida"))
                return "47770bf5-eadb-459b-bc48-e2de0986eabe";
            else if (categoryAdapar.Contains("inseticida"))
                return "036a667a-2a7b-4128-80ca-425e594e6bdc";
            else if (categoryAdapar.Contains("formicida"))
                return "036a667a-2a7b-4128-80ca-425e594e6bdc"; // inseticida
            else if (categoryAdapar.Contains("lesmicida"))
                return "d4a07307-ba99-490e-b1bb-0c20b14a5318";
            else if (categoryAdapar.Contains("nematicida"))
                return "73e435e5-39c0-4a6b-a852-714b0a0658d4";
            else
                return "34344133-3957-4d59-a584-7fdca1bbb092";
        }
    }
}
