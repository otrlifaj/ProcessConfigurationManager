using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Alternative : SoftwareProcessElement
    {
        public List<ProcessStep> ContributesTo;
        public List<Issue> RespondsTo;
        public List<Role> IsSelectedBy;

        public Alternative(string identifier = null, string name = null, string description = null)
            : base(UPMMTypes.Alternative, identifier, name, description)
        {
            ContributesTo = new List<ProcessStep>();
            RespondsTo = new List<Issue>();
            IsSelectedBy = new List<Role>();
        }

        public Alternative() : base()
        {
            ContributesTo = new List<ProcessStep>();
            RespondsTo = new List<Issue>();
            IsSelectedBy = new List<Role>();
        }
    }
}
