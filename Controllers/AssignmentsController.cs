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

        /// <summary>
        /// Get all the assignements under a module in a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="moduleid"></param>
        /// <returns></returns>
        [HttpGet("/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> GetAllAssignmentsUnderModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid)
        {
            //Get all assignments under a module in a subject
            var assignments = await dbmanager.GetAllAssignments(subjectid, moduleid);
            //Turn them into DTO's
            var dtos = assignments.Select(x => new DTOAssignment { Id = x.GUID, Name = x.Name });
            //Return the DTO's
            return Ok(dtos);
        }

        /// <summary>
        /// Get an assignement with the next assignment value stored
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="moduleid"></param>
        /// <param name="assignmentid"></param>
        /// <returns></returns>
        [HttpGet("/Subjects/{subjectid}/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> GetAnAssignment([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromRoute] Guid assignmentid)
        {
            //Get all assignments
            var assignments = await dbmanager.GetAllAssignments(subjectid, moduleid);

            //Get the specific assignment and prepare the DTO
            var asm = assignments.Where(x => x.GUID == assignmentid).
                Select(x => new DTOAssignmentWithQA { Id = x.GUID, Name = x.Name, Answer = x.Answer, Question = x.Question, Type = x.Type, Video = x.Video}).
                FirstOrDefault();
            if (asm == null)
                return NotFound("Assignment not found");

            //Find the assignment index
            var index = assignments.FindIndex(x => x.GUID == assignmentid);
            if (index < assignments.Count -1)
                //Get the next assignment after the one choosen
                asm.NextAssignmentId = assignments.Skip(index + +1).Select(x => x.GUID).FirstOrDefault();

            return Ok(asm);
        }


        /// <summary>
        /// Create an assignment under a module in a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="moduleid"></param>
        /// <param name="assignment"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/Subjects/{subjectid}/Modules/{moduleid}")]
        public async Task<IActionResult> CreateAssignmentInModule([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromBody] DTOAssignmentWithQA assignment)
        {
            var subject = await dbmanager.GetASubject(subjectid);
            if (subject == null)
                return NotFound("Couldnt find the subject");

            var module = await dbmanager.GetAModule(subject.Id, moduleid);
            if (module == null)
                return NotFound("Module not found");

            if (await dbmanager.CheckAssignmentName(module.Id, assignment.Name))
                return BadRequest("Assignment with that name already exist");

            var asm = await dbmanager.CreateAssignment(module.Id, assignment);

            assignment.Id = asm.GUID;

            return CreatedAtAction(nameof(CreateAssignmentInModule), assignment);
        }

        /// <summary>
        /// Delete an assigment under a module in a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <param name="moduleid"></param>
        /// <param name="assignmentid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("/Subjects/{subjectid}/Modules/{moduleid}/Assignments/{assignmentid}")]
        public async Task<IActionResult> DeleteAnAssignment([FromRoute] Guid subjectid, [FromRoute] Guid moduleid, [FromRoute] Guid assignmentid)
        {
            var subject = await dbmanager.GetASubject(subjectid);
            if (subject == null)
                return NotFound("Couldnt find the subject");

            var module = await dbmanager.GetAModule(subject.Id, moduleid);
            if (module == null)
                return NotFound("Module not found");

            var assignment = await dbmanager.GetAnAssignment(module.Id, assignmentid);
            if (assignment == null)
                return NotFound("Assignment not found");

            await dbmanager.DeleteAssignment(assignment);

            var dto = new DTOAssignment { Name = assignment.Name, Id = assignment.GUID };
            return Ok(dto);
        }
    }
}
