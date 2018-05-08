using Northwoods.GoXam.Model;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class UmlLinkData : GraphLinksModelLinkData<String, String>
    {
        private string _Color;

        public String Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    String old = _Color;
                    _Color = value;
                    RaisePropertyChanged("Color", old, value);
                }
            }
        }

        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("Color", this.Color, "Black"));
            return xe;
        }
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Color = XHelper.Read("Color", e, "Black");
        }

        public UmlLinkData() : base()
        {

        }

        public UmlLinkData(String category) : this()
        {
            Category = category;
        }
    }
}
