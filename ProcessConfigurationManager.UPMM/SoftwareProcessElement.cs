using System;

namespace ProcessConfigurationManager.UPMM
{
    public abstract class SoftwareProcessElement
    {
        public string IRI { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UPMMTypes Type { get; set; }
        public SoftwareProcessElement()
        {

        }
        public SoftwareProcessElement(UPMMTypes type, string identifier=null, string name=null, string description=null)
        {
            IRI = identifier;
            Name = name;
            Description = description;
            Type = type;
        }

        public override string ToString()
        {
            return String.Format("Name: {1}, Class: {2}", IRI, Name, GetUPMMType());
        }

        public string GetUPMMType()
        {
            return this.GetType().Name;
        }
    }
}
