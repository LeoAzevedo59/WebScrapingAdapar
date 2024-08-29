using Newtonsoft.Json;
using System.Data.SqlClient;

string? connectionString = Environment.GetEnvironmentVariable("SQL_SERVER");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new ArgumentException("Connection string not found.");

string jsonFilePath = "agrochemicals.json";
string jsonContent = File.ReadAllText(jsonFilePath);
var products = JsonConvert.DeserializeObject<List<Product>>(jsonContent);

using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    foreach (var product in products)
    {

        Guid productId;

        if (ExistProduct(connection, product.register_number, product.product_description))
        {
            UpdateProduct(connection, product);
            continue;
        }

        productId = InsertProduct(connection, product);

        if (productId == Guid.Empty)
            continue;

        foreach (var ingredient in product.active_ingredients)
        {
            InsertComposition(connection, productId, ingredient);
        }
    }
}

Console.WriteLine("Dados inseridos com sucesso.");


static void UpdateProduct(SqlConnection connection, Product product)
{
    string query = @$"
        UPDATE products
        SET 
            created_at = @created_at,
            updated_at = dbo.GetUtc(),
            category = @category,
            unit = @unit,
            synced_at = dbo.GetUtc(),
            class_toxicological = @class_toxicological,
            enterprise = @enterprise,
            class_toxicological = @class_toxicological,
            flammability = @flammability,
            formulation = @formulation,
            action_form = @action_form,
            bulletin_link = @bulletin_link
        WHERE code = @code, description = @description";

    using (SqlCommand command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@description", product.product_description);
        command.Parameters.AddWithValue("@code", product.register_number);
        command.Parameters.AddWithValue("@status", product.status);
        command.Parameters.AddWithValue("@category", product.product_class);
        command.Parameters.AddWithValue("@classToxicological", product.category);
        command.Parameters.AddWithValue("@enterprise", product.company);
        command.Parameters.AddWithValue("@classification", product.classification);
        command.Parameters.AddWithValue("@flammability", product.flammability);
        command.Parameters.AddWithValue("@formulation", product.formulation);
        command.Parameters.AddWithValue("@action_form", product.formulation);
        command.Parameters.AddWithValue("@bulletin_link", product.bulletin_link);
    }
}

static Guid InsertProduct(SqlConnection connection, Product product)
{
    if (string.IsNullOrEmpty(product.product_description))
        return Guid.Empty;

    string insertProductQuery = @"
            INSERT INTO products_adapar (id, description, code, status, category, classToxicological, enterprise, classification, flammability, formulation, action_form, bulletin_link)
            VALUES (@id, @description, @code, @status, @category, @classToxicological, @enterprise, @classification, @flammability, @formulation, @action_form, @bulletin_link)";

    Guid id = Guid.NewGuid();

    using (SqlCommand command = new SqlCommand(insertProductQuery, connection))
    {
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@description", product.product_description);
        command.Parameters.AddWithValue("@code", product.register_number);
        command.Parameters.AddWithValue("@status", product.status);
        command.Parameters.AddWithValue("@category", product.product_class);
        command.Parameters.AddWithValue("@classToxicological", product.category);
        command.Parameters.AddWithValue("@enterprise", product.company);
        command.Parameters.AddWithValue("@classification", product.classification);
        command.Parameters.AddWithValue("@flammability", product.flammability);
        command.Parameters.AddWithValue("@formulation", product.formulation);
        command.Parameters.AddWithValue("@action_form", product.formulation);
        command.Parameters.AddWithValue("@bulletin_link", product.bulletin_link);

        return id;
    }
}

static bool ExistProduct(SqlConnection connection, string? register_number, string product_description)
{
    string query = @$"
        select count(*) from products
        where code = @code and description = @description";

    using (SqlCommand command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@code", register_number);
        command.Parameters.AddWithValue("@description", product_description);

        int count = (int)command.ExecuteScalar();
        return count > 0;
    }
}

static void InsertComposition(SqlConnection connection, Guid productId, ActiveIngredient ingredient)
{
    string insertCompositionQuery = @"
            INSERT INTO product_compositions_adapar (product, concentration, ingredient)
            VALUES (@product, @concentration, @ingredient)";

    using (SqlCommand command = new SqlCommand(insertCompositionQuery, connection))
    {
        command.Parameters.AddWithValue("@product", productId);
        command.Parameters.AddWithValue("@concentration", ingredient.concentration);
        command.Parameters.AddWithValue("@ingredient", ingredient.ingredient_description);

        command.ExecuteNonQuery();
    }
}

public class Product
{
    public string product_description { get; set; } = string.Empty;
    public string? register_number { get; set; }
    public string? status { get; set; }
    public string? product_class { get; set; }
    public string? category { get; set; }
    public string? company { get; set; }
    public string? classification { get; set; }
    public string? flammability { get; set; }
    public string? formulation { get; set; }
    public string? action_form { get; set; }
    public string? bulletin_link { get; set; }
    public List<ActiveIngredient> active_ingredients { get; set; } = new();
}

public class ActiveIngredient
{
    public string? ingredient_description { get; set; }
    public string? concentration { get; set; }
}
