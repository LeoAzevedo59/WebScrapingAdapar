namespace Adapar.sqls
{
    public class ProductSql
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string FullCategory { get; set; }
        public Guid? CategoryId { get; set; }
        public string Category { get; set; }
        public Guid? UnitId { get; set; }
        public string Unit { get; set; }
        public string ClassToxicological { get; set; }
        public string Enterprise { get; set; }
        public string Classification { get; set; }
        public string Flammability { get; set; }
        public string Formulation { get; set; }
        public string ActionForm { get; set; }
        public string BulletinLink { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime SyncedAt { get; set; }
    }
}