using ProcessConfigurationManager.UPMM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    public class UML4UPMM
    {
        private Dictionary<UPMMTypes, List<String>> ActivityDiagramNodeDataMappingRules;
        private List<SoftwareProcessElement> softwareProcess;
        public UML4UPMM(List<SoftwareProcessElement> softwareProcess)
        {
            ActivityDiagramNodeDataMappingRules = new Dictionary<UPMMTypes, List<String>>();
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Task, new List<String>() { "Activity" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Alternative, new List<String>() { "Activity" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Process, new List<String>() { "Activity" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Role, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Group, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Competence, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Law, new List<String>() { "Object" });


            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Object, new List<String>() { "Object" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Entity, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Information, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Artifact, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Material, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Document, new List<String>() { "Object" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Resource, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.HumanResource, new List<String>() { "Object" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.InanimateResource, new List<String>() { "Object" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Event, new List<String>() { "Send Signal Action", "Accept Event Action" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Issue, new List<String>() { "Send Signal Action", "Accept Event Action" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Context, new List<String>() { "Swimlane" });

            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Goal, new List<String>() { "Note" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Intention, new List<String>() { "Note" });
            ActivityDiagramNodeDataMappingRules.Add(UPMMTypes.Argument, new List<String>() { "Note" });

            this.softwareProcess = softwareProcess;
        }

        public List<ClassDiagramNodeData> MapUPMMToClassDiagramNodeData()
        {
            return new List<ClassDiagramNodeData>();
        }

        public List<ActivityDiagramNodeData> MapUPMMToActivityDiagramNodeData()
        {
            List<ActivityDiagramNodeData> result = new List<ActivityDiagramNodeData>();
            foreach (var item in softwareProcess)
            {
                result.AddRange(GetListOfActivityDiagramNodes(item));
            }
            return result;
        }

        private List<ActivityDiagramNodeData> GetListOfActivityDiagramNodes(UPMM.SoftwareProcessElement item)
        {
            List<ActivityDiagramNodeData> result = new List<ActivityDiagramNodeData>();
            List<String> categories = ActivityDiagramNodeDataMappingRules.Where(x => x.Key == item.Type).Select(x => x.Value).FirstOrDefault();
            if (categories == null)
                return result;
            foreach (var category in categories)
            {
                result.Add(new ActivityDiagramNodeData(item, category));
            }
            return result;
        }

        public string CheckADRelationship(string sourceIRI, string targetIRI, bool validation, out string color)
        {
            SoftwareProcessElement sourceElement = softwareProcess.Where(x => x.IRI == sourceIRI).FirstOrDefault();
            SoftwareProcessElement targetElement = softwareProcess.Where(x => x.IRI == targetIRI).FirstOrDefault();

            if (sourceElement == null || targetElement == null)
            {
                color = "Black";
                return null;
            }

            #region Alternative
            if (sourceElement is UPMM.Alternative)
            {
                if (targetElement is UPMM.ProcessStep)
                {

                    if ((sourceElement as Alternative).ContributesTo.Contains(targetElement))
                    {
                        color = "Black";
                        return "contributes to";
                    }
                    else
                    {
                        color = "Red";
                        if (validation)
                        {
      
                            return null;
                        }
                        else
                        {
                            return "contributes to";
                        }
                    }
                }

            }
            #endregion
            #region Argument
            if (sourceElement is UPMM.Argument)
            {
                if (targetElement is Alternative)
                {
                    if ((sourceElement as Argument).Supports.Contains(targetElement))
                    {
                        color = "Black";
                        return "supports";
                    }
                    else if ((sourceElement as Argument).ObjectsTo.Contains(targetElement))
                    {
                        color = "Black";
                        return "objects to";
                    }
                    else
                    {
                        color = "Red";
                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "objects to or supports";
                        }
                    }
                }
            }

            #endregion
            #region Competence
            if (sourceElement is UPMM.Competence)
            {
                if (targetElement is UPMM.Law)
                {
                    if ((sourceElement as UPMM.Competence).Checks.Contains(targetElement))
                    {
                        color = "Black";
                        return "checks";
                    }
                    else
                    {
                        color = "Red";
                        if (validation)
                        { 
                            return null;
                        }
                        else
                        {
                            return "checks";
                        }
                    }
                }
            }
            #endregion
            #region Entity
            if (sourceElement is UPMM.Entity)
            {
                if (targetElement is UPMM.Task)
                {
                    if ((sourceElement as UPMM.Entity).MandatoryInputTo.Contains(targetElement))
                    {
                        color = "Black";
                        return "mandatory input";
                    }
                    else if ((sourceElement as UPMM.Entity).OptionalInputTo.Contains(targetElement))
                    {
                        color = "Black";
                        return "optional input";
                    }
                    else if ((sourceElement as UPMM.Entity).InputTo.Contains(targetElement))
                    {
                        color = "Black";
                        return "input";
                    }
                    else
                    {
                        color = "Red";
                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "input";
                        }
                    }
                }
                else if (targetElement is UPMM.Goal)
                {
                    if ((sourceElement as UPMM.Entity).ResultsIn.Contains(targetElement))
                    {
                        color = "Black";
                        return "results in";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "results in";
                        }
                    }
                }
            }
            #endregion
            #region Event
            if (sourceElement is UPMM.Event)
            {
                if (targetElement is UPMM.Task || targetElement is UPMM.Process)
                {
                    if ((sourceElement as Event).ReceiveSignal.Contains(targetElement))
                    {
                        color = "Black";
                        return "activates";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "activates";
                        }
                    }
                }
            }
            #endregion
            #region Intention
            if (sourceElement is UPMM.Intention)
            {
                if (targetElement is UPMM.Goal)
                {
                    if ((sourceElement as UPMM.Intention).Concretizes.Contains(targetElement))
                    {
                        color = "Black";
                        return "concretizes";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "concretizes";
                        }
                    }
                }
            }
            #endregion
            #region Issue
            if (sourceElement is UPMM.Issue)
            {
                if (targetElement is Alternative)
                {
                    if ((sourceElement as Issue).HasResponse.Contains(targetElement))
                    {
                        color = "Black";
                        return "has response";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "has response";
                        }
                    }
                }
            }
            #endregion
            #region Law
            if (sourceElement is UPMM.Law)
            {
                if (targetElement is UPMM.Task)
                {
                    if ((sourceElement as Law).Controls.Contains(targetElement))
                    {
                        color = "Black";
                        return "controls";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "controls";
                        }
                    }
                }
            }
            #endregion
            #region Object
            if (sourceElement is UPMM.Object)
            {
                if (targetElement is Process)
                {
                    if ((sourceElement as UPMM.Object).UsedIn.Contains(targetElement))
                    {
                        color = "Black";
                        return "is used in";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "is used in";
                        }
                    }
                }
            }
            #endregion
            #region ProcessStep
            if (sourceElement is UPMM.ProcessStep)
            {
                if (targetElement is UPMM.Alternative)
                {
                    if ((sourceElement as UPMM.ProcessStep).Decides.Contains(targetElement))
                    {
                        color = "Black";
                        return "decides";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "decides";
                        }
                    }
                }
                else if (targetElement is UPMM.Issue)
                {
                    if ((sourceElement as UPMM.ProcessStep).Raises.Contains(targetElement))
                    {
                        color = "Black";
                        return "raises";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "raises";
                        }
                    }
                }
                else if (targetElement is UPMM.ProcessStep)
                {
                    var allcooperations = softwareProcess.Where(x => x is Cooperation).Select(x => x as Cooperation);
                    int count = 0;
                    CooperationType type = CooperationType.IsCoordinatedWith;
                    foreach (Cooperation cooperation in allcooperations)
                    {
                        if (cooperation.Source == sourceElement && cooperation.Target == targetElement && cooperation.Relation == CooperationType.IsFollowedBy)
                        {
                            count++;
                            type = cooperation.Relation;
                        }
                        if (cooperation.Source == targetElement && cooperation.Target == sourceElement && cooperation.Relation == CooperationType.IsPrecededBy)
                        {
                            count++;
                            type = cooperation.Relation;
                        }
                        if (cooperation.Source == sourceElement && cooperation.Target == targetElement && cooperation.Relation == CooperationType.Interrupts)
                        {
                            count++;
                            type = cooperation.Relation;
                        }

                    }
                    if (count == 0)
                    {
                        color = "Red";
                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "is coordinated with";
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case CooperationType.IsPrecededBy:
                                color = "Black";
                                return "precedes";
                                break;
                            case CooperationType.IsFollowedBy:
                                color = "Black";
                                return "is followed by";
                                break;
                            case CooperationType.Interrupts:
                                color = "Black";
                                return "interrupts";
                                break;
                            default:
                                {
                                    color = "Red";
                                    if (validation)
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        return "is coordinated with";
                                    }
                                }
                        }
                    }
                }
            }
            #endregion
            #region Process
            if (sourceElement is UPMM.Process)
            {
                if (targetElement is UPMM.Event)
                {
                    if ((sourceElement as UPMM.Process).SendSignal.Contains(targetElement))
                    {
                        color = "Black";
                        return "sends signal";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "sends signal";
                        }
                    }
                }

                else if (targetElement is UPMM.Goal)
                {
                    if ((sourceElement as UPMM.Process).Realizes.Contains(targetElement))
                    {
                        color = "Black";
                        return "realizes";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "realizes";
                        }
                    }
                }

            }
            #endregion
            #region Resource
            if (sourceElement is UPMM.Resource)
            {
                if (targetElement is UPMM.Competence)
                {
                    if ((sourceElement as UPMM.Resource).Provides.Contains(targetElement))
                    {
                        color = "Black";
                        return "provides";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "provides";
                        }
                    }
                }
                else if (targetElement is UPMM.Role)
                {
                    if ((sourceElement as UPMM.Resource).PlaysRole.Contains(targetElement))
                    {
                        color = "Black";
                        return "plays";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "plays";
                        }
                    }
                }
                else if (targetElement is UPMM.Entity)
                {
                    if ((sourceElement as UPMM.Resource).Processes.Contains(targetElement))
                    {
                        color = "Black";
                        return "processes";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "processes";
                        }
                    }
                }
            }
            #endregion
            #region Role
            if (sourceElement is UPMM.Role)
            {
                if (targetElement is UPMM.Task)
                {
                    if ((sourceElement as UPMM.Role).Performs.Contains(targetElement))
                    {
                        color = "Black";
                        return "performs";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "performs";
                        }
                    }
                }
                else if (targetElement is UPMM.Alternative)
                {
                    if ((sourceElement as UPMM.Role).Selects.Contains(targetElement))
                    {
                        color = "Black";
                        return "selects";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "selects";
                        }
                    }
                }
                else if (targetElement is UPMM.Competence)
                {
                    if ((sourceElement as UPMM.Role).Specifies.Contains(targetElement))
                    {
                        color = "Black";
                        return "specifies";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "specifies";
                        }
                    }
                }
                else if (targetElement is UPMM.Object)
                {
                    if ((sourceElement as UPMM.Role).ResponsibleFor.Contains(targetElement))
                    {
                        color = "Black";
                        return "is responsible for";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "is responsible for";
                        }
                    }
                }

            }
            #endregion
            #region Task
            if (sourceElement is UPMM.Task)
            {
                if (targetElement is Event)
                {
                    if ((sourceElement as UPMM.Task).SendSignal.Contains(targetElement))
                    {
                        color = "Black";
                        return "sends signal";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "sends signal";
                        }
                    }
                }
                else if (targetElement is UPMM.Entity)
                {
                    if ((sourceElement as UPMM.Task).Output.Contains(targetElement))
                    {
                        color = "Black";
                        return "output";
                    }
                    else
                    {
                        color = "Red";

                        if (validation)
                        {
                            return null;
                        }
                        else
                        {
                            return "output";
                        }
                    }
                }
            }
            #endregion
            color = "Black";
            return null;

        }
    }
}

