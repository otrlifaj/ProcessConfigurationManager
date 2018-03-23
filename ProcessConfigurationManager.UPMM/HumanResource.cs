using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class HumanResource : Resource
    {
        public int YearsOfExperience { get; set; }
        public string ExpertiseLevel { get; set; }
        public List<HumanResource> IsOrganizedWith;

        public HumanResource(string identifier = null, string name = null, string description = null, int cost = 0, int yearsOfExperience = 0, string expertiseLevel = null)
            : base(UPMMTypes.HumanResource,identifier, name, description, cost)
        {
            YearsOfExperience = yearsOfExperience;
            ExpertiseLevel = expertiseLevel;

            IsOrganizedWith = new List<HumanResource>();
        }

        public HumanResource() : base()
        {
            IsOrganizedWith = new List<HumanResource>();
        }
    }
}
