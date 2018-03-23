using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public abstract class Object : SoftwareProcessElement
    {
        #region relationships
        public List<Context> IsInContext;

        public List<Process> UsedIn;

        public List<Role> HasResponsibleRole;

        public List<Parameter> Parameters;
        #endregion

        public Object(UPMMTypes type=UPMMTypes.Object, string identifier = null, string name = null, string description = null)
            : base(type, identifier, name, description)
        {
            IsInContext = new List<Context>();
            UsedIn = new List<Process>();
            HasResponsibleRole = new List<Role>();
            Parameters = new List<Parameter>();
        }

        public Object() : base()
        {
            IsInContext = new List<Context>();
            UsedIn = new List<Process>();
            HasResponsibleRole = new List<Role>();
            Parameters = new List<Parameter>();
        }
    }
}
