
namespace ProcessConfigurationManager.UPMM
{
    public class Parameter : SoftwareProcessElement
    {
        public Parameter(string identifier=null, string name=null, string description=null) : base(UPMMTypes.Parameter, identifier, name, description)
        {

        }

        public Parameter() : base()
        {

        }
    }
}
