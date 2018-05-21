using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    [Serializable]
    public class UseCaseDiagramLinkData : UmlLinkData
    {
        private String _arrowHeadColor;

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
            xe.Add(XHelper.Attribute("ArrowHeadColor", this.ArrowHeadColor, "White"));
            return xe;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.ArrowHeadColor = XHelper.Read("ArrowHeadColor", e, "White");
            this.Points = XHelper.Read("Points", e, new List<System.Windows.Point> { });
        }

        public UseCaseDiagramLinkData() : this("", "", "Black", "White")
        {
        }

        public UseCaseDiagramLinkData(String category, String text, String color, String arrowHeadColor) : base(category, color, text)
        {
            ArrowHeadColor = arrowHeadColor;
        }
    }
}
