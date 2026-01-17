using Business.Objects;

namespace Business
{
    public interface IAutocompleteService
    {
        Task<bool> CreateIndexAsync(string indexName);
        Task IndexAsync(string indexName, List<Product> products);
        Task IndexCompletionAsync(string indexName, List<CompletionProduct> products);
        Task<ProductSuggestResponse> SuggestProductAsync(string indexName, string term);
    }
}
