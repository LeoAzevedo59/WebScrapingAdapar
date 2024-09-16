using Adapar.dtos;
using Newtonsoft.Json;

namespace Adapar.useCases
{
  public class Log
  {
    private static string logFilePath = "log.json";

    public static void LogError(ProductDto product)
    {
      try
      {
        // Serializar o objeto Product em formato JSON
        string productJson = JsonConvert.SerializeObject(product, Formatting.Indented);

        // Verificar se o arquivo log.json já existe
        if (File.Exists(logFilePath))
        {
          // Reescrever o arquivo com os novos dados
          File.WriteAllText(logFilePath, productJson);
        }
        else
        {
          // Criar o arquivo e escrever os dados
          File.WriteAllText(logFilePath, productJson);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Erro ao registrar o log: " + ex.Message);
      }
    }
  }
}