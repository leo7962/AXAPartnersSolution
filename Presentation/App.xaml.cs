using Applications.Common.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.ViewModels;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        public App()
        {
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, context.Configuration);
                }).Build();

            await _host.StartAsync();

            // Initialize database with migrations
            using var scope = _host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Aplicar migraciones automáticamente
            await context.Database.MigrateAsync();

            await SeedInitialData(context);

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Applications.Users.Commands.CreateUserCommand).Assembly);
            });

            // Register ViewModels
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<DepartmentAssignmentViewModel>();
            services.AddTransient<UserAssignmentsViewModel>();
            services.AddSingleton<MainViewModel>();

            // Register Views
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
                _host.Dispose();
            }
            base.OnExit(e);
        }

        private async Task SeedInitialData(ApplicationDbContext context)
        {
            if (!await context.Departments.AnyAsync())
            {
                var departments = new[]
                {
                    new Domain.Entities.Department { Name = "Nomina", Description = "Departamento de nómina" },
                    new Domain.Entities.Department { Name = "Facturacion", Description = "Departamento de facturación" },
                    new Domain.Entities.Department { Name = "ServicioCliente", Description = "Servicio al cliente" },
                    new Domain.Entities.Department { Name = "IT", Description = "Departamento de tecnología" },
                    new Domain.Entities.Department { Name = "RecursosHumanos", Description = "Recursos humanos" },
                    new Domain.Entities.Department { Name = "Contabilidad", Description = "Departamento contable" }
                };

                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }
        }
    }
}
