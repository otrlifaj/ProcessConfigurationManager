using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Competence : SoftwareProcessElement
    {
        public List<Resource> IsProvidedBy;
        public List<Role> IsSpecifiedBy;
        public List<Law> Checks;

        public Competence(string identifier = null, string name = null, string description = null)
            : base(UPMMTypes.Competence, identifier, name, description)
        {
            IsProvidedBy = new List<Resource>();
            IsSpecifiedBy = new List<Role>();
            Checks = new List<Law>();
        }

        public Competence() : base()
        {
            IsProvidedBy = new List<Resource>();
            IsSpecifiedBy = new List<Role>();
            Checks = new List<Law>();
        }
    }
}
