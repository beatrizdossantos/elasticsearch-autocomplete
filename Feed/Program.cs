using Business;
using Business.Objects;
using Nest;

namespace Feed
{
    class Program
    {
        static void Main(string[] args)
        {
            IndexNormalProducts();
            IndexCompletionSuggesterProducts();
        }

        static void IndexNormalProducts()
        {
            List<Product> products = new List<Product>();
            FillProductList(products);

            var connectionString = new ConnectionSettings(new Uri("http://localhost:9200"));

            IAutocompleteService autocompleteService = new SearchAsYouTypeService(connectionString);

            string productSuggestIndex = "product";
            bool isCreated = autocompleteService.CreateIndexAsync(productSuggestIndex).Result;

            if (isCreated)
                autocompleteService.IndexAsync(productSuggestIndex, products).Wait();
        }

        static void FillProductList(List<Product> products)
        {
            products.Add(new Product()
            {
                Id = 1,
                Name = "Samsung Galaxy Note 8"
            });

            products.Add(new Product()
            {
                Id = 2,
                Name = "Samsung Galaxy S8"
            });

            products.Add(new Product()
            {
                Id = 3,
                Name = "Apple Iphone 8"
            });

            products.Add(new Product()
            {
                Id = 4,
                Name = "Apple Iphone X"
            });

            products.Add(new Product()
            {
                Id = 5,
                Name = "Apple iPad Pro"
            });
        }

        static void IndexCompletionSuggesterProducts()
        {
            List<CompletionProduct> products = new List<CompletionProduct>();
            FillCompletionProductList(products);

            var connectionString = new ConnectionSettings(new Uri("http://localhost:9200"));

            IAutocompleteService autocompleteService = new CompletionSuggesterService(connectionString);

            string productSuggestIndex = "product_suggester";
            bool isCreated = autocompleteService.CreateIndexAsync(productSuggestIndex).Result;

            if (isCreated)
                autocompleteService.IndexCompletionAsync(productSuggestIndex, products).Wait();
        }

        static void FillCompletionProductList(List<CompletionProduct> products)
        {
            products.Add(new CompletionProduct()
            {
                Id = 1,
                Name = "Samsung Galaxy Note 8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Samsung Galaxy Note 8", "Galaxy Note 8", "Note 8" }
                }
            });

            products.Add(new CompletionProduct()
            {
                Id = 2,
                Name = "Samsung Galaxy S8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Samsung Galaxy S8", "Galaxy S8", "S8" }
                }
            });

            products.Add(new CompletionProduct()
            {
                Id = 3,
                Name = "Apple Iphone 8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone 8", "Iphone 8" }
                }
            });

            products.Add(new CompletionProduct()
            {
                Id = 4,
                Name = "Apple Iphone X",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone X", "Iphone X" }
                }
            });

            products.Add(new CompletionProduct()
            {
                Id = 5,
                Name = "Apple iPad Pro",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple iPad Pro", "iPad Pro" }
                }
            });
        }
    }
}