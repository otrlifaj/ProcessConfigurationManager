using ProcessConfigurationManager.UPMM;
using System;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramNodeData : UmlNodeData
    {
        public ClassDiagramNodeData() : base()
        {
        }

        public ClassDiagramNodeData(SoftwareProcessElement processElement, String category)
            : this()
        {
            IRI = processElement.IRI;
            Name = processElement.Name;
            Description = processElement.Description;
            Stereotype = processElement.GetUPMMType();
            BorderColor = "Black";

            Category = category;
            Key = processElement.Name + "-" + category;

            Width = 400;
            Height = 600;
        }
    }
}
