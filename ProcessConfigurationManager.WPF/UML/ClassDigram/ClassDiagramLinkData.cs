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
        private String _arrowHeadColor;

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

        public String ArrowHeadColor
        {
            get { return _arrowHeadColor; }
            set
            {
                if (_arrowHeadColor != value)
                {
                    String old = _arrowHeadColor;
                    _arrowHeadColor = value;
                    RaisePropertyChanged("ArrowHeadColor", old, value);
                }
            }
        }

        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("FromText", this.FromText, ""));
            xe.Add(XHelper.Attribute("ToText", this.ToText, ""));
            xe.Add(XHelper.Attribute("ArrowHeadColor", this.ArrowHeadColor, "White"));
            return xe;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.FromText = XHelper.Read("FromText", e, "");
            this.ToText = XHelper.Read("ToText", e, "");
            this.ArrowHeadColor = XHelper.Read("ArrowHeadColor", e, "White");
        }

        public ClassDiagramLinkData() : this("", "", "", "", "Black", "White")
        {
            
        }


        public ClassDiagramLinkData(String category, String fromText, String toText, String text, String color = "Black", String arrowHeadColor = "White") : base(category, color, text)
        {
            FromText = fromText;
            ToText = toText;
            ArrowHeadColor = arrowHeadColor;
        }


    }
}
