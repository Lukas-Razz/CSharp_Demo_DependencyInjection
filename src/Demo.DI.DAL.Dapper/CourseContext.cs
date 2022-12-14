using Dapper;
using System.Data;
using System.Text.RegularExpressions;

namespace Demo.DI.DAL.Dapper
{
    public class CourseContext
    {
        public IDbConnection DbConnection { get; }

        public CourseContext(IDbConnection connection)
        {
            DbConnection = connection;
        }

        public async Task CreateDatabase()
        {
            // Create Tables
            using (var transaction = DbConnection.BeginTransaction())
            {
                var tables = @"CREATE TABLE ""Courses"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_Courses"" PRIMARY KEY,
                    ""Name"" TEXT NOT NULL,
                    ""Start"" TEXT NOT NULL,
                    ""Location"" TEXT NOT NULL,
                    ""Contact"" TEXT NOT NULL
                );

                CREATE TABLE ""Enrollments"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_Enrollments"" PRIMARY KEY,
                    ""CourseId"" TEXT NOT NULL,
                    ""UserId"" TEXT NOT NULL,
                    ""ContactEmail"" TEXT NOT NULL,
                    ""EnrollmentTimestamp"" TEXT NOT NULL,
                    ""CanceledTimestamp"" TEXT NULL,
                    CONSTRAINT ""FK_Enrollments_Courses_CourseId"" FOREIGN KEY (""CourseId"") REFERENCES ""Courses"" (""Id"") ON DELETE CASCADE
                );

                CREATE INDEX ""IX_Enrollments_CourseId"" ON ""Enrollments"" (""CourseId"");";

                await DbConnection.ExecuteAsync(tables, transaction: transaction);
                transaction.Commit();
            }
        }
    }
}
