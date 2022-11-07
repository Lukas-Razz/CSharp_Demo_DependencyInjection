using AutoMapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.EFCore;
using Entities = Demo.DI.DAL.EFCore.Entities;
using Demo.DI.Domain;
using Microsoft.EntityFrameworkCore;
using Demo.DI.Infrastucture.EFCore;

namespace Demo.DI.Infrastructure.EFCore
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private IEFCoreUnitOfWork _uow;
        private IMapper _mapper;

        public EnrollmentRepository(IEFCoreUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var enrollments = await _uow.Context.Enrollments.ToListAsync();

            return _mapper.Map<IEnumerable<Enrollment>>(enrollments);
        }

        public async Task<Guid> CreateAsync(Enrollment enrollment)
        {
            var enrollmentEntity = _mapper.Map<Entities.Enrollment>(enrollment);

            var entry = await _uow.Context.Enrollments.AddAsync(enrollmentEntity);

            await _uow.Context.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task<Guid> UpdateAsync(Enrollment enrollment)
        {
            var enrollmentEntity = _mapper.Map<Entities.Enrollment>(enrollment);

            var entry = _uow.Context.Enrollments.Update(enrollmentEntity);

            await _uow.Context.SaveChangesAsync();

            return entry.Entity.Id;
        }
    }
}
