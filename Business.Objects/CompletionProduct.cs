using Nest;

namespace Business.Objects
{
    public class CompletionProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CompletionField Suggest { get; set; }
    }
}
