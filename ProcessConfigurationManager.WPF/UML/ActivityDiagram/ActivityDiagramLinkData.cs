using System;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ActivityDiagramLinkData : UmlLinkData
    {
        
        public ActivityDiagramLinkData() : base()
        {

        }
        public ActivityDiagramLinkData(String guide=null, String category=null) : base(guide, category)
        {

        }

    }
}
