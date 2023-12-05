using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubjectsController : MyBaseController
    {
        public SubjectsController(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Get all subjects
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAllSubjects()
        {
            //Get all Subjects
            var subjects = await dbmanager.GetAllSubjects();
            //Convert Subjects to DTO's
            var dtos = subjects.Select(x => new DTOSubject { Id = x.GUID, Name = x.Name }).ToList();
            //Return in a OK : 200 code
            return Ok(dtos);
        }

        /// <summary>
        /// Add a subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> AddSubject(DTOSubject subject)
        {
            subject.Name = subject.Name.Trim();
            if (subject.Name.Equals(String.Empty))
                return BadRequest("You need to give the subject a name");
          
            if (await dbmanager.CheckSubjectName(subject.Name))
                return BadRequest("Name already exist");

            var sbj = await dbmanager.CreateSubject(subject);
            subject.Id = sbj.GUID;

            return CreatedAtAction(nameof(AddSubject), subject);
        }

        /// <summary>
        /// Delete a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{subjectid}")]
        public async Task<IActionResult> DeleteSubject(Guid subjectid)
        {
            var subject = await dbmanager.GetASubject(subjectid);
            if (subject == null)
                return NotFound("We couldnt find the subject");

            await dbmanager.DeleteSubject(subject);

            return Ok(new DTOSubject { Id = subject.GUID, Name = subject.Name});
        }
    }
}
