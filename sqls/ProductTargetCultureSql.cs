namespace Adapar.sqls
{
    public class ProductTargetCultureSql
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid Product { get; set; }
        public string Culture { get; set; }
        public string StatusCulture { get; set; }
        public string Target { get; set; }
        public string StatusTarget { get; set; }
        public string DetailLink { get; set; }
        public string ScientificName { get; set; }
        public string CommonName { get; set; }
        public string Dosage { get; set; }
        public string SafetyRange { get; set; }
        public string Observation { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime SyncedAt { get; set; }
    }
}