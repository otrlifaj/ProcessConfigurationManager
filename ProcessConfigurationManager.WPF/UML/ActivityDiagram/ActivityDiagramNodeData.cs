using Northwoods.GoXam.Model;
using ProcessConfigurationManager.UPMM;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ActivityDiagramNodeData : UmlNodeData
    {

        public ActivityDiagramNodeData() : base()
        {
        }

        public ActivityDiagramNodeData(SoftwareProcessElement processElement, String category)
            : this()
        {
            IRI = processElement.IRI;
            Name = processElement.Name;
            Description = processElement.Description;
            Stereotype = processElement.GetUPMMType();
            BorderColor = "Black";

            Category = category;
            Key = processElement.Name + "-" + category;

            if (category=="Swimlane")
            {
                this.IsSubGraph = true;
            }
            Width = 400;
            Height = 600;
        }
    }
}
