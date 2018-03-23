using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public abstract class Resource : Object
    {
        public int Cost { get; set; }
        public List<Role> PlaysRole;
        public List<Entity> Processes;
        public List<Competence> Provides;

        public Resource(UPMMTypes type=UPMMTypes.Resource, string identifier = null, string name = null, string description = null, int cost = 0)
            : base(type, identifier, name, description)
        {
            Cost = cost;

            PlaysRole = new List<Role>();
            Processes = new List<Entity>();
            Provides = new List<Competence>();
        }

        public Resource() : base()
        {
            PlaysRole = new List<Role>();
            Processes = new List<Entity>();
            Provides = new List<Competence>();
        }
    }
}
