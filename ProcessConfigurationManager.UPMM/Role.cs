using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Role : SoftwareProcessElement
    {
        public List<Competence> Specifies;
        public List<Task> Performs;
        public List<Alternative> Selects;
        public List<Object> ResponsibleFor;
        public List<Resource> IsPlayedBy;
        public List<Entity> Modifies;
        public List<Group> RoleIsIn;

        public Role(UPMMTypes type=UPMMTypes.Role, string identifier = null, string name = null, string description = null)
            : base(type, identifier, name, description)
        {
            Specifies = new List<Competence>();
            Performs = new List<Task>();
            Selects = new List<Alternative>();
            ResponsibleFor = new List<Object>();
            IsPlayedBy = new List<Resource>();
            Modifies = new List<Entity>();
            RoleIsIn = new List<Group>();
        }

        public Role() : base()
        {
            Specifies = new List<Competence>();
            Performs = new List<Task>();
            Selects = new List<Alternative>();
            ResponsibleFor = new List<Object>();
            IsPlayedBy = new List<Resource>();
            Modifies = new List<Entity>();
            RoleIsIn = new List<Group>();
        }
    }
}
