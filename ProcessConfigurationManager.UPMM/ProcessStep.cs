using System;
using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public abstract class ProcessStep : SoftwareProcessElement
    {
        #region variables
        public string Guard { get; set; }
        public int Cost { get; set; }
        public int TimeEstimation { get; set; }
        public string Location { get; set; }
        public Boolean PlannedExecution { get; set; }
        public Boolean MultipleExecution { get; set; }
        public Boolean OptionalExecution { get; set; }

        #endregion

        #region relations
        public List<Parameter> Parameters;

        public List<Event> SendSignal;
        public List<Intention> HasTarget;
        public List<Intention> HasSource;
        public List<Issue> Raises;
        public List<Alternative> Decides;
        public List<Context> IsBuildOn;

        #endregion

        public ProcessStep(UPMMTypes type=UPMMTypes.ProcessStep, string identifier = null, string name = null, string description = null, string guard = null, int cost = 0, int timeEstimation = 0, string location = null, Boolean plannedExecution = false, Boolean multipleExecution = false, Boolean optionalExecution = false)
            : base(type, identifier, name, description)
        {
            Guard = guard;
            Cost = cost;
            TimeEstimation = timeEstimation;
            Location = location;
            PlannedExecution = plannedExecution;
            MultipleExecution = multipleExecution;
            OptionalExecution = optionalExecution;

            Parameters = new List<Parameter>();

            SendSignal = new List<Event>();
            HasTarget = new List<Intention>();
            HasSource = new List<Intention>();
            Raises = new List<Issue>();
            Decides = new List<Alternative>();
            IsBuildOn = new List<Context>();

        }

        public ProcessStep() : base()
        {
            Parameters = new List<Parameter>();

            SendSignal = new List<Event>();
            HasTarget = new List<Intention>();
            HasSource = new List<Intention>();
            Raises = new List<Issue>();
            Decides = new List<Alternative>();
            IsBuildOn = new List<Context>();
        }

    }
}
