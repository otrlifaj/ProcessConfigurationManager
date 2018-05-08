using System;


namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramLinkData : UmlLinkData
    {
     
        public ClassDiagramLinkData() : base()
        {

        }

        public ClassDiagramLinkData(String guide=null, String category=null) : base(guide, category)
        {

        }
    }
}
