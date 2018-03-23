
namespace ProcessConfigurationManager.UPMM
{
    public class Document : Artifact
    {
        public Document(string identifier = null, string name = null, string description = null, int volatility = 0, int priority = 0, string language = null, int cost = 0)
            : base(UPMMTypes.Document, identifier, name, description, volatility, priority, language, cost)
        {
        }

        public Document() : base() { }
    }
}
