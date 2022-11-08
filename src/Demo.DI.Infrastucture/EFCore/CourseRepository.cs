using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Entities = Demo.DI.DAL.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Demo.DI.Domain;
using Demo.DI.Infrastucture.EFCore;

namespace Demo.DI.Infrastructure.EFCore
{
    public class CourseRepository : ICourseRepository
    {
        private IEFCoreUnitOfWork _uow;
        private IMapper _mapper;

        public CourseRepository(IEFCoreUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var courses = await _uow.Context.Courses.ToListAsync();

            return _mapper.Map<IEnumerable<Course>>(courses);
        }

        public async Task<Course> GetAsync(Guid courseId)
        {
            var course = await _uow.Context.Courses.FindAsync(courseId);

            return _mapper.Map<Course>(course);
        }

        public async Task<Guid> CreateAsync(Course course)
        {
            var courseEntity = _mapper.Map<Entities.Course>(course);

            var entry = await _uow.Context.Courses.AddAsync(courseEntity);

            await _uow.Context.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task<Course> GetByNameAsync(string courseName)
        {
            var course = await _uow.Context.Courses.Where(c => c.Name == courseName).FirstOrDefaultAsync();

            return _mapper.Map<Course>(course);
        }
    }
}