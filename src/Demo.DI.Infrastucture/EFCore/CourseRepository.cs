using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Demo.DI.DAL.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.DI.Infrastructure
{
    public class CourseRepository : ICourseRepository
    {
        private CourseContext _context;
        private IMapper _mapper;

        public CourseRepository(CourseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Domain.Course>> GetAllAsync()
        {
            var courses = await _context.Courses.ToListAsync();

            return _mapper.Map<IEnumerable<Domain.Course>>(courses);
        }

        public async Task<Guid> CreateAsync(Domain.Course course)
        {
            var courseEntity = _mapper.Map<Course>(course);

            var entry = await _context.Courses.AddAsync(courseEntity);

            await _context.SaveChangesAsync();

            return entry.Entity.Id;
        }
    }
}