
namespace ProcessConfigurationManager.UPMM
{
    public class InanimateResource : Resource
    {
        public InanimateResource(string identifier = null, string name = null, string description = null, int cost = 0)
            : base(UPMMTypes.InanimateResource, identifier, name, description, cost)
        {

        }

        public InanimateResource() : base()
        {

        }
    }
}
