using Business.Objects;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class NgramService : AutocompleteBaseService, IAutocompleteService
    {
        public NgramService(ConnectionSettings connectionSettings) : base(connectionSettings)
        {
        }

        public override async Task<bool> CreateIndexAsync(string indexName)
        {
            indexName = indexName.ToLowerInvariant();

            if ((await _elasticClient.Indices.ExistsAsync(indexName)).Exists)
                await _elasticClient.Indices.DeleteAsync(indexName);

            var createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, cd => cd
                .Settings(sd => sd
                    .Analysis(ad => ad
                        .Tokenizers(td => td
                            .EdgeNGram("autocomplete_tokenizer", ed => ed
                                .MinGram(2)
                                .MaxGram(20)
                                .TokenChars(
                                    TokenChar.Letter,
                                    TokenChar.Digit
                                )
                            )
                        )
                        .Analyzers(and => and
                            .Custom("autocomplete", cad => cad
                                .Tokenizer("autocomplete_tokenizer")
                                .Filters("lowercase")
                            )
                            .Custom("autocomplete_search", cad => cad
                                .Tokenizer("lowercase")
                            )
                        )
                    )
                )
                .Map<Product>(md => md
                    .AutoMap()
                    .Properties(psd => psd
                        .Text(td => td
                            .Name(product => product.Name)
                            .Analyzer("standard")
                            .Fields(f => f
                                .Text(ttd => ttd
                                    .Name("autocomplete")
                                    .Analyzer("autocomplete")
                                    .SearchAnalyzer("autocomplete_search")
                                )
                            )
                        )
                    )
                )
            );

            return createIndexResponse.IsValid;
        }

        public override async Task<ProductSuggestResponse> SuggestProductAsync(string indexName, string term)
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                                .Index(indexName)
                                                .Size(10)
                                                .Query(q => q
                                                    .Match(m => m
                                                        .Field("name.autocomplete")
                                                        .Query(term)
                                                        .Operator(Operator.And)
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
