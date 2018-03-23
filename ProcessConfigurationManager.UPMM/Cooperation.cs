
namespace ProcessConfigurationManager.UPMM
{
    public class Cooperation : SoftwareProcessElement
    {
        public ProcessStep Source { get; set; }
        public ProcessStep Target { get; set; }
        public CooperationType Relation { get; set; }
        public string Guard;

        public Cooperation(string identifier = null, string name = null, string description = null, string guard = null, ProcessStep source = null, ProcessStep target = null)
            : base(UPMMTypes.Cooperation, identifier, name, description)
        {
            Guard = guard;
            Source = source;
            Target = target;
        }

        public Cooperation(CooperationType relation, string identifier = null, string name = null, string description = null, string guard = null, ProcessStep source = null, ProcessStep target = null)
            : this(identifier, name, description, guard, source, target)
        {
            Relation = relation;
        }

        public Cooperation() : base()
        {
            
        }
    }
}
