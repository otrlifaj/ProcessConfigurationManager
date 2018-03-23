
namespace ProcessConfigurationManager.UPMM
{
    public class Material : Artifact
    {
        public Material(string identifier = null, string name = null, string description = null, int volatility = 0, int priority = 0, string language = null, int cost = 0)
            : base(UPMMTypes.Material, identifier, name, description, volatility, priority, language, cost)
        {
        }

        public Material() : base()
        {

        }
    }
}
