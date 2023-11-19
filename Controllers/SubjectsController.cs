using DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : MyBaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await db.Set<Subject>().Select(x => new DTOSubject { Id = x.Id, Name = x.Name }).ToListAsync();
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
            subject.Id = sbj.Id;

            return CreatedAtAction(nameof(AddSubject), subject);
        }

        [HttpDelete("{id}")]
       public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await db.Set<Subject>().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (subject == null)
                return NotFound("We couldnt find the subject");

            db.Set<Subject>().Remove(subject);
            await db.SaveChangesAsync();

            return Ok(new DTOSubject { Id = subject.Id, Name = subject.Name});
        }
    }
}
