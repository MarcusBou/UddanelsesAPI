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

        [HttpGet("")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await db.Set<Subject>().Select(x => new DTOSubject { Id = x.GUID, Name = x.Name }).ToListAsync();
            return Ok(subjects);
        }

        
        [HttpPost("")]
        public async Task<IActionResult> AddSubject(DTOSubject subject)
        {
            subject.Name = subject.Name.Trim();
            if (subject.Name.Equals(String.Empty))
                return BadRequest("You need to give the subject a name");

            var IsExisting = await db.Set<Subject>().AnyAsync(x => x.Name.Equals(subject.Name));
            if (IsExisting)
                return BadRequest("Name already exist");

            var sbj = new Subject { Name = subject.Name };
            await db.Set<Subject>().AddAsync(sbj);
            await db.SaveChangesAsync();
            subject.Id = sbj.GUID;

            return CreatedAtAction(nameof(AddSubject), subject);
        }

        [HttpDelete("{subjectid}")]
       public async Task<IActionResult> DeleteSubject(Guid subjectid)
        {
            var subject = await db.Set<Subject>().Where(x => x.GUID == subjectid).FirstOrDefaultAsync();
            if (subject == null)
                return NotFound("We couldnt find the subject");

            db.Set<Subject>().Remove(subject);
            await db.SaveChangesAsync();

            return Ok(new DTOSubject { Id = subject.GUID, Name = subject.Name});
        }
    }
}
