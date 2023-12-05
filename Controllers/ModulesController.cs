using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;
using DTOModels;
using Microsoft.AspNetCore.Authorization;

namespace UddanelsesAPI.Controllers
{
    [Route("Subjects/{subjectid}/[controller]")]
    [ApiController]
    public class ModulesController : MyBaseController
    {
        public ModulesController(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Get a list of modules in a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        [HttpGet("/Subjects/{subjectid}/Modules")]
        public async Task<IActionResult> GetAllModules([FromRoute] Guid subjectid)
        {
            //Get Modules in a subject
            var modules = await dbmanager.GetAllModules(subjectid);
            //Turn them into DTO's
            var dtos = modules.Select(x => new DTOModule { Id = x.GUID, Name = x.Name });
            // Return them in a OK : 200
            return Ok(dtos);
        }

        /// <summary>
        /// Create a module under a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/Subjects/{subjectid}/Modules")]
        public async Task<IActionResult> CreateModuleUnderSubject([FromRoute] Guid subjectid, [FromBody] DTOModule module)
        {
            module.Name = module.Name.Trim();
            if (module.Name.Equals(String.Empty))
                return BadRequest("You need to give the module a name");

            var subject = await dbmanager.GetASubject(subjectid);
            if (subject == null)
                return NotFound("Couldnt find the subject");

            if(await dbmanager.CheckModuleName(subject.Id, module.Name))
                return BadRequest("The module name already exist");

            var mdl = await dbmanager.CreateModule(subject.Id, module);

            module.Id = mdl.GUID;
            return CreatedAtAction(nameof(CreateModuleUnderSubject), module);
        }

        /// <summary>
        /// Delete a module under a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="moduleid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> DeleteModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid)
        {
            var subject = await dbmanager.GetASubject(subjectid);
            if (subject == null)
                return NotFound("Couldnt find the subject");

            var module = await dbmanager.GetAModule(subject.Id, moduleid);
            if (module == null)
                return NotFound("Module not found");

            await dbmanager.DeleteModule(module);

            var dto = new DTOModule { Id = module.GUID, Name = module.Name };

            return Ok(dto);
        }
    }
}
