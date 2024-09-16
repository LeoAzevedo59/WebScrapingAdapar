using Adapar.sqls;

namespace Adapar.useCases.Products
{
    public class ProductExist
    {
        public static bool Find(List<ProductSql> products,
            string description)
        {
            return products.Exists(product => product.Description == description);
        }
    }
}