using ProcessConfigurationManager.UPMM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramNodeData : UmlNodeData
    {
        private List<String> _Attributes;

        public List<String> Attributes
        {
            get { return _Attributes; }
            set
            {
                if (_Attributes != value)
                {
                    List<String> old = _Attributes;
                    _Attributes = value;
                    RaisePropertyChanged("Attributes", old, value);
                }
            }
        }
        public ClassDiagramNodeData() : base()
        {
            _Attributes = new List<String>();
        }

        public ClassDiagramNodeData(SoftwareProcessElement processElement, String category)
            : base(processElement, category)
        {

            _Attributes = new List<String>();

            var type = processElement.GetType();
            var properties = type.GetProperties();

            List<String> ignoredProperties = new List<string> { "IRI", "Name", "Description", "Type" };
            var classSpecificProperties = properties.Where(p => !ignoredProperties.Contains(p.Name)).ToList();

            foreach (var p in classSpecificProperties)
            {
                _Attributes.Add(p.Name);  
            }
        }
    }
}
