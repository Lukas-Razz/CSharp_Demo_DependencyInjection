using AutoMapper;
using Dapper;
using Demo.DI.BL.Contracts;
using Demo.DI.DAL.Dapper;
using Entities = Demo.DI.DAL.Dapper.Entities;
using Demo.DI.Domain;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Demo.DI.Infrastucture.Dapper
{
    public class CourseRepository : ICourseRepository
    {
        private IDapperUnitOfWork _uow;
        private IMapper _mapper;

        public CourseRepository(IDapperUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Course course)
        {
            var insertQuery = @"INSERT INTO Courses (Id, Name, Start, Location, Contact) VALUES (@Id, @Name, @Start, @Location, @Contact);";
            var newId = Guid.NewGuid();

            var connection = _uow.Transaction.Connection;

            await connection.ExecuteAsync(insertQuery, new
            {
                Id = newId,
                Name = course.Name,
                Start = course.Start,
                Location = course.Location,
                Contact = course.Contact,
            });

            return newId;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var selectQuery = @"SELECT Id, Name, Start, Location, Contact FROM Courses;";

            var connection = _uow.Transaction.Connection;

            var courses = await connection.QueryAsync<Entities.Course>(selectQuery);

            return _mapper.Map<IEnumerable<Course>>(courses);
        }
    }
}
