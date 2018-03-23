using System;
using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Task : ProcessStep
    {
        #region variables
        public int Priority { get; set; }
        public Boolean IsAtomicStep { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        #endregion

        #region relations
        public List<Task> HasSubStep;
        public List<Event> IsActivatedBy;
        public List<Law> IsControlledBy;
        public List<Role> IsPerformedBy;
        public List<Entity> Input;
        public List<Entity> OptionalInput;
        public List<Entity> MandatoryInput;
        public List<Entity> Output;
        #endregion

        public Task(string identifier = null, string name = null, string description = null, string guard = null, int cost = 0, int timeEstimation = 0, string location = null, Boolean plannedExecution = false, Boolean multipleExecution = false, Boolean optionalExecution = false, int priority = 0, Boolean isAtomicStep = false, DateTime? start = null, DateTime? end = null)
            : base(UPMMTypes.Task, identifier, name, description, guard, cost, timeEstimation, location, plannedExecution, multipleExecution, optionalExecution)
        {
            Priority = priority;
            IsAtomicStep = isAtomicStep;
            Start = start;
            End = end;

            HasSubStep = new List<Task>();
            IsActivatedBy = new List<Event>();
            IsControlledBy = new List<Law>();
            IsPerformedBy = new List<Role>();
            Input = new List<Entity>();
            OptionalInput = new List<Entity>();
            MandatoryInput = new List<Entity>();
            Output = new List<Entity>();
        }

        public Task() : base()
        {
            HasSubStep = new List<Task>();
            IsActivatedBy = new List<Event>();
            IsControlledBy = new List<Law>();
            IsPerformedBy = new List<Role>();
            Input = new List<Entity>();
            OptionalInput = new List<Entity>();
            MandatoryInput = new List<Entity>();
            Output = new List<Entity>();
        }
    }
}
