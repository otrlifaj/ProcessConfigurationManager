using System;
using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Process : ProcessStep
    {
        #region variables
        public int Acceptance { get; set; }
        public int Performance { get; set; }
        public int Reusability { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        #endregion


        #region relations
        public List<Process> Collaborates;
        public List<ProcessStep> ParentProcess;
        public List<Event> IsActivatedBy;
        public List<Goal> Realizes;
        public List<Object> Uses;
        #endregion

        public Process(string identifier = null, string name = null, string description = null, string guard = null, int cost = 0, int timeEstimation = 0, string location = null, Boolean plannedExecution = false, Boolean multipleExecution = false, Boolean optionalExecution = false, int acceptance = 0, int performance = 0, int reusability = 0, DateTime? start = null, DateTime? end = null)
            : base(UPMMTypes.Process, identifier, name, description, guard, cost, timeEstimation, location, plannedExecution, multipleExecution, optionalExecution)
        {
            Acceptance = acceptance;
            Performance = performance;
            Reusability = reusability;
            Start = start;
            End = end;

            Collaborates = new List<Process>();
            ParentProcess = new List<ProcessStep>();
            IsActivatedBy = new List<Event>();
            Realizes = new List<Goal>();
            Uses = new List<Object>();
        }

        public Process() : base()
        {
            Collaborates = new List<Process>();
            ParentProcess = new List<ProcessStep>();
            IsActivatedBy = new List<Event>();
            Realizes = new List<Goal>();
            Uses = new List<Object>();
        }
    }
}
