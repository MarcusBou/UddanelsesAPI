using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;
using DTOModels;

namespace UddanelsesAPI.Controllers
{
    [Route("api/Subjects/{subjectid}/[controller]")]
    [ApiController]
    public class ModulesController : MyBaseController
    {
        [HttpGet("/api/Subjects/{subjectid}/Modules")]
        public async Task<IActionResult> GetAllModules([FromRoute] Guid subjectid)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == subjectid)
                        select subject.Modules.Select(x => new DTOModule { Id = x.GUID, Name = x.Name });
            var modules = await query.ToListAsync();
            return Ok(modules);
        }

        [HttpPost("/api/Subjects/{subjectid}/Modules")]
        public async Task<IActionResult> CreateModuleUnderSubject([FromRoute] Guid subjectid, [FromBody] DTOModule module)
        {
            module.Name = module.Name.Trim();
            if (module.Name.Equals(String.Empty))
                return BadRequest("You need to give the module a name");

            var subject = await db.Set<Subject>().Where(x => x.GUID == subjectid).FirstOrDefaultAsync();
            if (subject == null)
                return NotFound("Couldnt find the subject");

            var IsExisting = await db.Set<Module>().AnyAsync(x => x.SubjectId == subject.Id && x.Name.Equals(module.Name));
            if(IsExisting)
                return BadRequest("The module name already exist");

            var mdl = new Module { Name = module.Name, SubjectId = subject.Id };
            await db.Set<Module>().AddAsync(mdl);
            await db.SaveChangesAsync();

            module.Id = mdl.GUID;
            return CreatedAtAction(nameof(CreateModuleUnderSubject), module);
        }

        [HttpDelete("/api/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> DeleteModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid)
        {
            var subject = await db.Set<Subject>().Where(x => x.GUID == subjectid).FirstOrDefaultAsync();
            if (subject == null)
                return NotFound("Couldnt find the subject");

            var module = await db.Set<Module>().Where(x => x.GUID == moduleid && x.SubjectId == subject.Id).FirstOrDefaultAsync();
            if (module == null)
                return NotFound("Module not found");

            db.Set<Module>().Remove(module);
            await db.SaveChangesAsync();

            var dto = new DTOModule { Id = module.GUID, Name = module.Name };

            return Ok(dto);
        }
    }
}
