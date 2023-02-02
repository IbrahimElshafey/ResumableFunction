using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.EfDataImplementation
{
    public class EngineDataContext : DbContext
    {
        public EngineDataContext(DbContextOptions<EngineDataContext> options)
       : base(options)
        {
        }

        public DbSet<FunctionRuntimeInfo> FunctionInfos { get; set; }
        public DbSet<EventWait> ActiveWaits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventWait>()
                .Property(x => x.EventData)
                .HasConversion<ObjectToJsonConverter>();

            modelBuilder.Entity<FunctionRuntimeInfo>()
               .HasKey(x => x.FunctionId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Expression>()
                .HaveConversion<ExpressionToJsonConverter>();
            configurationBuilder
               .Properties<LambdaExpression>()
               .HaveConversion<LambdaExpressionToJsonConverter>();
            configurationBuilder
                .Properties<Type>()
                .HaveConversion<TypeToStringConverter>();
        }

        //public static async Task InitializeAsync(EngineDataContext db)
        //{
        //    await db.Database.MigrateAsync();

        //    // already seeded
        //    if (db.Vehicles.Any())
        //        return;

        //    // sample data will be different due
        //    // to the nature of generating data
        //    var fake = new Faker<Vehicle>()
        //        .Rules((f, v) => v.VehicleIdentificationNumber = f.Vehicle.Vin())
        //        .Rules((f, v) => v.Model = f.Vehicle.Model())
        //        .Rules((f, v) => v.Type = f.Vehicle.Type())
        //        .Rules((f, v) => v.Fuel = f.Vehicle.Fuel());

        //    var vehicles = fake.Generate(100);

        //    db.Vehicles.AddRange(vehicles);
        //    await db.SaveChangesAsync();
        //}
    }
}
