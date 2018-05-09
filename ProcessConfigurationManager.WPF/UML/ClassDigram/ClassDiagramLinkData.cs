using Northwoods.GoXam.Model;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ClassDiagramLinkData : UmlLinkData
    {
        private String _fromText;
        private String _toText;

        public String FromText
        {
            get { return _fromText; }
            set
            {
                if (_fromText != value)
                {
                    String old = _fromText;
                    _fromText = value;
                    RaisePropertyChanged("FromText", old, value);
                }
            }
        }

        public String ToText
        {
            get { return _toText; }
            set
            {
                if (_toText != value)
                {
                    String old = _toText;
                    _toText = value;
                    RaisePropertyChanged("ToText", old, value);
                }
            }
        }

        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("FromText", this.Color, ""));
            xe.Add(XHelper.Attribute("ToText", this.Color, ""));
            return xe;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Color = XHelper.Read("FromText", e, "Black");
            this.Color = XHelper.Read("ToText", e, "Black");
        }

        public ClassDiagramLinkData() : base()
        {
            
        }

        public ClassDiagramLinkData(String category, String fromText, String toText) : base(category)
        {
            FromText = fromText;
            ToText = toText;
        }


    }
}
