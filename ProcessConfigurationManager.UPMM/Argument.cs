using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Argument : SoftwareProcessElement
    {
        public List<Alternative> ObjectsTo;
        public List<Alternative> Supports;

        public Argument(string identifier = null, string name = null, string description = null)
            : base(UPMMTypes.Argument, identifier, name, description)
        {
            ObjectsTo = new List<Alternative>();
            Supports = new List<Alternative>();
        }

        public Argument() : base()
        {
            ObjectsTo = new List<Alternative>();
            Supports = new List<Alternative>();
        }
    }
}
