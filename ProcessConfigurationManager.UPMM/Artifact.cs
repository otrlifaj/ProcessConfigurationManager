using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Artifact : Entity
    {
        public int Cost { get; set; }
        public List<Entity> PartiallyConsistsOf;

        public Artifact(UPMMTypes type = UPMMTypes.Artifact, string identifier = null, string name = null, string description = null, int volatility = 0, int priority = 0, string language = null, int cost = 0)
            : base(type, identifier, name, description, volatility, priority, language)
        {
            Cost = cost;
            PartiallyConsistsOf = new List<Entity>();

        }

        public Artifact() : base()
        {
            PartiallyConsistsOf = new List<Entity>();
        }
    }
}
