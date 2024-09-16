namespace Adapar.dtos
{
    public class ProductDto
    {
        public string product_description { get; set; } = string.Empty;
        public string? status { get; set; }
        public string? category { get; set; }
        public string? commercial_name { get; set; }
        public string? product_class { get; set; }
        public string? register_number { get; set; }
        public string? toxicological_classification { get; set; }
        public string? classification { get; set; }
        public string? flammability { get; set; }
        public string? formulation { get; set; }
        public string? action_form { get; set; }
        public string? registering_company { get; set; }
        public string? bulletin_link { get; set; }
        public List<ActiveIngredientDto> active_ingredients { get; set; } = new();
        public List<TargetCultureDto> target_culture { get; set; } = new();
    }
}