using AutoMapper;
using Dapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.Dapper;
using Entities = Demo.DI.DAL.Dapper.Entities;
using Demo.DI.Domain;
using Optional.Unsafe;

namespace Demo.DI.Infrastucture.Dapper
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private IDapperUnitOfWork _uow;
        private IMapper _mapper;

        public EnrollmentRepository(IDapperUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Enrollment enrollment)
        {
            var insertQuery = @"INSERT INTO Enrollments 
                (Id, CourseId, UserId, ContactEmail, EnrollmentTimestamp, CanceledTimestamp)
                VALUES (@Id, @CourseId, @UserId, @ContactEmail, @EnrollmentTimestamp, @CanceledTimestamp);";
            var newId = Guid.NewGuid();

            var connection = _uow.Transaction.Connection;

            await connection.ExecuteAsync(insertQuery, new
            {
                Id = newId,
                CourseId = enrollment.Course.Id,
                UserId = enrollment.UserId,
                ContactEmail = enrollment.ContactEmail,
                EnrollmentTimestamp = enrollment.EnrollmentTimestamp,
                CanceledTimestamp = enrollment.CanceledTimestamp.ToNullable()
            });

            return newId;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var selectQuery = @"SELECT 
                E.Id, E.CourseId, E.UserId, E.ContactEmail, E.EnrollmentTimestamp, E.CanceledTimestamp,
                C.Id, C.Name, C.Start, C.Location, C.Contact
                FROM Enrollments AS E 
                JOIN Courses AS C ON 
                E.CourseId = C.Id;";

            var connection = _uow.Transaction.Connection;

            var courses = await connection.QueryAsync<Entities.Enrollment, Entities.Course, Entities.Enrollment>
                (selectQuery, (enrollment, course) =>
                {
                    enrollment.Course = course;
                    return enrollment;
                });

            return _mapper.Map<IEnumerable<Enrollment>>(courses);
        }

        public async Task<Guid> UpdateAsync(Enrollment enrollment)
        {
            var updateQuery = @"UPDATE Enrollments SET 
                CourseId = @CourseId,
                UserId = @UserId,
                ContactEmail = @ContactEmail,
                EnrollmentTimestamp = @EnrollmentTimestamp,
                CanceledTimestamp = @CanceledTimestamp,
                WHERE id = @Id;";

            var connection = _uow.Transaction.Connection;

            await connection.ExecuteAsync(updateQuery, new
            {
                CourseId = enrollment.Course.Id,
                UserId = enrollment.UserId,
                ContactEmail = enrollment.ContactEmail,
                EnrollmentTimestamp = enrollment.EnrollmentTimestamp,
                CanceledTimestamp = enrollment.CanceledTimestamp.ToNullable()
            });

            return enrollment.Id;
        }
    }
}
