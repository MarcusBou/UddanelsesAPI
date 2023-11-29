using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UddanelsesAPI
{
    public class MyBaseController : ControllerBase
    {
        protected EducationContext db { get; set; }
        public MyBaseController(IConfiguration configuration)
        {
            db = new EducationContext(configuration);
        }
    }
}
