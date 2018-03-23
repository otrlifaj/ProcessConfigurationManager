using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Law : SoftwareProcessElement
    {
        public int CompetenceLevel { get; set; }
        public string RegulationCode { get; set; }

        public List<Task> Controls;
        public List<Competence> IsCheckedBy;

        public Law(string identifier = null, string name = null, string description = null, int competenceLevel = 0, string regulationCode = null)
            : base(UPMMTypes.Law, identifier, name, description)
        {
            CompetenceLevel = competenceLevel;
            RegulationCode = regulationCode;

            Controls = new List<Task>();
            IsCheckedBy = new List<Competence>();
        }

        public Law() : base()
        {
            Controls = new List<Task>();
            IsCheckedBy = new List<Competence>();
        }
    }
}
