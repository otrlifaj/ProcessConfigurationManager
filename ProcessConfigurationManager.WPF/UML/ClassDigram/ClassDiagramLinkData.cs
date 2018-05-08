using System;


namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramLinkData : UmlLinkData
    {
     
        public ClassDiagramLinkData() : base()
        {

        }

        public ClassDiagramLinkData(String category) : base(category)
        {

        }
    }
}
