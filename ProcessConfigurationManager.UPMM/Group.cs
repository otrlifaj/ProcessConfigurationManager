using System.Collections.Generic;

namespace ProcessConfigurationManager.UPMM
{
    public class Group : Role
    {
        public List<Role> ConsistsOf;

        public Group(string identifier = null, string name = null, string description = null)
            : base(UPMMTypes.Group, identifier, name, description)
        {
            ConsistsOf = new List<Role>();
        }

        public Group() : base()
        {
            ConsistsOf = new List<Role>();
        }
    }
}
