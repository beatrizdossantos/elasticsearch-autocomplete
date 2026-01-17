using Business.Objects;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class AutocompleteBaseService : IAutocompleteService
    {
        protected readonly ElasticClient _elasticClient;

        public AutocompleteBaseService(ConnectionSettings connectionSettings)
        {
            _elasticClient = new ElasticClient(connectionSettings);
        }

        public virtual async Task<bool> CreateIndexAsync(string indexName)
        {
            var indexDescriptor = new CreateIndexDescriptor(indexName)
                .Map<Product>(md => md
                    .AutoMap()
                );

            var indexExists = _elasticClient.Indices.Exists(indexName.ToLowerInvariant());

            if (indexExists != null && indexExists.Exists)
                _elasticClient.Indices.Delete(indexName.ToLowerInvariant());

            CreateIndexResponse createIndexResponse = await _elasticClient.Indices.CreateAsync(indexDescriptor);

            return createIndexResponse.IsValid;
        }

        public async Task IndexAsync(string indexName, List<Product> products)
        {
            await _elasticClient.IndexManyAsync(products, indexName);
        }

        public async Task IndexCompletionAsync(string indexName, List<CompletionProduct> products)
        {
            await _elasticClient.IndexManyAsync(products, indexName);
        }

        public virtual async Task<ProductSuggestResponse> SuggestProductAsync(string indexName, string term)
        {
            // OBS: Usa apenas uma busca por prefixos, não é um autocomplete real!!
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                                        .Index(indexName)
                                                        .Size(5)
                                                        .Query(qd => qd
                                                            .Prefix(pd => pd
                                                                .Field(f => f.Name)
                                                                .Value(term.ToLower())
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
