using System;
using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Issue : SoftwareProcessElement
    {
        public DateTime? OccuredOn;

        public List<ProcessStep> IsRaisedBy;
        public List<Entity> Reviews;
        public List<Alternative> HasResponse;

        public Issue(string identifier = null, string name = null, string description = null, DateTime? occuredOn = null)
            : base(UPMMTypes.Issue, identifier, name, description)
        {
            OccuredOn = occuredOn;

            IsRaisedBy = new List<ProcessStep>();
            Reviews = new List<Entity>();
            HasResponse = new List<Alternative>();
        }

        public Issue() : base()
        {
            IsRaisedBy = new List<ProcessStep>();
            Reviews = new List<Entity>();
            HasResponse = new List<Alternative>();
        }
    }
}
