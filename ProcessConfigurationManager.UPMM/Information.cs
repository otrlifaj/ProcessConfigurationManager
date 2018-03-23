
namespace ProcessConfigurationManager.UPMM
{
    public class Information : Entity
    {
        public int Readability { get; set; }

        public Information(string identifier = null, string name = null, string description = null, int volatility = 0, int priority = 0, string language = null, int readability = 0)
            : base(UPMMTypes.Information, identifier, name, description, volatility, priority, language)
        {
            Readability = readability;
        }

        public Information() : base()
        {

        }
    }
}
