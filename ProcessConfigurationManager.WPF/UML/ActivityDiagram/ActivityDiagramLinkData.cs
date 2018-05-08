using Northwoods.GoXam.Model;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class ActivityDiagramLinkData : UmlLinkData
    {
        private string _Guide;
        public String Guide
        {
            get { return _Guide; }
            set
            {
                if (_Guide != value)
                {
                    String old = _Guide;
                    _Guide = value;
                    RaisePropertyChanged("Guide", old, value);
                }
            }
        }

        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("Guide", this.Guide, ""));
            return xe;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Guide = XHelper.Read("Guide", e, "");
        }

        public ActivityDiagramLinkData() : base()
        {

        }
        public ActivityDiagramLinkData(String guide, String category) : base(category)
        {
            Guide = guide;
        }

    }
}
