using Northwoods.GoXam.Model;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF
{
    [Serializable]
    public class ActivityDiagramLinkData : GraphLinksModelLinkData<String, String>
    {
        private string _Guide;
        private string _Color;
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
        public ActivityDiagramLinkData() : base()
        {

        }
        public ActivityDiagramLinkData(String guide=null, String category=null) : this()
        {
            Guide = guide;
            Category = category;
        }

        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("Guide", this.Guide, ""));
            xe.Add(XHelper.Attribute("Color", this.Color, "Black"));
            return xe;
        }
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Guide = XHelper.Read("Guide", e, "");
            this.Color = XHelper.Read("Color", e, "Black");
        }
    }
}
