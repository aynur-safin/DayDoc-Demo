using DayDoc.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DayDoc.Web.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private static readonly string APP_DATA_DIR = "App_Data";
        private readonly string _dbPath;

        public AppDbContext(IWebHostEnvironment webHostEnvironment)
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);

            var path = Path.Combine(webHostEnvironment.ContentRootPath, APP_DATA_DIR, "db");
            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            _dbPath = Path.Join(path, "DayDoc.db");
            Database.EnsureCreated();
        }

        //static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder.AddConsole();
        //    builder.SetMinimumLevel(LogLevel.Warning);
        //});

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {            
            base.OnConfiguring(options);

            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={_dbPath}");

                // https://stackoverflow.com/questions/43680174/entity-framework-core-log-queries-for-a-single-db-context-instance
                // https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                options.LogTo(Console.WriteLine, LogLevel.Warning); /* 1 - нативный логинг через EF */
                //options.UseLoggerFactory(MyLoggerFactory); /* 2 - логинг через Microsoft.Extensions.Logging */
                //options.EnableSensitiveDataLogging(true); // !!! Позволяет включать данные приложения в сообщения об исключениях, логи и т. д. 

                options.ConfigureWarnings(warnings => {
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                });

                //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Company */
            {
                var conf = modelBuilder.Entity<Company>();

                conf.Property(m => m.CompType)
                    .HasDefaultValue(CompType.Customer);
            }

            /* Doc */
            {
                var conf = modelBuilder.Entity<Doc>();

                conf.HasOne(m => m.OwnCompany)
                    .WithMany()
                    .HasForeignKey(m => m.OwnCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                conf.HasOne(m => m.Contragent)
                    .WithMany()
                    .HasForeignKey(m => m.ContragentId)
                    .OnDelete(DeleteBehavior.Restrict);

                conf.Property(m => m.Sum)
                    .HasConversion<double>();
            }
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Doc> Docs { get; set; }
        public DbSet<XmlDoc> XmlDocs { get; set; }
    }
}
