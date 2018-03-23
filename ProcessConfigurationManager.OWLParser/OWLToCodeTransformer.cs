using org.semanticweb.owlapi.model; // OWLOntologyManager
using ProcessConfigurationManager.UPMM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessConfigurationManager.OWLParser
{
    public static partial class OWLAPI
    {
        private static void Transform(List<OWLClass> owlClasses, List<SoftwareProcessElement> softwareProcess)
        {
            CreateAllIndividuals(owlClasses, softwareProcess);
            Trace.WriteLine("Loaded " + softwareProcess.Count + " individuals...");
            CreateAllRelationships(owlClasses, softwareProcess);
            Trace.WriteLine("Relations loaded as well...");
        }

        #region helper methods
        private static List<OWLClass> GetClasses()
        {
            List<OWLClass> owlClasses = new List<OWLClass>();
            foreach (OWLClass owlClass in Reasoner.getRootOntology().getClassesInSignature(true).toArray())
            {
                owlClasses.Add(owlClass);
            }
            return owlClasses;
        }
        private static List<OWLNamedIndividual> GetIndividuals(OWLClass owlClass)
        {

            List<OWLNamedIndividual> owlIndividuals = new List<OWLNamedIndividual>();
            foreach (OWLNamedIndividual owlIndividual in Reasoner.getInstances(owlClass, true).getFlattened().toArray())
            {
                owlIndividuals.Add(owlIndividual);
            }
            return owlIndividuals;
        }
        private static void GetBasicPropeties(OWLNamedIndividual owlIndividual, out string IRI, out string name, out string description)
        {
            IRI = owlIndividual.getIRI().toString();
            description = GetDescription(owlIndividual);
            name = GetLabel(owlIndividual);
        }
        private static string GetDescription(OWLNamedIndividual individual)
        {
            string description = null;
            var annotations = individual.getAnnotations(Manager.getOntology(ProfileIRI), DataFactory.getRDFSIsDefinedBy()).toArray().ToList();
            foreach (OWLAnnotation item in annotations)
            {
                description = (item.getValue() as OWLLiteral).getLiteral();
            }
            return description;
        }
        private static string GetLabel(OWLNamedIndividual individual)
        {
            string label = null;
            OWLAnnotation annotation = (OWLAnnotation)individual.getAnnotations(Manager.getOntology(ProfileIRI), DataFactory.getRDFSLabel()).toArray().FirstOrDefault();
            if (annotation.getProperty().isLabel())
            {
                label = (annotation.getValue() as OWLLiteral).getLiteral();
            }
            return label;
        }
        private static string GetConcreteDataPropertyValue(OWLNamedIndividual individual, string dataProperty)
        {
            if (!dataProperty.StartsWith("#"))
                dataProperty = "#" + dataProperty;
            IRI propertyIRI = IRI.create(MetamodelIRI.toString(), dataProperty);
            OWLDataProperty property = DataFactory.getOWLDataProperty(propertyIRI);
            OWLLiteral value = (OWLLiteral)Reasoner.getDataPropertyValues(individual, property).toArray().FirstOrDefault();
            if (value == null)
                return null;
            else
                return value.getLiteral();
        }
        private static List<string> GetConcreteObjectPropertyValues(OWLNamedIndividual individual, string objectProperty)
        {
            List<string> result = new List<string>();
            if (!objectProperty.StartsWith("#"))
                objectProperty = "#" + objectProperty;

            IRI propertyIRI = IRI.create(MetamodelIRI.toString(), objectProperty);
            OWLObjectProperty property = DataFactory.getOWLObjectProperty(propertyIRI);

            // PelletReasoner protože StructuralReasoner není schopný odvodit inverzní property
            foreach (var propertyValue in PelletReasoner.getObjectPropertyValues(individual, property).getFlattened().toArray())
            {
                if (propertyValue == null)
                    continue;
                else
                    result.Add((propertyValue as OWLNamedIndividual).getIRI().toString());
            }
            return result;
        }
        #endregion

        private static void CreateAllIndividuals(List<OWLClass> owlClasses, List<SoftwareProcessElement> softwareProcess)
        {

            foreach (OWLClass owlClass in owlClasses)
            {
                try
                {
                    UPMMTypes classType = IRIClassMap.Where(x => x.Key.toString() == owlClass.getIRI().toString()).Select(x => x.Value).FirstOrDefault();
                    switch (classType)
                    {
                        case UPMMTypes.Alternative: CreateAlternatives(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Argument: CreateArguments(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Artifact: CreateArtifacts(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Competence: CreateCompetences(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Context: CreateContexts(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Cooperation: // cooperations are created in CreateAllRelationships method
                            break;
                        case UPMMTypes.Document: CreateDocuments(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Entity: //entity is an abstract class
                            break;
                        case UPMMTypes.Event: CreateEvents(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Goal: CreateGoals(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Group: CreateGroups(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.HumanResource: CreateHumanResources(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.InanimateResource: CreateInanimateResources(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Information: CreateInformations(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Intention: CreateIntentions(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Issue: CreateIssues(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Law: CreateLaws(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Material: CreateMaterials(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Object: //object is an abstract class
                            break;
                        case UPMMTypes.Parameter: CreateParameters(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Process: CreateProcesses(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.ProcessStep: //process step is an abstract class
                            break;
                        case UPMMTypes.Resource: //alternative is an abstract class
                            break;
                        case UPMMTypes.Role: CreateRoles(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Task: CreateTasks(owlClass, softwareProcess);
                            break;
                        default: //throw new ApplicationException("Unknown class! IRI: " + owlClass.getIRI());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occured during creating individuals of class with IRI " + owlClass.getIRI(), ex);
                }
            }

        }
        private static void CreateAllRelationships(List<OWLClass> owlClasses, List<SoftwareProcessElement> softwareProcess)
        {
            foreach (OWLClass owlClass in owlClasses)
            {
                try
                {
                    UPMMTypes classType = IRIClassMap.Where(x => x.Key.toString() == owlClass.getIRI().toString()).Select(x => x.Value).FirstOrDefault();
                    switch (classType)
                    {
                        case UPMMTypes.Alternative: CreateAlternativeRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Argument: CreateArgumentRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Artifact: CreateArtifactRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Competence: CreateCompetenceRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Context: CreateContextRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Cooperation: CreateCooperationsRelationShips(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Document: CreateDocumentRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Entity: // entity is an abstract class
                            break;
                        case UPMMTypes.Event: CreateEventRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Goal: CreateGoalRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Group: CreateGroupRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.HumanResource: CreateHumanResourceRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.InanimateResource: CreateInanimateResourceRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Information: CreateInformationRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Intention: CreateIntentionRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Issue: CreateIssueRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Law: CreateLawRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Material: CreateMaterialRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Object: // object is an abstract class
                            break;
                        case UPMMTypes.Parameter: // TODO maybe in the future, optional parameters are currently not supported
                            break;
                        case UPMMTypes.Process: CreateProcessRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.ProcessStep: // process step is an abstract class
                            break;
                        case UPMMTypes.Resource: // resource is an abstract class
                            break;
                        case UPMMTypes.Role: CreateRoleRelationships(owlClass, softwareProcess);
                            break;
                        case UPMMTypes.Task: CreateTaskRelationships(owlClass, softwareProcess);
                            break;
                        default: //throw new ApplicationException("Unknown class!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occured during initializing relationships between individuals of class with IRI " + owlClass.getIRI(), ex);
                }
            }
        }


        private static void CreateAlternatives(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Alternative(IRI, name, description));
            }
        }
        private static void CreateArguments(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Argument(IRI, name, description));
            }

        }
        private static void CreateArtifacts(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);


                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int volatility = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "volatility"), out volatility);

                int priority = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "priority"), out priority);

                string language = GetConcreteDataPropertyValue(individual, "language");

                softwareProcess.Add(new Artifact(identifier:IRI, name:name, description:description, volatility: volatility, priority: priority, cost: cost, language: language));
            }
        }
        private static void CreateCompetences(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Competence(IRI, name, description));
            }
        }
        private static void CreateContexts(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                string location = GetConcreteDataPropertyValue(individual, "location");
                softwareProcess.Add(new Context(IRI, name, description, location));
            }
        }
        private static void CreateDocuments(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);


                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int volatility = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "volatility"), out volatility);

                int priority = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "priority"), out priority);

                string language = GetConcreteDataPropertyValue(individual, "language");

                softwareProcess.Add(new Document(IRI, name, description, volatility: volatility, priority: priority, cost: cost, language: language));
            }
        }
        private static void CreateEvents(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                DateTime result;
                DateTime? occuredOn = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "occuredOn"), out result))
                    occuredOn = result;

                string guard = GetConcreteDataPropertyValue(individual, "guard");

                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int timeEstimation = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "timeEstimation"), out timeEstimation);

                string location = GetConcreteDataPropertyValue(individual, "location");
                Boolean planned = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "planned_execution"), out planned);
                Boolean multiple = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "multiple_execution"), out multiple);
                Boolean optional = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "optional_execution"), out optional);

                softwareProcess.Add(new UPMM.Event(IRI, name, description, guard: guard, cost: cost, timeEstimation: timeEstimation, location: location, plannedExecution: planned, multipleExecution: multiple, optionalExecution:

optional, occuredOn: occuredOn));
            }
        }
        private static void CreateGoals(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Goal(IRI, name, description));
            }
        }
        private static void CreateGroups(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Group(IRI, name, description));
            }
        }
        private static void CreateHumanResources(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                int yearsOfExperience;
                int.TryParse(GetConcreteDataPropertyValue(individual, "yearsOfExperience"), out yearsOfExperience);

                string expertiseLevel = GetConcreteDataPropertyValue(individual, "expertiseLevel");

                int cost;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                softwareProcess.Add(new HumanResource(IRI, name, description, cost, yearsOfExperience, expertiseLevel));
            }
        }
        private static void CreateInanimateResources(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                int cost;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                softwareProcess.Add(new InanimateResource(IRI, name, description, cost));
            }
        }
        private static void CreateInformations(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                int readability = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "readability"), out readability);

                int volatility = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "volatility"), out volatility);

                int priority = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "priority"), out priority);

                string language = GetConcreteDataPropertyValue(individual, "language");

                softwareProcess.Add(new Information(IRI, name, description, volatility, priority, language, readability));
            }
        }
        private static void CreateIntentions(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Intention(identifier:IRI, name:name, description:description));
            }
        }
        private static void CreateIssues(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                DateTime result;
                DateTime? occuredOn = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "occuredOn"), out result))
                    occuredOn = result;

                softwareProcess.Add(new Issue(IRI, name, description, occuredOn));
            }
        }
        private static void CreateLaws(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                int competenceLevel = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "competence_level"), out competenceLevel);

                string regulationCode = GetConcreteDataPropertyValue(individual, "regulationCode");
                softwareProcess.Add(new Law(IRI, name, description, competenceLevel, regulationCode));
            }
        }
        private static void CreateMaterials(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);


                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int volatility = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "volatility"), out volatility);

                int priority = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "priority"), out priority);

                string language = GetConcreteDataPropertyValue(individual, "language");

                softwareProcess.Add(new Material(IRI, name, description, volatility: volatility, priority: priority, cost: cost, language: language));
            }
        }
        private static void CreateParameters(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Parameter(IRI, name, description));
            }
        }
        private static void CreateProcesses(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                int acceptance = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "acceptance"), out acceptance);


                int performance = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "performance"), out performance);

                int reusability = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "reusability"), out reusability);

                DateTime result;
                DateTime? start = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "start"), out result))
                    start = result;

                DateTime? end = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "end"), out result))
                    end = result;

                string guard = GetConcreteDataPropertyValue(individual, "guard");

                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int timeEstimation = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "timeEstimation"), out timeEstimation);

                string location = GetConcreteDataPropertyValue(individual, "location");
                Boolean planned = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "planned_execution"), out planned);
                Boolean multiple = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "multiple_execution"), out multiple);
                Boolean optional = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "optional_execution"), out optional);

                softwareProcess.Add(new UPMM.Process(IRI, name, description, performance: performance, acceptance: acceptance, start: start, end: end, guard: guard, cost: cost, timeEstimation: timeEstimation, location: location,

plannedExecution: planned, multipleExecution: multiple, optionalExecution: optional, reusability: reusability));
            }
        }
        private static void CreateRoles(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                softwareProcess.Add(new Role(identifier:IRI, name:name, description:description));
            }
        }
        private static void CreateTasks(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);

                Boolean atomic = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "isAtomicStep"), out atomic);
                int priority = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "priority"), out priority);

                DateTime result;
                DateTime? start = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "start"), out result))
                    start = result;

                DateTime? end = null;
                if (DateTime.TryParse(GetConcreteDataPropertyValue(individual, "end"), out result))
                    end = result;

                string guard = GetConcreteDataPropertyValue(individual, "guard");

                int cost = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "cost"), out cost);

                int timeEstimation = 0;
                int.TryParse(GetConcreteDataPropertyValue(individual, "timeEstimation"), out timeEstimation);

                string location = GetConcreteDataPropertyValue(individual, "location");
                Boolean planned = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "planned_execution"), out planned);
                Boolean multiple = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "multiple_execution"), out multiple);
                Boolean optional = false;
                Boolean.TryParse(GetConcreteDataPropertyValue(individual, "optional_execution"), out optional);

                softwareProcess.Add(new UPMM.Task(IRI, name, description, isAtomicStep: atomic, priority: priority, start: start, end: end, guard: guard, cost: cost, timeEstimation: timeEstimation, location: location, plannedExecution:

planned, multipleExecution: multiple, optionalExecution: optional));
            }
        }




        private static void CreateAlternativeRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Alternative)).Select(x => x as UPMM.Alternative).FirstOrDefault();

                if (alternative == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "contributesTo"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.ProcessStep).FirstOrDefault();

                    if (alternative.ContributesTo.Contains(processStep))
                        continue;
                    else
                    {
                        alternative.ContributesTo.Add(processStep);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "respondsTo"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (alternative.RespondsTo.Contains(issue))
                        continue;
                    else
                    {
                        alternative.RespondsTo.Add(issue);
                        issue.HasResponse.Add(alternative);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isSelectedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (alternative.IsSelectedBy.Contains(role))
                        continue;
                    else
                    {
                        alternative.IsSelectedBy.Add(role);
                        role.Selects.Add(alternative);
                    }
                }


            }
        }
        private static void CreateArgumentRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Argument argument = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Argument)).Select(x => x as UPMM.Argument).FirstOrDefault();

                if (argument == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "objectsTo"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (argument.ObjectsTo.Contains(alternative))
                        continue;
                    else
                    {
                        argument.ObjectsTo.Add(alternative);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "supports"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (argument.Supports.Contains(alternative))
                        continue;
                    else
                    {
                        argument.Supports.Add(alternative);
                    }
                }


            }
        }
        private static void CreateArtifactRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Artifact artifact = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Artifact)).Select(x => x as UPMM.Artifact).FirstOrDefault();

                if (artifact == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (artifact.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        artifact.IsInContext.Add(context);
                        context.Scopes.Add(artifact);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (artifact.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        artifact.UsedIn.Add(process);
                        process.Uses.Add(artifact);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (artifact.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        artifact.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(artifact);
                    }
                }

                // ENTITY PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (artifact.IsProcessedBy.Contains(resource))
                        continue;
                    else
                    {
                        artifact.IsProcessedBy.Add(resource);
                        resource.Processes.Add(artifact);
                    }
                }

                //foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedIn"))
                //{
                //    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                //    if (artifact.IsProcessedIn.Contains(task))
                //        continue;
                //    else
                //    {
                //        artifact.IsProcessedIn.Add(task);
                //        task.Transforms.Add(artifact);
                //    }
                //}

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "resultsIn"))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();

                    if (artifact.ResultsIn.Contains(goal))
                        continue;
                    else
                    {
                        artifact.ResultsIn.Add(goal);
                        goal.HasResult.Add(artifact);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isReviewedBy"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (artifact.IsReviewedBy.Contains(issue))
                        continue;
                    else
                    {
                        artifact.IsReviewedBy.Add(issue);
                        issue.Reviews.Add(artifact);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "interactsWith"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (artifact.InteractsWith.Contains(entity))
                        continue;
                    else
                    {
                        artifact.InteractsWith.Add(entity);
                        entity.InteractsWith.Add(artifact);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifiedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (artifact.ModifiedBy.Contains(role))
                        continue;
                    else
                    {
                        artifact.ModifiedBy.Add(role);
                        role.Modifies.Add(artifact);
                    }
                }

                // ARTIFACT PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "partiallyConsistsOf"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (artifact.PartiallyConsistsOf.Contains(entity))
                        continue;
                    else
                    {
                        artifact.PartiallyConsistsOf.Add(entity);
                    }
                }
            }
        }
        private static void CreateCompetenceRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Competence competence = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Competence)).Select(x => x as UPMM.Competence).FirstOrDefault();

                if (competence == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProvidedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (competence.IsProvidedBy.Contains(resource))
                        continue;
                    else
                    {
                        competence.IsProvidedBy.Add(resource);
                        resource.Provides.Add(competence);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isSpecifiedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (competence.IsSpecifiedBy.Contains(role))
                        continue;
                    else
                    {
                        competence.IsSpecifiedBy.Add(role);
                        role.Specifies.Add(competence);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "checks"))
                {
                    UPMM.Law law = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Law).FirstOrDefault();

                    if (competence.Checks.Contains(law))
                        continue;
                    else
                    {
                        competence.Checks.Add(law);
                        law.IsCheckedBy.Add(competence);
                    }
                }



            }
        }
        private static void CreateContextRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Context context = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Context)).Select(x => x as UPMM.Context).FirstOrDefault();

                if (context == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "executes"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.ProcessStep).FirstOrDefault();

                    if (context.Executes.Contains(processStep))
                        continue;
                    else
                    {
                        context.Executes.Add(processStep);
                        processStep.IsBuildOn.Add(context);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "satisfies"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (context.Satisfies.Contains(intention))
                        continue;
                    else
                    {
                        context.Satisfies.Add(intention);
                        intention.IsSatisfiedBy.Add(context);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "scopes"))
                {
                    UPMM.Object obj = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Object).FirstOrDefault();

                    if (context.Scopes.Contains(obj))
                        continue;
                    else
                    {
                        context.Scopes.Add(obj);
                        obj.IsInContext.Add(context);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasSubContext"))
                {
                    UPMM.Context subContext = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (context.HasSubContext.Contains(subContext))
                        continue;
                    else
                    {
                        context.HasSubContext.Add(subContext);
                    }
                }

            }
        }
        private static void CreateCooperationsRelationShips(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);
            foreach (OWLNamedIndividual individual in individuals)
            {
                string IRI;
                string name;
                string description;
                GetBasicPropeties(individual, out IRI, out name, out description);
                string guard = GetConcreteDataPropertyValue(individual, "guard");
                
                string sourceIRI = GetConcreteObjectPropertyValues(individual, "hasSourceStep").FirstOrDefault();
                ProcessStep source = softwareProcess.Where(x => x.IRI == sourceIRI).Select(x => x as ProcessStep).FirstOrDefault();
                string targetIRI = GetConcreteObjectPropertyValues(individual, "hasTargetStep").FirstOrDefault();
                ProcessStep target = softwareProcess.Where(x => x.IRI == targetIRI).Select(x => x as ProcessStep).FirstOrDefault();
                
                CooperationType type = CooperationType.IsCoordinatedWith;
                if (GetConcreteObjectPropertyValues(individual, "isFollowedBy").FirstOrDefault() != null)
                    type = CooperationType.IsFollowedBy;
                else if (GetConcreteObjectPropertyValues(individual, "precedes").FirstOrDefault() != null)
                    type = CooperationType.IsPrecededBy;
                else if (GetConcreteObjectPropertyValues(individual, "isParallelWith").FirstOrDefault() != null)
                    type = CooperationType.IsParallelWith;
                else if (GetConcreteObjectPropertyValues(individual, "interrupts").FirstOrDefault() != null)
                    type = CooperationType.Interrupts;
                
                Cooperation cooperation = new Cooperation(type, IRI, name, description, guard, source, target);
                softwareProcess.Add(cooperation);
            }
        }
        private static void CreateDocumentRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Document document = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Document)).Select(x => x as UPMM.Document).FirstOrDefault();

                if (document == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (document.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        document.IsInContext.Add(context);
                        context.Scopes.Add(document);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (document.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        document.UsedIn.Add(process);
                        process.Uses.Add(document);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (document.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        document.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(document);
                    }
                }

                // ENTITY PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (document.IsProcessedBy.Contains(resource))
                        continue;
                    else
                    {
                        document.IsProcessedBy.Add(resource);
                        resource.Processes.Add(document);
                    }
                }

                //foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedIn"))
                //{
                //    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                //    if (document.IsProcessedIn.Contains(task))
                //        continue;
                //    else
                //    {
                //        document.IsProcessedIn.Add(task);
                //        task.Transforms.Add(document);
                //    }
                //}

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "resultsIn"))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();

                    if (document.ResultsIn.Contains(goal))
                        continue;
                    else
                    {
                        document.ResultsIn.Add(goal);
                        goal.HasResult.Add(document);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isReviewedBy"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (document.IsReviewedBy.Contains(issue))
                        continue;
                    else
                    {
                        document.IsReviewedBy.Add(issue);
                        issue.Reviews.Add(document);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "interactsWith"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (document.InteractsWith.Contains(entity))
                        continue;
                    else
                    {
                        document.InteractsWith.Add(entity);
                        entity.InteractsWith.Add(document);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifiedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (document.ModifiedBy.Contains(role))
                        continue;
                    else
                    {
                        document.ModifiedBy.Add(role);
                        role.Modifies.Add(document);
                    }
                }

                // ARTIFACT PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "partiallyConsistsOf"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (document.PartiallyConsistsOf.Contains(entity))
                        continue;
                    else
                    {
                        document.PartiallyConsistsOf.Add(entity);
                    }
                }
            }
        }
        private static void CreateEventRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Event eventUPMM = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Event)).Select(x => x as UPMM.Event).FirstOrDefault();

                if (eventUPMM == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "receiveSignal").Concat(GetConcreteObjectPropertyValues(individual, "isFiredBy")))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.ProcessStep).FirstOrDefault();

                    if (eventUPMM.ReceiveSignal.Contains(processStep))
                        continue;
                    else
                    {
                        eventUPMM.ReceiveSignal.Add(processStep);
                        if (processStep.GetType() == typeof(UPMM.Task))
                            (processStep as UPMM.Task).IsActivatedBy.Add(eventUPMM);

                        if (processStep.GetType() == typeof(UPMM.Process))
                            (processStep as UPMM.Process).IsActivatedBy.Add(eventUPMM);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasTarget"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (eventUPMM.HasTarget.Contains(intention))
                        continue;
                    else
                    {
                        eventUPMM.HasTarget.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasSource"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (eventUPMM.HasSource.Contains(intention))
                        continue;
                    else
                    {
                        eventUPMM.HasSource.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "raises"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (eventUPMM.Raises.Contains(issue))
                        continue;
                    else
                    {
                        eventUPMM.Raises.Add(issue);
                        issue.IsRaisedBy.Add(eventUPMM);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "decides"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (eventUPMM.Decides.Contains(alternative))
                        continue;
                    else
                    {
                        eventUPMM.Decides.Add(alternative);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isBuildOn"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (eventUPMM.IsBuildOn.Contains(context))
                        continue;
                    else
                    {
                        eventUPMM.IsBuildOn.Add(context);
                        context.Executes.Add(eventUPMM);
                    }
                }
            }
        }
        private static void CreateGoalRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Goal goal = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Goal)).Select(x => x as UPMM.Goal).FirstOrDefault();

                if (goal == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isRealizedBy"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (goal.IsRealizedBy.Contains(process))
                        continue;
                    else
                    {
                        goal.IsRealizedBy.Add(process);
                        process.Realizes.Add(goal);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isConcretizedBy"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (goal.IsConcretizedBy.Contains(intention))
                        continue;
                    else
                    {
                        goal.IsConcretizedBy.Add(intention);
                        intention.Concretizes.Add(goal);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResult"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (goal.HasResult.Contains(entity))
                        continue;
                    else
                    {
                        goal.HasResult.Add(entity);
                        entity.ResultsIn.Add(goal);
                    }
                }


                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isSatisfiedBy"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (goal.IsSatisfiedBy.Contains(context))
                        continue;
                    else
                    {
                        goal.IsSatisfiedBy.Add(context);
                        context.Satisfies.Add(goal);
                    }
                }


            }
        }
        private static void CreateGroupRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Group group = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Group)).Select(x => x as UPMM.Group).FirstOrDefault();

                if (group == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "consistsOf"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (group.ConsistsOf.Contains(role))
                        continue;
                    else
                    {
                        group.ConsistsOf.Add(role);
                        role.RoleIsIn.Add(group);
                    }
                }
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "specifies"))
                {
                    UPMM.Competence competence = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Competence).FirstOrDefault();

                    if (group.Specifies.Contains(competence))
                        continue;
                    else
                    {
                        group.Specifies.Add(competence);
                        competence.IsSpecifiedBy.Add(group);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "performs"))
                {
                    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                    if (group.Performs.Contains(task))
                        continue;
                    else
                    {
                        group.Performs.Add(task);
                        task.IsPerformedBy.Add(group);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "selects"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (group.Selects.Contains(alternative))
                        continue;
                    else
                    {
                        group.Selects.Add(alternative);
                        alternative.IsSelectedBy.Add(group);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "responsibleFor"))
                {
                    UPMM.Object obj = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Object).FirstOrDefault();

                    if (group.ResponsibleFor.Contains(obj))
                        continue;
                    else
                    {
                        group.ResponsibleFor.Add(obj);
                        obj.HasResponsibleRole.Add(group);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isPlayedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (group.IsPlayedBy.Contains(resource))
                        continue;
                    else
                    {
                        group.ResponsibleFor.Add(resource);
                        resource.PlaysRole.Add(group);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifies"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (group.Modifies.Contains(entity))
                        continue;
                    else
                    {
                        group.Modifies.Add(entity);
                        entity.ModifiedBy.Add(group);
                    }
                }
            }
        }
        private static void CreateHumanResourceRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.HumanResource humanResource = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.HumanResource)).Select(x => x as UPMM.HumanResource).FirstOrDefault();

                if (humanResource == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (humanResource.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        humanResource.IsInContext.Add(context);
                        context.Scopes.Add(humanResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (humanResource.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        humanResource.UsedIn.Add(process);
                        process.Uses.Add(humanResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (humanResource.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        humanResource.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(humanResource);
                    }
                }
                // RESOURCE PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "plays"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (humanResource.PlaysRole.Contains(role))
                        continue;
                    else
                    {
                        humanResource.PlaysRole.Add(role);
                        role.IsPlayedBy.Add(humanResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "provides"))
                {
                    UPMM.Competence competence = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Competence).FirstOrDefault();

                    if (humanResource.Provides.Contains(competence))
                        continue;
                    else
                    {
                        humanResource.Provides.Add(competence);
                        competence.IsProvidedBy.Add(humanResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "processes"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (humanResource.Processes.Contains(entity))
                        continue;
                    else
                    {
                        humanResource.Processes.Add(entity);
                        entity.IsProcessedBy.Add(humanResource);
                    }
                }

                // HR RELATIONSHIPS
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isOrganizedWith"))
                {
                    UPMM.HumanResource organizingHumanResource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.HumanResource).FirstOrDefault();

                    if (humanResource.IsOrganizedWith.Contains(organizingHumanResource))
                        continue;
                    else
                    {
                        humanResource.IsOrganizedWith.Add(organizingHumanResource);
                    }
                }
            }
        }
        private static void CreateInanimateResourceRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.InanimateResource inanimateResource = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.InanimateResource)).Select(x => x as UPMM.InanimateResource).FirstOrDefault();

                if (inanimateResource == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (inanimateResource.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        inanimateResource.IsInContext.Add(context);
                        context.Scopes.Add(inanimateResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (inanimateResource.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        inanimateResource.UsedIn.Add(process);
                        process.Uses.Add(inanimateResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (inanimateResource.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        inanimateResource.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(inanimateResource);
                    }
                }
                // RESOURCE PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "plays"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (inanimateResource.PlaysRole.Contains(role))
                        continue;
                    else
                    {
                        inanimateResource.PlaysRole.Add(role);
                        role.IsPlayedBy.Add(inanimateResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "provides"))
                {
                    UPMM.Competence competence = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Competence).FirstOrDefault();

                    if (inanimateResource.Provides.Contains(competence))
                        continue;
                    else
                    {
                        inanimateResource.Provides.Add(competence);
                        competence.IsProvidedBy.Add(inanimateResource);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "processes"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (inanimateResource.Processes.Contains(entity))
                        continue;
                    else
                    {
                        inanimateResource.Processes.Add(entity);
                        entity.IsProcessedBy.Add(inanimateResource);
                    }
                }
            }
        }
        private static void CreateInformationRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Information information = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Information)).Select(x => x as UPMM.Information).FirstOrDefault();

                if (information == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (information.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        information.IsInContext.Add(context);
                        context.Scopes.Add(information);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (information.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        information.UsedIn.Add(process);
                        process.Uses.Add(information);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (information.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        information.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(information);
                    }
                }

                // ENTITY PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (information.IsProcessedBy.Contains(resource))
                        continue;
                    else
                    {
                        information.IsProcessedBy.Add(resource);
                        resource.Processes.Add(information);
                    }
                }

                //foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedIn"))
                //{
                //    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                //    if (information.IsProcessedIn.Contains(task))
                //        continue;
                //    else
                //    {
                //        information.IsProcessedIn.Add(task);
                //        task.Transforms.Add(information);
                //    }
                //}

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "resultsIn"))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();
                    // testing goal to null because of bad inference in ontology (iteration plan setup)
                    if (goal == null || information.ResultsIn.Contains(goal))
                        continue;
                    else
                    {
                        information.ResultsIn.Add(goal);
                        goal.HasResult.Add(information);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isReviewedBy"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (information.IsReviewedBy.Contains(issue))
                        continue;
                    else
                    {
                        information.IsReviewedBy.Add(issue);
                        issue.Reviews.Add(information);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "interactsWith"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (information.InteractsWith.Contains(entity))
                        continue;
                    else
                    {
                        information.InteractsWith.Add(entity);
                        entity.InteractsWith.Add(information);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifiedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (information.ModifiedBy.Contains(role))
                        continue;
                    else
                    {
                        information.ModifiedBy.Add(role);
                        role.Modifies.Add(information);
                    }
                }
            }

        }
        private static void CreateIntentionRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Intention intention = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Intention)).Select(x => x as UPMM.Intention).FirstOrDefault();

                if (intention == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "concretizes"))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();

                    if (intention.Concretizes.Contains(goal))
                        continue;
                    else
                    {
                        intention.Concretizes.Add(goal);
                        goal.IsConcretizedBy.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isSatisfiedBy"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (intention.IsSatisfiedBy.Contains(context))
                        continue;
                    else
                    {
                        intention.IsSatisfiedBy.Add(context);
                        context.Satisfies.Add(intention);
                    }
                }


            }
        }
        private static void CreateIssueRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Issue issue = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Issue)).Select(x => x as UPMM.Issue).FirstOrDefault();

                if (issue == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isRaisedBy"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.ProcessStep).FirstOrDefault();

                    if (issue.IsRaisedBy.Contains(processStep))
                        continue;
                    else
                    {
                        issue.IsRaisedBy.Add(processStep);
                        processStep.Raises.Add(issue);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "reviews"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (issue.Reviews.Contains(entity))
                        continue;
                    else
                    {
                        issue.Reviews.Add(entity);
                        entity.IsReviewedBy.Add(issue);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponse"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (issue.HasResponse.Contains(alternative))
                        continue;
                    else
                    {
                        issue.HasResponse.Add(alternative);
                        alternative.RespondsTo.Add(issue);
                    }
                }
            }
        }
        private static void CreateLawRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Law law = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Law)).Select(x => x as UPMM.Law).FirstOrDefault();

                if (law == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "controls"))
                {
                    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                    if (law.Controls.Contains(task))
                        continue;
                    else
                    {
                        law.Controls.Add(task);
                        task.IsControlledBy.Add(law);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isCheckedBy"))
                {
                    UPMM.Competence competence = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Competence).FirstOrDefault();

                    if (law.IsCheckedBy.Contains(competence))
                        continue;
                    else
                    {
                        law.IsCheckedBy.Add(competence);
                        competence.Checks.Add(law);
                    }
                }
            }
        }
        private static void CreateMaterialRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Material material = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Material)).Select(x => x as UPMM.Material).FirstOrDefault();

                if (material == null)
                    continue;

                // OBJECT PROPERTIES
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isInContext"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (material.IsInContext.Contains(context))
                        continue;
                    else
                    {
                        material.IsInContext.Add(context);
                        context.Scopes.Add(material);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "usedIn"))
                {
                    UPMM.Process process = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (material.UsedIn.Contains(process))
                        continue;
                    else
                    {
                        material.UsedIn.Add(process);
                        process.Uses.Add(material);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasResponsibleRole"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (material.HasResponsibleRole.Contains(role))
                        continue;
                    else
                    {
                        material.HasResponsibleRole.Add(role);
                        role.ResponsibleFor.Add(material);
                    }
                }

                // ENTITY PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (material.IsProcessedBy.Contains(resource))
                        continue;
                    else
                    {
                        material.IsProcessedBy.Add(resource);
                        resource.Processes.Add(material);
                    }
                }

                //foreach (string iri in GetConcreteObjectPropertyValues(individual, "isProcessedIn"))
                //{
                //    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                //    if (material.IsProcessedIn.Contains(task))
                //        continue;
                //    else
                //    {
                //        material.IsProcessedIn.Add(task);
                //        task.Transforms.Add(material);
                //    }
                //}

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "resultsIn"))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();

                    if (material.ResultsIn.Contains(goal))
                        continue;
                    else
                    {
                        material.ResultsIn.Add(goal);
                        goal.HasResult.Add(material);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isReviewedBy"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (material.IsReviewedBy.Contains(issue))
                        continue;
                    else
                    {
                        material.IsReviewedBy.Add(issue);
                        issue.Reviews.Add(material);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "interactsWith"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (material.InteractsWith.Contains(entity))
                        continue;
                    else
                    {
                        material.InteractsWith.Add(entity);
                        entity.InteractsWith.Add(material);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifiedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (material.ModifiedBy.Contains(role))
                        continue;
                    else
                    {
                        material.ModifiedBy.Add(role);
                        role.Modifies.Add(material);
                    }
                }

                // ARTIFACT PROPERTIES

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "partiallyConsistsOf"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (material.PartiallyConsistsOf.Contains(entity))
                        continue;
                    else
                    {
                        material.PartiallyConsistsOf.Add(entity);
                    }
                }
            }
        }
        private static void CreateProcessRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Process process = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Process)).Select(x => x as UPMM.Process).FirstOrDefault();

                if (process == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isFollowedBy"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as ProcessStep).FirstOrDefault();
                    Cooperation coop = new Cooperation(CooperationType.IsFollowedBy, source: process, target: processStep);
                    int count = 0;
                    foreach (Cooperation cooperation in softwareProcess.Where(x => x is Cooperation).Select(x => x as Cooperation))
                    {
                        if (cooperation.Relation == coop.Relation && cooperation.Source == coop.Source && cooperation.Target == coop.Target)
                        {
                            count += 1;
                        }
                    }
                    if (count == 0)
                    {
                        softwareProcess.Add(coop);
                        softwareProcess.Add(new Cooperation(CooperationType.IsPrecededBy, source: processStep, target: process));
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "precedes"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as ProcessStep).FirstOrDefault();
                    Cooperation coop = new Cooperation(CooperationType.IsPrecededBy, source: process, target: processStep);
                    int count = 0;
                    foreach (Cooperation cooperation in softwareProcess.Where(x => x is Cooperation).Select(x => x as Cooperation))
                    {
                        if (cooperation.Relation == coop.Relation && cooperation.Source == coop.Source && cooperation.Target == coop.Target)
                        {
                            count += 1;
                        }
                    }
                    {
                        softwareProcess.Add(coop);
                        softwareProcess.Add(new Cooperation(CooperationType.IsFollowedBy, source: processStep, target: process));
                    }
                    
                }
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isParallelWith"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as ProcessStep).FirstOrDefault();
                    Cooperation coop = new Cooperation(CooperationType.IsParallelWith, source: process, target: processStep);
                    int count = 0;
                    foreach (Cooperation cooperation in softwareProcess.Where(x => x is Cooperation).Select(x => x as Cooperation))
                    {
                        if (cooperation.Relation == coop.Relation && cooperation.Source == coop.Source && cooperation.Target == coop.Target)
                        {
                            count += 1;
                        }
                    }
                    if (count == 0)
                    {
                        softwareProcess.Add(coop);
                        softwareProcess.Add(new Cooperation(CooperationType.IsParallelWith, source: processStep, target: process));
                    }
                    
                }
                foreach (string iri in GetConcreteObjectPropertyValues(individual, "interrupts"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as ProcessStep).FirstOrDefault();
                    Cooperation coop = new Cooperation(CooperationType.Interrupts, source: process, target: processStep);
                    int count = 0;
                    foreach (Cooperation cooperation in softwareProcess.Where(x => x is Cooperation).Select(x => x as Cooperation))
                    {
                        if (cooperation.Relation == coop.Relation && cooperation.Source == coop.Source && cooperation.Target == coop.Target)
                        {
                            count += 1;
                        }
                    }
                    if (count == 0)
                        softwareProcess.Add(coop);

                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "parentProcess"))
                {
                    UPMM.ProcessStep processStep = softwareProcess.Where(x => x.IRI == iri).Select(x => x as ProcessStep).FirstOrDefault();
                    if (processStep == null)
                        continue;
                    else
                    {
                        process.ParentProcess.Add(processStep);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "uses"))
                {
                    UPMM.Object obj = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Object).FirstOrDefault();

                    if (process.Uses.Contains(obj))
                        continue;
                    else
                    {
                        process.Uses.Add(obj);
                        obj.UsedIn.Add(process);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "realizes").Concat(GetConcreteObjectPropertyValues(individual, "demands")))
                {
                    UPMM.Goal goal = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Goal).FirstOrDefault();

                    if (process.Realizes.Contains(goal))
                        continue;
                    else
                    {
                        process.Realizes.Add(goal);
                        goal.IsRealizedBy.Add(process);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "collaborates"))
                {
                    UPMM.Process secondProcess = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Process).FirstOrDefault();

                    if (process.Collaborates.Contains(secondProcess))
                        continue;
                    else
                    {
                        process.Collaborates.Add(secondProcess);
                        secondProcess.Collaborates.Add(process);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isActivatedBy"))
                {
                    UPMM.Event eventUPMM = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Event).FirstOrDefault();

                    if (process.IsActivatedBy.Contains(eventUPMM))
                        continue;
                    else
                    {
                        process.IsActivatedBy.Add(eventUPMM);
                        eventUPMM.ReceiveSignal.Add(process);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "sendSignal"))
                {
                    UPMM.Event eventUPMM = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Event).FirstOrDefault();

                    if (process.SendSignal.Contains(eventUPMM))
                        continue;
                    else
                    {
                        process.SendSignal.Add(eventUPMM);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasTarget"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (process.HasTarget.Contains(intention))
                        continue;
                    else
                    {
                        process.HasTarget.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasSource"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (process.HasSource.Contains(intention))
                        continue;
                    else
                    {
                        process.HasSource.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "raises"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (process.Raises.Contains(issue))
                        continue;
                    else
                    {
                        process.Raises.Add(issue);
                        issue.IsRaisedBy.Add(process);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "decides"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (process.Decides.Contains(alternative))
                        continue;
                    else
                    {
                        process.Decides.Add(alternative);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isBuildOn"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (process.IsBuildOn.Contains(context))
                        continue;
                    else
                    {
                        process.IsBuildOn.Add(context);
                        context.Executes.Add(process);
                    }
                }


            }
        }
        private static void CreateRoleRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Role role = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Role)).Select(x => x as UPMM.Role).FirstOrDefault();

                if (role == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "specifies"))
                {
                    UPMM.Competence competence = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Competence).FirstOrDefault();

                    if (role.Specifies.Contains(competence))
                        continue;
                    else
                    {
                        role.Specifies.Add(competence);
                        competence.IsSpecifiedBy.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "performs"))
                {
                    UPMM.Task task = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                    if (role.Performs.Contains(task))
                        continue;
                    else
                    {
                        role.Performs.Add(task);
                        task.IsPerformedBy.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "selects"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (role.Selects.Contains(alternative))
                        continue;
                    else
                    {
                        role.Selects.Add(alternative);
                        alternative.IsSelectedBy.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "responsibleFor"))
                {
                    UPMM.Object obj = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Object).FirstOrDefault();

                    if (role.ResponsibleFor.Contains(obj))
                        continue;
                    else
                    {
                        role.ResponsibleFor.Add(obj);
                        obj.HasResponsibleRole.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isPlayedBy"))
                {
                    UPMM.Resource resource = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Resource).FirstOrDefault();

                    if (role.IsPlayedBy.Contains(resource))
                        continue;
                    else
                    {
                        role.ResponsibleFor.Add(resource);
                        resource.PlaysRole.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "modifies"))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (role.Modifies.Contains(entity))
                        continue;
                    else
                    {
                        role.Modifies.Add(entity);
                        entity.ModifiedBy.Add(role);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "roleIsIn"))
                {
                    UPMM.Group group = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Group).FirstOrDefault();

                    if (role.RoleIsIn.Contains(group))
                        continue;
                    else
                    {
                        role.RoleIsIn.Add(group);
                        group.ConsistsOf.Add(role);
                    }
                }

            }
        }
        private static void CreateTaskRelationships(OWLClass owlClass, List<SoftwareProcessElement> softwareProcess)
        {
            List<OWLNamedIndividual> individuals = GetIndividuals(owlClass);

            foreach (OWLNamedIndividual individual in individuals)
            {
                string individualIRI = individual.getIRI().toString();
                UPMM.Task task = softwareProcess.Where(x => x.IRI == individualIRI && x.GetType() == typeof(UPMM.Task)).Select(x => x as UPMM.Task).FirstOrDefault();

                if (task == null)
                    continue;

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasStep"))
                {
                    UPMM.Task subTask = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Task).FirstOrDefault();

                    if (task.HasSubStep.Contains(subTask))
                        continue;
                    else
                    {
                        task.HasSubStep.Add(subTask);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isControlledBy"))
                {
                    UPMM.Law law = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Law).FirstOrDefault();

                    if (task.IsControlledBy.Contains(law))
                        continue;
                    else
                    {
                        task.IsControlledBy.Add(law);
                        law.Controls.Add(task);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isPerformedBy"))
                {
                    UPMM.Role role = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Role).FirstOrDefault();

                    if (task.IsPerformedBy.Contains(role))
                        continue;
                    else
                    {
                        task.IsPerformedBy.Add(role);
                        role.Performs.Add(task);
                    }
                }


                foreach (string iri in (GetConcreteObjectPropertyValues(individual, "hasInput")))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (task.Input.Contains(entity))
                        continue;
                    else
                    {
                        task.Input.Add(entity);
                        entity.InputTo.Add(task);
                    }
                }

                foreach (string iri in (GetConcreteObjectPropertyValues(individual, "hasMandatoryInput")))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (task.MandatoryInput.Contains(entity))
                        continue;
                    else
                    {
                        task.MandatoryInput.Add(entity);
                        entity.MandatoryInputTo.Add(task);
                    }
                }

                foreach (string iri in (GetConcreteObjectPropertyValues(individual, "hasOptionalInput")))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (task.OptionalInput.Contains(entity))
                        continue;
                    else
                    {
                        task.OptionalInput.Add(entity);
                        entity.OptionalInputTo.Add(task);
                    }
                }

                foreach (string iri in (GetConcreteObjectPropertyValues(individual, "hasOutput")))
                {
                    UPMM.Entity entity = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Entity).FirstOrDefault();

                    if (task.Output.Contains(entity))
                        continue;
                    else
                    {
                        task.Output.Add(entity);
                        entity.OutputFrom.Add(task);
                    }
                }


                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isActivatedBy"))
                {
                    UPMM.Event eventUPMM = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Event).FirstOrDefault();

                    if (task.IsActivatedBy.Contains(eventUPMM))
                        continue;
                    else
                    {
                        task.IsActivatedBy.Add(eventUPMM);
                        eventUPMM.ReceiveSignal.Add(task);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "sendSignal").Concat(GetConcreteObjectPropertyValues(individual, "triggers")))
                {
                    UPMM.Event eventUPMM = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Event).FirstOrDefault();

                    if (task.SendSignal.Contains(eventUPMM))
                        continue;
                    else
                    {
                        task.SendSignal.Add(eventUPMM);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasTarget"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (task.HasTarget.Contains(intention))
                        continue;
                    else
                    {
                        task.HasTarget.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "hasSource"))
                {
                    UPMM.Intention intention = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Intention).FirstOrDefault();

                    if (task.HasSource.Contains(intention))
                        continue;
                    else
                    {
                        task.HasSource.Add(intention);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "raises"))
                {
                    UPMM.Issue issue = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Issue).FirstOrDefault();

                    if (task.Raises.Contains(issue))
                        continue;
                    else
                    {
                        task.Raises.Add(issue);
                        issue.IsRaisedBy.Add(task);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "decides"))
                {
                    UPMM.Alternative alternative = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Alternative).FirstOrDefault();

                    if (task.Decides.Contains(alternative))
                        continue;
                    else
                    {
                        task.Decides.Add(alternative);
                    }
                }

                foreach (string iri in GetConcreteObjectPropertyValues(individual, "isBuildOn"))
                {
                    UPMM.Context context = softwareProcess.Where(x => x.IRI == iri).Select(x => x as UPMM.Context).FirstOrDefault();

                    if (task.IsBuildOn.Contains(context))
                        continue;
                    else
                    {
                        task.IsBuildOn.Add(context);
                        context.Executes.Add(task);
                    }
                }
            }
        }
    }
}
