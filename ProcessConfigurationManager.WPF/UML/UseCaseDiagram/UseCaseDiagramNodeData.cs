using ProcessConfigurationManager.UPMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class UseCaseDiagramNodeData : UmlNodeData
    {

        public UseCaseDiagramNodeData() : base()
        {
            
        }

        public UseCaseDiagramNodeData(SoftwareProcessElement processElement, String category)
            : base(processElement, category)
        {
            if (category == Constants.UML_UCD_SYSTEM)
            {
                this.IsSubGraph = true;
            }
        }

    }
}
