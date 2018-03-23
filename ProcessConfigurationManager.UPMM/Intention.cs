using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Intention : SoftwareProcessElement
    {
        public List<Goal> Concretizes;
        public List<Context> IsSatisfiedBy;

        public Intention(UPMMTypes type = UPMMTypes.Intention, string identifier = null, string name = null, string description = null)
            : base(type, identifier, name, description)
        {
            Concretizes = new List<Goal>();
            IsSatisfiedBy = new List<Context>();
        }

        public Intention() : base()
        {
            Concretizes = new List<Goal>();
            IsSatisfiedBy = new List<Context>();
        }
    }
}
