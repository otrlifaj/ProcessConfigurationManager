using Northwoods.GoXam.Model;
using System;
using System.Xml.Linq;

namespace ProcessConfigurationManager.WPF.UML
{
    public class UmlNodeData : GraphLinksModelNodeData<String>
    {
        private String _IRI = null;
        private String _Name = null;
        private String _Description = null;
        private String _Stereotype = null;
        private String _BorderColor = null;
        private int _Width;
        private int _Height;

        public int Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    Int32 old = _Width;
                    _Width = value;
                    RaisePropertyChanged("Width", old, value);
                }
            }
        }
        public int Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    Int32 old = _Height;
                    _Height = value;
                    RaisePropertyChanged("Height", old, value);
                }
            }
        }
        public String Color
        {
            get
            {
                switch (Stereotype)
                {
                    case "<<Process>>":
                        return "LightBlue";
                        break;
                    case "<<Task>>":
                        return "LightGreen";
                        break;
                    case "<<Alternative>>":
                        return "Cyan";
                        break;
                    case "<<Group>>":
                        return "LightGray";
                        break;
                    case "<<Role>>":
                        return "SkyBlue";
                        break;
                    case "<<Competence>>":
                        return "LightYellow";
                        break;
                    case "<<Law>>":
                        return "LightGreen";
                        break;
                    case "<<Issue>>":
                        return "PowderBlue";
                        break;
                    case "<<Event>>":
                        return "PeachPuff";
                        break;
                    case "<<Context>>":
                        return "LightGray";
                        break;
                    case "<<Goal>>":
                        return "YellowGreen";
                        break;
                    case "<<Intention>>":
                        return "SandyBrown";
                        break;
                    case "<<Argument>>":
                        return "Tomato";
                        break;
                    case "<<Object>>":
                        return "Teal";
                        break;
                    case "<<Entity>>":
                        return "Tan";
                        break;
                    case "<<Information>>":
                        return "Khaki";
                        break;
                    case "<<Artifact>>":
                        return "Azure";
                        break;
                    case "<<Material>>":
                        return "Pink";
                        break;
                    case "<<Document>>":
                        return "SeaShell";
                        break;
                    case "<<Resource>>":
                        return "Sienna";
                        break;
                    case "<<HumanResource>>":
                        return "Beige";
                        break;
                    case "<<InanimateResource>>":
                        return "Orchid";
                        break;

                    default: return "White";

                }
            }
        }
        public String BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor != value)
                {
                    String old = _BorderColor;
                    _BorderColor = value;
                    RaisePropertyChanged("BorderColor", old, value);
                }
            }
        }
        public String IRI
        {
            get { return _IRI; }
            set
            {
                if (_IRI != value)
                {
                    String old = _IRI;
                    _IRI = value;
                    RaisePropertyChanged("IRI", old, value);
                }
            }
        }
        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    String old = _Name;
                    _Name = value;
                    RaisePropertyChanged("Name", old, value);
                }
            }
        }
        public String Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    String old = _Description;
                    _Description = value;
                    RaisePropertyChanged("Description", old, value);
                }
            }
        }


        public String Stereotype
        {
            get { return _Stereotype; }
            set
            {

                string newValue = "<<" + value.Replace("<<", "").Replace(">>", "") + ">>";
                if (_Stereotype != newValue && newValue != "<<>>")
                {
                    String old = _Stereotype;
                    _Stereotype = newValue;
                    RaisePropertyChanged("Stereotype", old, newValue);
                }
            }
        }



        public override XElement MakeXElement(XName n)
        {
            XElement xe = base.MakeXElement(n);
            xe.Add(XHelper.Attribute("IRI", this.IRI, ""));
            xe.Add(XHelper.Attribute("Name", this.Name, ""));
            xe.Add(XHelper.Attribute("Description", this.Description, ""));
            xe.Add(XHelper.Attribute("Stereotype", this.Stereotype, ""));
            xe.Add(XHelper.Attribute("BorderColor", this.BorderColor, "Black"));
            xe.Add(XHelper.Attribute("Width", this.Width, 400));
            xe.Add(XHelper.Attribute("Height", this.Height, 600));
            return xe;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.IRI = XHelper.Read("IRI", e, "");
            this.Name = XHelper.Read("Name", e, "");
            this.Description = XHelper.Read("Description", e, "");
            this.Stereotype = XHelper.Read("Stereotype", e, "");
            this.BorderColor = XHelper.Read("BorderColor", e, "Black");
            this.Width = XHelper.Read("Width", e, 400);
            this.Height = XHelper.Read("Height", e, 400);
        }

        public UmlNodeData()
        {

        }
    }
}
