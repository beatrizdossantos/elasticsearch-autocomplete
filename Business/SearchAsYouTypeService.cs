using Business.Objects;
using Nest;

namespace Business
{
    public class SearchAsYouTypeService : AutocompleteBaseService, IAutocompleteService
    {
        public SearchAsYouTypeService(ConnectionSettings connectionSettings) : base(connectionSettings)
        {
        }

        public override async Task<bool> CreateIndexAsync(string indexName)
        {
            var response = await _elasticClient.Indices.CreateAsync(indexName, c => c
                                                        .Map<Product>(md => md
                                                           .AutoMap()
                                                            .Properties(pd => pd
                                                                .SearchAsYouType(sd => sd
                                                                    .Name(product => product.Name)
                                                                )
                                                            )
                                                        )
                                                      );

            return response.IsValid;
        }

        public override async Task<ProductSuggestResponse> SuggestProductAsync(string indexName, string term)
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(sd => sd
                                                .Index(indexName)
                                                .Size(5)
                                                .Query(qd => qd
                                                    .MultiMatch(mmd => mmd
                                                        .Query(term)
                                                        .Type(TextQueryType.BoolPrefix)
                                                        .Fields(f => f
                                                            .Field(product => product.Name)
                                                            .Field("name._2gram")
                                                            .Field("name._3gram")
                                                        )
                                                    )
                                                )
                                               );

            var responseDocuments = searchResponse.Documents.Select(document => new ProductSuggest() { Name = document.Name });
            var productSuggestReponse = new ProductSuggestResponse()
            {
                Suggests = responseDocuments
            };

            return productSuggestReponse;
        }
    }
}
