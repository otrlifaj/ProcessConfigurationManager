using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public abstract class Entity : Object
    {
        #region variables
        public int Volatility { get; set; }
        public int Priority { get; set; }
        public string Language { get; set; }
        #endregion

        #region relations
        public List<Resource> IsProcessedBy;
        public List<Task> OutputFrom;
        public List<Task> InputTo;
        public List<Task> OptionalInputTo;
        public List<Task> MandatoryInputTo;
        public List<Goal> ResultsIn;
        public List<Issue> IsReviewedBy;
        public List<Entity> InteractsWith;
        public List<Role> ModifiedBy;
        #endregion

        public Entity(UPMMTypes type = UPMMTypes.Entity, string identifier = null, string name = null, string description = null, int volatility = 0, int priority = 0, string language = null)
            : base(type, identifier, name, description)
        {
            Volatility = volatility;
            Priority = priority;
            Language = language;

            IsProcessedBy = new List<Resource>();
            OutputFrom = new List<Task>();
            InputTo = new List<Task>();
            MandatoryInputTo = new List<Task>();
            OptionalInputTo = new List<Task>();
            ResultsIn = new List<Goal>();
            IsReviewedBy = new List<Issue>();
            InteractsWith = new List<Entity>();
            ModifiedBy = new List<Role>();
        }

        public Entity() : base()
        {
            IsProcessedBy = new List<Resource>();
            OutputFrom = new List<Task>();
            InputTo = new List<Task>();
            MandatoryInputTo = new List<Task>();
            OptionalInputTo = new List<Task>();
            ResultsIn = new List<Goal>();
            IsReviewedBy = new List<Issue>();
            InteractsWith = new List<Entity>();
            ModifiedBy = new List<Role>();
        }
    }
}
