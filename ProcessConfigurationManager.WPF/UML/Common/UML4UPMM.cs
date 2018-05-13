using ProcessConfigurationManager.UPMM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    public class UML4UPMM
    {
        private Dictionary<UPMMTypes, List<String>> ActivityDiagramNodeDataMappingRules;
        private Dictionary<UPMMTypes, List<String>> ClassDiagramNodeDataMappingRules;

        private List<SoftwareProcessElement> softwareProcess;


        public UML4UPMM(List<SoftwareProcessElement> softwareProcess)
        {
            InitADMappingRules();
            InitCDMappingRules();
            this.softwareProcess = softwareProcess;
        }

        private void InitADMappingRules()
        {
            ActivityDiagramNodeDataMappingRules = new Dictionary<UPMMTypes, List<String>>()
            {
                {UPMMTypes.Task, new List<String>() { "Activity" } },
                {UPMMTypes.Alternative, new List<String>() { "Activity" } },
                {UPMMTypes.Process, new List<String>() { "Activity" } },

                {UPMMTypes.Role, new List<String>() { "Object" } },
                {UPMMTypes.Group, new List<String>() { "Object" } },
                {UPMMTypes.Competence, new List<String>() { "Object" } },
                {UPMMTypes.Law, new List<String>() { "Object" } },


                {UPMMTypes.Object, new List<String>() { "Object" } },

                {UPMMTypes.Entity, new List<String>() { "Object" } },
                {UPMMTypes.Information, new List<String>() { "Object" } },
                {UPMMTypes.Artifact, new List<String>() { "Object" } },
                {UPMMTypes.Material, new List<String>() { "Object" } },
                {UPMMTypes.Document, new List<String>() { "Object" } },

                {UPMMTypes.Resource, new List<String>() { "Object" } },
                {UPMMTypes.HumanResource, new List<String>() { "Object" } },
                {UPMMTypes.InanimateResource, new List<String>() { "Object" } },

                {UPMMTypes.Event, new List<String>() { "Send Signal Action", "Accept Event Action" } },
                {UPMMTypes.Issue, new List<String>() { "Send Signal Action", "Accept Event Action" } },

                {UPMMTypes.Context, new List<String>() { "Swimlane" } },

                {UPMMTypes.Goal, new List<String>() { "Note" } },
                {UPMMTypes.Intention, new List<String>() { "Note" } },
                {UPMMTypes.Argument, new List<String>() { "Note" } }
            };
        }

        private void InitCDMappingRules()
        {
            ClassDiagramNodeDataMappingRules = new Dictionary<UPMMTypes, List<string>>()
            {
                {UPMMTypes.Context, new List<String>() {"Class"}},

                {UPMMTypes.Object, new List<String>() {"Class"}},

                {UPMMTypes.Entity, new List<String>() {"Class"}},
                {UPMMTypes.Information, new List<String>() {"Class"}},
                {UPMMTypes.Artifact, new List<String>() {"Class"}},
                {UPMMTypes.Material, new List<String>() {"Class"}},
                {UPMMTypes.Document, new List<String>() {"Class"}},

                {UPMMTypes.Resource, new List<String>() {"Class"}},
                {UPMMTypes.HumanResource, new List<String>() {"Class"}},
                {UPMMTypes.InanimateResource, new List<String>() {"Class"}},

                {UPMMTypes.Alternative, new List<String>() {"Class"}},
                {UPMMTypes.Argument, new List<String>() {"Class"}},
                {UPMMTypes.Issue, new List<String>() {"Class"}},
                {UPMMTypes.Intention, new List<String>() {"Class"}},
                {UPMMTypes.Goal, new List<String>() {"Class"}},
                {UPMMTypes.Competence, new List<String>() {"Class"}},
                {UPMMTypes.Role, new List<String>() {"Class"}},
                {UPMMTypes.Group, new List<String>() {"Class"}},
                {UPMMTypes.Law, new List<String>() {"Class"}},

            };

        }

        public List<ClassDiagramNodeData> MapUPMMToClassDiagramNodeData()
        {
            List<ClassDiagramNodeData> result = new List<ClassDiagramNodeData>();
            foreach (var item in softwareProcess)
            {
                result.AddRange(GetListOfClassDiagramNodes(item));
            }
            return result;
        }

        private List<ClassDiagramNodeData> GetListOfClassDiagramNodes(SoftwareProcessElement item)
        {
            List<ClassDiagramNodeData> result = new List<ClassDiagramNodeData>();
            List<String> categories = ClassDiagramNodeDataMappingRules.Where(x => x.Key == item.Type).Select(x => x.Value).FirstOrDefault();
            if (categories == null)
                return result;
            foreach (var category in categories)
            {
                result.Add(new ClassDiagramNodeData(item, category));
            }
            return result;
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

        public string CheckCDRelationship(string sourceIRI, string targetIRI, bool validation, out string color, string linkType)
        {
            color = Constants.VALID_COLOR;
            SoftwareProcessElement sourceElement = softwareProcess.FirstOrDefault(x => x.IRI == sourceIRI);
            SoftwareProcessElement targetElement = softwareProcess.FirstOrDefault(x => x.IRI == targetIRI);

            if (sourceElement == null || targetElement == null)
            {
                return null;
            }
            else if (linkType == Constants.UML_CD_ASSOCIATION)
            {
                return CheckCDAssociation(sourceElement, targetElement, validation, out color);
            }
            else if (linkType == Constants.UML_CD_GENERALIZATION)
            {
                return CheckCDGeneralization(sourceElement, targetElement, validation, out color);
            }
            else if (linkType == Constants.UML_CD_AGGREGATION)
            {
                return CheckCDAggregation(sourceElement, targetElement, validation, out color);
            }
            else if (linkType == Constants.UML_CD_ANCHOR)
            {
                return CheckCDAnchor(sourceElement, targetElement, validation, out color);
            }
            else
            {
                return null;
            }
        }

        private string CheckCDAnchor(SoftwareProcessElement sourceElement, SoftwareProcessElement targetElement, bool validation, out string color)
        {
            color = Constants.VALID_COLOR;
            return "";
        }

        private string CheckCDGeneralization(SoftwareProcessElement sourceElement, SoftwareProcessElement targetElement, bool validation, out string color)
        {
            color = Constants.VALID_COLOR;
            if (sourceElement is Information)
            {
                if (targetElement is Information)
                {
                    return ValidCDRelationship("", out color);
                }
            }
            else if (sourceElement is Artifact || sourceElement is Material || sourceElement is Document)
            {
                if (targetElement is Artifact || targetElement is Material || targetElement is Document)
                {
                    return ValidCDRelationship("", out color);
                }
            }
            else if (sourceElement is HumanResource)
            {
                if (targetElement is HumanResource)
                {
                    return ValidCDRelationship("", out color);
                }
            }
            else if (sourceElement is InanimateResource)
            {
                if (targetElement is InanimateResource)
                {
                    return ValidCDRelationship("", out color);
                }
            }

            return null;

        }

        private string CheckCDAssociation(SoftwareProcessElement sourceElement, SoftwareProcessElement targetElement, bool validation, out string color)
        {
            color = Constants.VALID_COLOR;
            if (sourceElement is Entity)
            {
                if (targetElement is Context)
                {
                    if ((sourceElement as Entity).IsInContext.Contains(targetElement))
                    {
                        return ValidCDRelationship("is in context", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("is in context", out color);
                    }
                }
                else if (targetElement is Goal)
                {
                    if ((sourceElement as Entity).ResultsIn.Contains(targetElement))
                    {
                        return ValidCDRelationship("results in", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("results in", out color);
                    }
                }
                else if (targetElement is Entity)
                {
                    if ((sourceElement as Entity).InteractsWith.Contains(targetElement))
                    {
                        return ValidCDRelationship("interacts with", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("interacts with", out color);
                    }
                }
            }
            else if (sourceElement is Context)
            {
                if (targetElement is Intention)
                {
                    if ((sourceElement as Context).Satisfies.Contains(targetElement))
                    {
                        return ValidCDRelationship("satisfies", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("satisfies", out color);
                    }
                }
            }
            else if (sourceElement is Argument)
            {
                if (targetElement is Alternative)
                {
                    if ((sourceElement as Argument).Supports.Contains(targetElement))
                    {
                        return ValidCDRelationship("supports", out color);
                    }
                    else if ((sourceElement as Argument).ObjectsTo.Contains(targetElement))
                    {
                        return ValidCDRelationship("objects to", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("", out color);
                    }
                }
            }
            else if (sourceElement is Role)
            {
                if (targetElement is Alternative)
                {
                    if ((sourceElement as Role).Selects.Contains(targetElement))
                    {
                        return ValidCDRelationship("selects", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("selects", out color);
                    }
                }
                else if (targetElement is UPMM.Object)
                {
                    if ((sourceElement as Role).ResponsibleFor.Contains(targetElement))
                    {
                        return ValidCDRelationship("is responsible for", out color);
                    }
                    else if (targetElement is Entity)
                    {
                        if ((sourceElement as Role).Modifies.Contains(targetElement))
                        {
                            return ValidCDRelationship("modifies", out color);
                        }
                        else
                        {
                            return validation ? null : InvalidCDRelationship("modifies", out color);
                        }
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("is responsible for", out color);
                    }
                }
                else if (targetElement is Competence)
                {
                    if ((sourceElement as Role).Specifies.Contains(targetElement))
                    {
                        return ValidCDRelationship("specifies", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("specifies", out color);
                    }
                }
            }
            else if (sourceElement is Alternative)
            {
                if (targetElement is Issue)
                {
                    if ((sourceElement as Alternative).RespondsTo.Contains(targetElement))
                    {
                        return ValidCDRelationship("responds to", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("responds to", out color);
                    }
                }
            }
            else if (sourceElement is Issue)
            {
                if (targetElement is Entity)
                {
                    if ((sourceElement as Issue).Reviews.Contains(targetElement))
                    {
                        return ValidCDRelationship("reviews", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("reviews", out color);
                    }
                }
            }
            else if (sourceElement is Intention)
            {
                if (targetElement is Goal)
                {
                    if ((sourceElement as Intention).Concretizes.Contains(targetElement))
                    {
                        return ValidCDRelationship("concretizes", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("concretizes", out color);
                    }
                }
            }
            else if (sourceElement is Competence)
            {
                if (targetElement is Law)
                {
                    if ((sourceElement as Competence).Checks.Contains(targetElement))
                    {
                        return ValidCDRelationship("checks", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("checks", out color);
                    }
                }
            }
            else if (sourceElement is Resource)
            {
                if (targetElement is Entity)
                {
                    if ((sourceElement as Resource).Processes.Contains(targetElement))
                    {
                        return ValidCDRelationship("processes", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("processes", out color);
                    }
                }
                else if (sourceElement is HumanResource && targetElement is HumanResource)
                {
                    if ((sourceElement as HumanResource).IsOrganizedWith.Contains(targetElement))
                    {
                        return ValidCDRelationship("is organized with", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("is organized with", out color);
                    }
                }
                else if (targetElement is Role)
                {
                    if ((sourceElement as Resource).PlaysRole.Contains(targetElement))
                    {
                        return ValidCDRelationship("plays role", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("plays role", out color);
                    }
                }
                else if (targetElement is Competence)
                {
                    if ((sourceElement as Resource).Provides.Contains(targetElement))
                    {
                        return ValidCDRelationship("provides", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("provides", out color);
                    }
                }
            }
            return null;
        }

        private string CheckCDAggregation(SoftwareProcessElement sourceElement, SoftwareProcessElement targetElement, bool validation, out string color)
        {
            color = Constants.VALID_COLOR;
            if (sourceElement is Group)
            {
                if (targetElement is Role)
                {
                    if ((sourceElement as Group).ConsistsOf.Contains(targetElement))
                    {
                        return ValidCDRelationship("consists of", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("consists of", out color);
                    }
                }
            }
            else if (sourceElement is Artifact || sourceElement is Material || sourceElement is Document)
            {
                if (targetElement is Entity)
                {
                    if ((sourceElement as Artifact).PartiallyConsistsOf.Contains(targetElement))
                    {
                        return ValidCDRelationship("consists of", out color);
                    }
                    else
                    {
                        return validation ? null : InvalidCDRelationship("consists of", out color);
                    }
                }
            }

            return null;
        }

        private string ValidCDRelationship(string relationship, out string color)
        {
            color = Constants.VALID_COLOR;
            return relationship;
        }

        private string InvalidCDRelationship(string relationship, out string color)
        {
            color = Constants.INVALID_COLOR;
            return relationship;
        }
    }
}

