using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Goal : Intention
    {
        public List<Process> IsRealizedBy;
        public List<Intention> IsConcretizedBy;
        public List<Entity> HasResult;

        public Goal(string identifier = null, string name = null, string description = null)
            : base(UPMMTypes.Goal, identifier, name, description)
        {
            IsRealizedBy = new List<Process>();
            IsConcretizedBy = new List<Intention>();
            HasResult = new List<Entity>();
        }

        public Goal() : base()
        {
            IsRealizedBy = new List<Process>();
            IsConcretizedBy = new List<Intention>();
            HasResult = new List<Entity>();
        }
    }
}
