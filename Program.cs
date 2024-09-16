using Adapar.dtos;
using Adapar.useCases;
using Adapar.useCases.Cache;
using Adapar.useCases.Composition;
using Adapar.useCases.Products;
using Adapar.useCases.TargetCulture;
using Newtonsoft.Json;
using System.Data.SqlClient;

string? connectionString = Environment.GetEnvironmentVariable("SQL_SERVER");

if (string.IsNullOrWhiteSpace(connectionString))
  throw new ArgumentException("Connection string not found.");

string jsonFilePath = "agrochemicals.json";
string jsonContent = File.ReadAllText(jsonFilePath);
var products = JsonConvert.DeserializeObject<List<ProductDto>>(jsonContent);


using (SqlConnection connection = new SqlConnection(connectionString))
{
  try
  {
    await connection.OpenAsync();

    Console.WriteLine("open connection");
    await CacheManager.Init(connection);

    foreach (var product in products)
    {
      try
      {
        if (product.active_ingredients.Count == 0)
        {
          Log.LogError(product);
          continue;
        }

        Guid productId;

        if (ProductExist.Find(CacheManager.productsSql, product.product_description))
        {
          await ProductUpdate.Async(connection,
                  product,
                  CacheManager.productsSql.FirstOrDefault(p => p.Description == product.product_description));
          continue;
        }

        productId = await ProductCreate.Async(connection, product);

        if (productId == Guid.Empty)
          continue;

        foreach (var ingredient in product.active_ingredients)
        {
          await CompositionCreate.Async(connection, productId, ingredient);
        }

        foreach (var targetCulture in product.target_culture)
        {
          await TargetCultureCreate.Async(connection, productId, targetCulture);
        }
      }
      catch (Exception ex)
      {
        System.Console.WriteLine(ex.Message);
        Log.LogError(product);
      }


    }
  }
  catch (Exception ex)
  {
    Console.WriteLine("ERROR:", ex.Message);
  }
  finally
  {
    Console.WriteLine("close connection");
    await connection.CloseAsync();
  }
}