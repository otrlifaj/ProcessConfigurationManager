using System;
using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Event : ProcessStep
    {
        public DateTime? OccuredOn;
        public List<ProcessStep> ReceiveSignal; //processstep, but in real only task or process, not event!!

        public Event(string identifier = null, string name = null, string description = null, string guard = null, int cost = 0, int timeEstimation = 0, string location = null, Boolean plannedExecution = false, Boolean multipleExecution = false, Boolean optionalExecution = false, DateTime? occuredOn = null)
            : base(UPMMTypes.Event, identifier, name, description, guard, cost, timeEstimation, location, plannedExecution, multipleExecution, optionalExecution)
        {
            OccuredOn = occuredOn;
            ReceiveSignal = new List<ProcessStep>();
        }

        public Event() : base()
        {
            ReceiveSignal = new List<ProcessStep>();
        }
    }
}
