using Business.Objects;
using Nest;

namespace Business
{
    public class CompletionSuggesterService : AutocompleteBaseService, IAutocompleteService
    {
        public CompletionSuggesterService(ConnectionSettings connectionSettings) : base(connectionSettings)
        {
        }

        public override async Task<bool> CreateIndexAsync(string indexName)
        {
            var indexDescriptor = new CreateIndexDescriptor(indexName)
                .Map<CompletionProduct>(md => md
                        .AutoMap()
                        .Properties(pd => pd
                            .Completion(cd => cd
                                .Name(product => product.Suggest)
                            )
                        )
                );
            
            var indexExists = _elasticClient.Indices.Exists(indexName.ToLowerInvariant());

            if (indexExists != null && indexExists.Exists)
                _elasticClient.Indices.Delete(indexName.ToLowerInvariant());

            CreateIndexResponse createIndexResponse = await _elasticClient.Indices.CreateAsync(indexDescriptor);

            return createIndexResponse.IsValid;
        }

        public override async Task<ProductSuggestResponse> SuggestProductAsync(string indexName, string term)
        {
            ISearchResponse<CompletionProduct> searchResponse = await _elasticClient.SearchAsync<CompletionProduct>(s => s
                                                               .Index(indexName)
                                                               .Suggest(sd => sd
                                                                    .Completion("suggestions", cd => cd 
                                                                        .Field(product => product.Suggest)
                                                                        .Prefix(term)
                                                                        .Fuzzy(fd => fd 
                                                                            .Fuzziness(Fuzziness.Auto))
                                                                        .Size(5)
                                                                    )
                                                               ));
            var suggests = from suggest in searchResponse.Suggest["suggestions"]
                            from option in suggest.Options
                           select new ProductSuggest()
                           {
                               Id = option.Source.Id,
                               Name = option.Source.Name,
                               SuggestedName = option.Text,
                               Score = option.Score
                           };

            return new ProductSuggestResponse { Suggests = suggests };
        }
    }
}
