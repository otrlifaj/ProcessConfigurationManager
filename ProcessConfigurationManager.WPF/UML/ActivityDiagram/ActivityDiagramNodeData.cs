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
            : base(processElement, category)
        {
            if (category==Constants.UML_AD_SWIMLANE)
            {
                this.IsSubGraph = true;
            }
        }
    }
}
