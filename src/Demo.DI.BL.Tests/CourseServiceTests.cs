using Demo.DI.BL.Contracts;
using Demo.DI.BL.Services;
using Demo.DI.Domain;
using FluentAssertions;

namespace Demo.DI.BL.Tests
{
    public class CourseServiceTests
    {
        Mock<ICourseRepository> _courseRepositoryMock;

        public CourseServiceTests()
        {
            // Creation of a mock
            // It essentialy implements the interface, so everything works.
            _courseRepositoryMock = new Mock<ICourseRepository>();
        }

        [Theory]
        [InlineData("bad")]
        [InlineData("bad.mailerus")]
        [InlineData("@error")]
        public async Task EnlistCourse_InvalidEmail_Throws(string mail)
        {
            var service = new CourseService(_courseRepositoryMock.Object);

            var action = () => service.EnlistCourse("name", "location", mail, new DateTime());

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task EnlistCourse_Valid_ReturnsCourse()
        {
            var expectedGuid = Guid.NewGuid();
            var expected = new Course
            {
                Id = expectedGuid,
                Name = "name",
                Location = "location",
                Contact = "email@example.test",
                Start = new DateTime(),
            };

            _courseRepositoryMock
                // This sets-up the depoendency
                // It is compleately under our control
                // Note the ".Result" at the end. This is for async methods
                .Setup(x => x.CreateAsync(It.IsAny<Course>()).Result)
                .Returns(expectedGuid);
            _courseRepositoryMock
                .Setup(x => x.GetAsync(expectedGuid).Result)
                .Returns(expected);

            var service = new CourseService(_courseRepositoryMock.Object);

            var actual = await service.EnlistCourse("name", "location", "email@example.test", new DateTime());

            actual.Should().Be(expected);

            // Verify that the method was called as expected
            _courseRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Course>()), Times.Once());
        }

        [Fact]
        public async Task GetAvailableCourses_Existing_ReturnsFilteredLocations()
        {
            var expected = Guid.NewGuid();
            _courseRepositoryMock
                .Setup(x => x.GetAllAsync().Result)
                .Returns(new Course[]
                {
                    new Course
                    {
                        Id = Guid.NewGuid(),
                        Name = "name",
                        Location = "location",
                        Contact = "email@example.test",
                        Start = DateTime.UtcNow.AddDays(1),
                    },
                    new Course
                    {
                        Id = Guid.NewGuid(),
                        Name = "name",
                        Location = "location",
                        Contact = "email@example.test",
                        Start = DateTime.UtcNow.AddDays(1),
                    },
                    new Course
                    {
                        Id = Guid.NewGuid(),
                        Name = "name",
                        Location = "location_not",
                        Contact = "email@example.test",
                        Start = DateTime.UtcNow.AddDays(1),
                    },
                });
            var service = new CourseService(_courseRepositoryMock.Object);

            var actual = await service.GetAvailableCourses("location");

            actual.Should().HaveCount(2, "because we there are two Courses with right location and start date is the same");
        }

    }
}
