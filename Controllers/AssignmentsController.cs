using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.DTOModels;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AssignmentsController : MyBaseController
    {
        public AssignmentsController(IConfiguration configuration) : base(configuration)
        {
            
        }

        [HttpGet("/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> GetAllAssignmentsUnderModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == subjectid)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id && x.GUID == moduleid)
                        from assignment in db.Set<Assignment>().
                            Where(x => x.ModuleId == module.Id)
                        select new DTOAssignment { Id = assignment.GUID, Name = assignment.Name };
            var assignments = await query.ToListAsync();
            return Ok(assignments);
        }

        [HttpGet("/Subjects/{subjectid}/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> GetAnAssignment([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromRoute] Guid assignmentid)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == subjectid)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id && x.GUID == moduleid)
                        from assignment in db.Set<Assignment>().
                            Where(x => x.ModuleId == module.Id)
                        select new DTOAssignmentWithQA { Id = assignment.GUID, Name = assignment.Name, Answer = assignment.Answer, Question = assignment.Question, Type = assignment.Type, Video = assignment.Video };
            var assignments = await query.ToListAsync();
            var asm = assignments.Where(x => x.Id == assignmentid).FirstOrDefault();
            if (asm == null)
                return NotFound("Assignment not found");
            var index = assignments.FindIndex(x => x.Id == assignmentid);
            if (index < assignments.Count -1)
                asm.NextAssignmentId = assignments.Skip(index + +1).Select(x => x.Id).FirstOrDefault();
            return Ok(asm);
        }


        [Authorize]
        [HttpPost("/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> CreateAssignmentInModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromBody] DTOAssignmentWithQA assignment)
        {
            assignment.Name = assignment.Name.Trim();
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == subjectid)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id && x.GUID == moduleid)
                        select module;
            var mdl = query.FirstOrDefault();
            if (mdl == null)
                return NotFound("Module couldnt be found");

            if (await db.Set<Assignment>().AnyAsync(x => x.ModuleId == mdl.Id && x.Name == assignment.Name))
                return BadRequest("Assignment with that name already exist");

            var asm = new Assignment { 
                Name = assignment.Name, 
                ModuleId = mdl.Id, 
                Answer = assignment.Answer,
                Question = assignment.Question,
                Video = assignment.Video,
                Type = assignment.Type,
            };
            await db.Set<Assignment>().AddAsync(asm);
            await db.SaveChangesAsync();

            assignment.Id = asm.GUID;
            return CreatedAtAction(nameof(CreateAssignmentInModule), assignment);
        }


        [Authorize]
        [HttpDelete("/Subjects/{subjectid}/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> DeleteAnAssignment([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromRoute] Guid assignmentid)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == subjectid)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id && x.GUID == moduleid)
                        from assignment in db.Set<Assignment>().
                            Where(x => x.ModuleId == module.Id && x.GUID == assignmentid)
                        select assignment;
            var asm = await query.FirstOrDefaultAsync();
            if (asm == null)
                return NotFound("Assignment not found");

            db.Set<Assignment>().Remove(asm);
            await db.SaveChangesAsync();

            var dto = new DTOAssignment { Name = asm.Name, Id = asm.GUID };
            return Ok(dto);
        }
    }
}
