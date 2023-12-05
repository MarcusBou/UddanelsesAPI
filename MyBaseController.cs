using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UddanelsesAPI.Managers;

namespace UddanelsesAPI
{
    public class MyBaseController : ControllerBase
    {
        protected DBManager dbmanager;
        public MyBaseController(IConfiguration configuration)
        {
            dbmanager = new DBManager(configuration);
        }
    }
}
