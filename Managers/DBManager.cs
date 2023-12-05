using DTOModels;
using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.DTOModels;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Managers
{
    public class DBManager : SuperManager
    {
        public DBManager(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            var subjects = await db.Set<Subject>().ToListAsync();
            return subjects;
        }

        public async Task<Subject?> GetASubject(Guid SubjectId)
        {
            var subjects = await db.Set<Subject>().Where(x => x.GUID == SubjectId).FirstOrDefaultAsync();
            return subjects;
        }

        public async Task<bool> CheckSubjectName(string name)
        {
            var IsExisting = await db.Set<Subject>().AnyAsync(x => x.Name.Equals(name));
            return IsExisting;
        }

        public async Task<Subject> CreateSubject(DTOSubject subject)
        {
            var sbj = new Subject { Name = subject.Name };
            await db.Set<Subject>().AddAsync(sbj);
            await db.SaveChangesAsync();

            return sbj;
        }

        public async Task DeleteSubject(Subject subject)
        {
            db.Set<Subject>().Remove(subject);
            await db.SaveChangesAsync();
        }


        public async Task<List<Module>> GetAllModules(Guid SubjectId)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == SubjectId)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id)
                        select module;
            var modules = await query.ToListAsync();
            return modules;
        }

        public async Task<Module?> GetAModule(int SubjectId, Guid ModuleId)
        {
            var Module = await db.Set<Module>().Where(x => x.GUID == ModuleId && x.SubjectId == SubjectId).FirstOrDefaultAsync();
            return Module;
        }

        public async Task<bool> CheckModuleName(int subjectid ,string name)
        {
            var IsExisting = await db.Set<Module>().AnyAsync(x => x.Name.Equals(name) && x.SubjectId == subjectid);
            return IsExisting;
        }

        public async Task<Module> CreateModule(int SubjectId, DTOModule Module)
        {
            var Mdl = new Module { Name = Module.Name, SubjectId = SubjectId };
            await db.Set<Module>().AddAsync(Mdl);
            await db.SaveChangesAsync();

            return Mdl;
        }

        public async Task DeleteModule(Module Module)
        {
            db.Set<Module>().Remove(Module);
            await db.SaveChangesAsync();
        }

        public async Task<List<Assignment>> GetAllAssignments(Guid SubjectId, Guid ModuleId)
        {
            var query = from subject in db.Set<Subject>().
                            Where(x => x.GUID == SubjectId)
                        from module in db.Set<Module>().
                            Where(x => x.SubjectId == subject.Id && x.GUID == ModuleId)
                        from assignment in db.Set<Assignment>().
                            Where(x => x.ModuleId == module.Id)
                        select assignment;
            var assignments = await query.ToListAsync();
            return assignments;
        }

        public async Task<Assignment?> GetAnAssignment(int ModuleId, Guid AssignmentId)
        {
            var Assignment = await db.Set<Assignment>().Where(x => x.GUID == AssignmentId && x.ModuleId == ModuleId).FirstOrDefaultAsync();
            return Assignment;
        }

        public async Task<bool> CheckAssignmentName(int subjectid, string name)
        {
            var IsExisting = await db.Set<Module>().AnyAsync(x => x.Name.Equals(name) && x.SubjectId == subjectid);
            return IsExisting;
        }

        public async Task<Assignment> CreateAssignment(int ModuleId, DTOAssignmentWithQA Assignment)
        {
            var Asm = new Assignment { Name = Assignment.Name, ModuleId = ModuleId, Answer = Assignment.Answer, Question = Assignment.Question, Type = Assignment.Type };
            await db.Set<Assignment>().AddAsync(Asm);
            await db.SaveChangesAsync();

            return Asm;
        }

        public async Task DeleteAssignment(Assignment Assignment)
        {
            db.Set<Assignment>().Remove(Assignment);
            await db.SaveChangesAsync();
        }
    }
}
