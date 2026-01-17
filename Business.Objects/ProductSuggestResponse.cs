namespace Business.Objects
{
    public class ProductSuggestResponse
    {
        public IEnumerable<ProductSuggest> Suggests { get; set; }
    }

    public class ProductSuggest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SuggestedName { get; set; }
        public double Score { get; set; }
    }
}
