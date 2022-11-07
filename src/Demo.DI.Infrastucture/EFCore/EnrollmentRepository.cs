using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Entities = Demo.DI.DAL.EFCore.Entities;
using Demo.DI.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.DI.Infrastructure
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private CourseContext _context;
        private IMapper _mapper;

        public EnrollmentRepository(CourseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var enrollments = await _context.Enrollments.ToListAsync();

            return _mapper.Map<IEnumerable<Enrollment>>(enrollments);
        }

        public async Task<Guid> CreateAsync(Enrollment enrollment)
        {
            var enrollmentEntity = _mapper.Map<Entities.Enrollment>(enrollment);

            var entry = await _context.Enrollments.AddAsync(enrollmentEntity);

            await _context.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task<Guid> UpdateAsync(Enrollment enrollment)
        {
            var enrollmentEntity = _mapper.Map<Entities.Enrollment>(enrollment);

            var entry = _context.Enrollments.Update(enrollmentEntity);

            await _context.SaveChangesAsync();

            return entry.Entity.Id;
        }
    }
}
