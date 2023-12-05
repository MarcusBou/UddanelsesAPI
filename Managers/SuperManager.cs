namespace UddanelsesAPI.Managers
{
    public class SuperManager
    {
        protected EducationContext db ;
        protected IConfiguration configuration;
        public SuperManager(IConfiguration configuration)
        {
            db = new EducationContext(configuration);
            this.configuration = configuration;
        }
    }
}
