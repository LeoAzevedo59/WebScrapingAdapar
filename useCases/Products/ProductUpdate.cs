using System.Data.SqlClient;
using Adapar.dtos;
using Adapar.sqls;
using Adapar.useCases.Composition;
using Adapar.useCases.TargetCulture;
using Dapper;

namespace Adapar.useCases.Products
{
  public class ProductUpdate
  {
    public static async Task Async(SqlConnection connection,
     ProductDto productNew, ProductSql productOld)
    {
      bool isUpdate = false;

      string queryUpdate = "updated_at = dbo.GetUtc(), synced_at = dbo.GetUtc()";

      if (!productNew.product_description.Equals(productOld.Description))
      {
        queryUpdate += ",description = @description";
        isUpdate = true;
      }
      if (!productNew.status.Equals(productOld.Status))
      {
        queryUpdate += ",status = @status";
        isUpdate = true;
      }
      if (!productNew.registering_company.Equals(productOld.Enterprise))
      {
        queryUpdate += ",enterprise = @enterprise";
        isUpdate = true;
      }
      if (!productNew.toxicological_classification.Equals(productOld.ClassToxicological))
      {
        queryUpdate += ",class_toxicological = @class_toxicological";
        isUpdate = true;
      }
      if (!productNew.classification.Equals(productOld.Classification))
      {
        queryUpdate += ",classification = @classification";
        isUpdate = true;
      }
      if (!productNew.flammability.Equals(productOld.Flammability))
      {
        queryUpdate += ",flammability = @flammability";
        isUpdate = true;
      }
      if (!productNew.formulation.Equals(productOld.Formulation))
      {
        queryUpdate += ",formulation = @formulation";
        isUpdate = true;
      }
      if (!productNew.bulletin_link.Equals(productOld.BulletinLink))
      {
        queryUpdate += ",bulletin_link = @bulletin_link";
        isUpdate = true;
      }
      if (!productNew.register_number.Equals(productOld.Code))
      {
        queryUpdate += ",code = @code";
        isUpdate = true;
      }
      if (!productNew.product_class.Equals(productOld.FullCategory))
      {
        queryUpdate += ",full_category = @full_category";
        isUpdate = true;
      }
      if (!productNew.action_form.Equals(productOld.ActionForm))
      {
        queryUpdate += ",action_form = @action_form";
        isUpdate = true;
      }

      if (isUpdate)
      {
        string query = $"UPDATE products SET {queryUpdate} WHERE id = '{productOld.Id}'";

        var parameters = new
        {
          description = productNew.product_description,
          productNew.status,
          enterprise = productNew.registering_company,
          class_toxicological = productNew.toxicological_classification,
          productNew.classification,
          productNew.flammability,
          productNew.formulation,
          productNew.bulletin_link,
          code = productNew.register_number,
          full_category = productNew.product_class,
          productNew.action_form
        };


        Console.WriteLine($"{productNew.product_description} - UPDATE");
        await connection.ExecuteAsync(query, parameters);
      }

      if (productNew.active_ingredients is not null)
        await CompositionUpdate.Async(connection, productNew.active_ingredients, productOld.Id);

      if (productNew.target_culture is not null)
        await TargetCultureUpdate.Async(connection, productNew.target_culture, productOld.Id);
    }
  }
}