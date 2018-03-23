using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Context : SoftwareProcessElement
    {
        public string Location { get; set; }
        public List<ProcessStep> Executes;
        public List<Intention> Satisfies;
        public List<Context> HasSubContext;
        public List<Object> Scopes;

        public Context(string identifier = null, string name = null, string description = null, string location = null)
            : base(UPMMTypes.Context, identifier, name, description)
        {
            Location = location;

            Executes = new List<ProcessStep>();
            Satisfies = new List<Intention>();
            HasSubContext = new List<Context>();
            Scopes = new List<Object>();
        }

        public Context() : base()
        {
            Executes = new List<ProcessStep>();
            Satisfies = new List<Intention>();
            HasSubContext = new List<Context>();
            Scopes = new List<Object>();
        }
    }
}
