using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

public class Product
{
    public string? product_description { get; set; }
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
    public List<ActiveIngredient> active_ingredients { get; set; }  = new();
}

public class ActiveIngredient
{
    public string? ingredient_description { get; set; }
    public string? concentration { get; set; }
}

class Program
{
    // String de conexão ao banco de dados
    static string connectionString = "Server=tcp:brever-staging.database.windows.net,1433;Initial Catalog=staging;Persist Security Info=False;User ID=Adm;Password=UZf48vF7qhOyWffFZDTZ;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    static void Main(string[] args)
    {
        string jsonFilePath = "agrochemicals.json";

        // Carregar o JSON e desserializar
        string jsonContent = File.ReadAllText(jsonFilePath);
        var products = JsonConvert.DeserializeObject<List<Product>>(jsonContent);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            foreach (var product in products)
            {
                // Inserir o produto na tabela products_adapar e obter o ID inserido
                int productId = InsertProduct(connection, product);

    if(productId == 0)
    continue;

                // Inserir as composições (ingredientes ativos) na tabela product_compositions_adapar
                foreach (var ingredient in product.active_ingredients)
                {
                    InsertComposition(connection, productId, ingredient);
                }
            }
        }

        Console.WriteLine("Dados inseridos com sucesso.");
    }

    // Função para inserir um produto na tabela products_adapar
    static int InsertProduct(SqlConnection connection, Product product)
    {
        if(string.IsNullOrEmpty(product.product_description))
        return 0;

        string insertProductQuery = @"
            INSERT INTO products_adapar (description, code, status, category, classToxicological, enterprise, classification, flammability, formulation, action_form, bulletin_link)
            OUTPUT INSERTED.id
            VALUES (@description, @code, @status, @category, @classToxicological, @enterprise, @classification, @flammability, @formulation, @action_form, @bulletin_link)";
        
        using (SqlCommand command = new SqlCommand(insertProductQuery, connection))
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

            // Retorna o ID inserido
            return (int)command.ExecuteScalar();
        }
    }

    // Função para inserir ingredientes na tabela product_compositions_adapar
    static void InsertComposition(SqlConnection connection, int productId, ActiveIngredient ingredient)
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
}