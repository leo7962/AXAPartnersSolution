using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Common
{
    public class TestFixture : IDisposable
    {
        public ApplicationDbContext Context { get; private set; }

        public TestFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nombre único para cada test
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Agregar datos de prueba para departamentos
            if (!Context.Departments.Any())
            {
                Context.Departments.AddRange(
                    new Domain.Entities.Department { Name = "IT", Description = "Tecnología" },
                    new Domain.Entities.Department { Name = "HR", Description = "Recursos Humanos" }
                );
                Context.SaveChanges();
            }
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
