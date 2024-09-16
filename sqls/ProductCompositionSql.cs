namespace Adapar.sqls
{
    public class ProductCompositionSql
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? Product { get; set; }
        public string? Concentration { get; set; }
        public string? Ingredient { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime SyncedAt { get; set; }
    }
}