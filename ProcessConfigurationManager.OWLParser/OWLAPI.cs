using System;
using System.Collections.Generic;
using System.Diagnostics;

using org.semanticweb.owlapi.model; // OWLOntologyManager
using org.semanticweb.owlapi.apibinding; // OWLManager
using org.semanticweb.owlapi.reasoner; // OWLReasoner
using org.semanticweb.owlapi.reasoner.structural; // StructuralReasonerFactory
using com.clarkparsia.pellet.owlapiv3; // PelletReasonerFactory

namespace ProcessConfigurationManager.OWLParser
{
    public static partial class OWLAPI
    {
        private static OWLOntologyManager Manager { get; set; }
        private static OWLReasoner Reasoner { get; set; }
        private static OWLReasoner PelletReasoner { get; set; }
        private static OWLDataFactory DataFactory { get; set; }
        private static IRI MetamodelIRI { get; set; }
        private static IRI ProfileIRI { get; set; }

        private static Dictionary<IRI, UPMM.UPMMTypes> IRIClassMap = new Dictionary<IRI, UPMM.UPMMTypes>
        #region ontology to c# classes dictionary
        {
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Action"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Activity"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Activity_Parameter"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Alternative"), UPMM.UPMMTypes.Alternative},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Argument"), UPMM.UPMMTypes.Argument},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Artifact"), UPMM.UPMMTypes.Artifact},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Competence"), UPMM.UPMMTypes.Competence},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Constraint"), UPMM.UPMMTypes.Law},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Context"), UPMM.UPMMTypes.Context},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Control"), UPMM.UPMMTypes.Law},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Cooperation"), UPMM.UPMMTypes.Cooperation},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Document"), UPMM.UPMMTypes.Document},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Entity"), UPMM.UPMMTypes.Entity},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Event"), UPMM.UPMMTypes.Event},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Function"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Goal"), UPMM.UPMMTypes.Goal},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Group"), UPMM.UPMMTypes.Group},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Guard"), UPMM.UPMMTypes.Cooperation},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Human_Resource"), UPMM.UPMMTypes.HumanResource},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Inanimate_Resource"), UPMM.UPMMTypes.InanimateResource},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Information"), UPMM.UPMMTypes.Information},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Intention"), UPMM.UPMMTypes.Intention},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Issue"), UPMM.UPMMTypes.Issue},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Law"), UPMM.UPMMTypes.Law},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Material"), UPMM.UPMMTypes.Material},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Mechanism"), UPMM.UPMMTypes.Role},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Object"), UPMM.UPMMTypes.Object},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Object_Characteristic"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Object_Parameter"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Organization"), UPMM.UPMMTypes.Context},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Parameter"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Phase"), UPMM.UPMMTypes.Process},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Problem"), UPMM.UPMMTypes.Issue},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Process"), UPMM.UPMMTypes.Process},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Process_Argument"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Process_Parameter"), UPMM.UPMMTypes.Parameter},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Process_Role"), UPMM.UPMMTypes.Role},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Process_Step"), UPMM.UPMMTypes.ProcessStep},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Product_Part"), UPMM.UPMMTypes.Goal},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Resource"), UPMM.UPMMTypes.Resource},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Responsibility"), UPMM.UPMMTypes.Role},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Role"), UPMM.UPMMTypes.Role},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Scope"), UPMM.UPMMTypes.Context},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#step"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Step"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Task"), UPMM.UPMMTypes.Task},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Team"), UPMM.UPMMTypes.Group},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Work_Definition"), UPMM.UPMMTypes.ProcessStep},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Work_Product"), UPMM.UPMMTypes.Goal},
            {IRI.create("http://www.kosinar.me/ontologies/UnifiedProcessMetaModel#Work_Unit"), UPMM.UPMMTypes.ProcessStep},
        };
        #endregion


        public static void Initialize(string ontologyProfilePath)
        {
            try
            {
                Manager = OWLManager.createOWLOntologyManager();
                //tento řádek je potřebný kvůli nedokonale převedené knihovně z javy, kvůli které se načítá owl parser
                com.sun.org.apache.xerces.@internal.jaxp.SAXParserFactoryImpl s = new com.sun.org.apache.xerces.@internal.jaxp.SAXParserFactoryImpl();


                // načtení owl souboru, který obsahuje metamodel a který každá vložená báze individuí importuje
                byte[] owlResourceUPMM = Properties.Resources.UPMM;
                java.io.ByteArrayInputStream upmmInputStream = new java.io.ByteArrayInputStream(owlResourceUPMM);
                OWLOntology ontologyUPMM = Manager.loadOntologyFromOntologyDocument(upmmInputStream);
                MetamodelIRI = ontologyUPMM.getOntologyID().getOntologyIRI();
                Trace.WriteLine("Loaded Metamodel Ontology : " + MetamodelIRI);

                // načtení owl báze konkrétního profilu
                java.io.File processModel = new java.io.File(ontologyProfilePath);
                OWLOntology knowledgeProfile = Manager.loadOntologyFromOntologyDocument(processModel);
                ProfileIRI = knowledgeProfile.getOntologyID().getOntologyIRI();
                Trace.WriteLine("Loaded Profile Ontology : " + ProfileIRI);


                // vytvoření reasoneru nad profilem
                Reasoner = new StructuralReasonerFactory().createReasoner(knowledgeProfile, new SimpleConfiguration());
                PelletReasoner = new PelletReasonerFactory().createReasoner(knowledgeProfile, new SimpleConfiguration());
                //Reasoner = new Reasoner.ReasonerFactory().createReasoner(knowledgeProfile); HermiT
                Trace.WriteLine("Reasoner is running!");

                DataFactory = Manager.getOWLDataFactory();

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ontology loading failed!", ex);
            }


        }
        public static List<UPMM.SoftwareProcessElement> GetSoftwareProcess()
        {
            try
            {
                Trace.WriteLine("Begining UPMM tranformation...");
                List<UPMM.SoftwareProcessElement> softwareProcess = new List<UPMM.SoftwareProcessElement>();
                Transform(GetClasses(), softwareProcess);
                Trace.WriteLine("Process transformation done...");
                
                return softwareProcess;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Process transformation failed!", ex);
            }
        }
    }
}
