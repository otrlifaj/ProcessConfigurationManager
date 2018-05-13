using ProcessConfigurationManager.UPMM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramNodeData : UmlNodeData
    {
        private ObservableCollection<String> _Attributes;

        public ObservableCollection<String> Attributes
        {
            get { return _Attributes; }
            set
            {
                if (_Attributes != value)
                {
                    ObservableCollection<String> old = _Attributes;
                    _Attributes = value;
                    RaisePropertyChanged("Attributes", old, value);
                }
            }
        }

        public ClassDiagramNodeData() : base()
        {
            Attributes = new ObservableCollection<String>();
        }

        public ClassDiagramNodeData(SoftwareProcessElement processElement, String category)
            : base(processElement, category)
        {

            Attributes = new ObservableCollection<String>();

            var type = processElement.GetType();
            var properties = type.GetProperties();

            var ignoredProperties = new List<string> { "IRI", "Name", "Description", "Type" };
            var classSpecificProperties = properties.Where(p => !ignoredProperties.Contains(p.Name)).ToList();

            foreach (var attribute in classSpecificProperties.Select(p => p.Name))
            {
                Attributes.Add(attribute);
            }

            var parametersField = type.GetFields().FirstOrDefault(f => f.Name == "Parameters");
            if (parametersField != null)
            {
                List<Parameter> parameterList = parametersField.GetValue(processElement) as List<Parameter>;
                foreach (var parameter in parameterList.Select(p => p.Name))
                {
                    Attributes.Add(parameter);
                }
            }

        }
    }
}
