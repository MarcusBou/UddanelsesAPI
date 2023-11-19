using DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : MyBaseController
    {
        [HttpGet("/api/Modules/{moduleid}")]
        public async Task<IActionResult> GetAllAssignmentsUnderModule([FromRoute] int moduleid)
        {
            var assignments = await db.Set<Assignment>().Where(x => x.ModuleId == moduleid).ToListAsync();
            return Ok(assignments);
        }

        [HttpGet("/api/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> GetAnAssignment([FromRoute] int subjectid, [FromRoute] int moduleid, [FromRoute] int assignmentid)
        {
            var assignment = await db.Set<Assignment>().
                                Where(x => x.ModuleId == moduleid && x.Id == assignmentid).
                                Select(x => new DTOAssignment { Id = x.Id, Name = x.Name }).
                                FirstOrDefaultAsync();
            if (assignment == null)
                return NotFound("Assignment not found");
            return Ok(assignment);
        }

        [HttpPost("/api/Modules/{moduleid}")]
        public async Task<IActionResult> CreateAssignmentInModule([FromRoute] int moduleid, [FromBody] DTOAssignment assignment)
        {
            assignment.Name = assignment.Name.Trim();
            var module = await db.Set<Module>().Where(x => x.Id == moduleid).FirstOrDefaultAsync();
            if (module == null)
                return NotFound("Module couldnt be found");

            if (await db.Set<Assignment>().AnyAsync(x => x.ModuleId == module.Id && x.Name == assignment.Name))
                return BadRequest("Assignment with that name already exist");

            var asm = new Assignment { Name = assignment.Name, ModuleId = module.Id };
            await db.Set<Assignment>().AddAsync(asm);
            await db.SaveChangesAsync();

            assignment.Id = asm.Id;
            return CreatedAtAction(nameof(CreateAssignmentInModule), assignment);
        }

        [HttpDelete("/api/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> DeleteAnAssignment([FromRoute] int moduleid, [FromRoute] int assignmentid)
        {
            var assignment = await db.Set<Assignment>().Where(x => x.ModuleId == moduleid && x.Id == assignmentid).FirstOrDefaultAsync();
            if (assignment == null)
                return NotFound("Assignment not found");

            db.Set<Assignment>().Remove(assignment);
            await db.SaveChangesAsync();

            var dto = new DTOAssignment { Name = assignment.Name, Id = assignment.Id };
            return Ok(dto);
        }
    }
}
