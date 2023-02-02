using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Helpers;
using ResumableFunction.Engine.InOuts;
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

        public DbSet<FunctionRuntimeInfo> FunctionRuntimeInfos { get; set; }
        public DbSet<Wait> Waits { get; set; }
        public DbSet<EventWait> EventWaits { get; set; }
        public DbSet<AllEventsWait> AllEventsWaits { get; set; }
        public DbSet<AnyEventWait> AnyEventWaits { get; set; }
        public DbSet<FunctionWait> FunctionWaits { get; set; }
        public DbSet<AllFunctionsWait> AllFunctionsWaits { get; set; }
        public DbSet<AnyFunctionWait> AnyFunctionWaits { get; set; }
        public DbSet<FunctionFolder> FunctionFolders { get; set; }
        public DbSet<TypeInfo> TypeInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FunctionRuntimeInfo>().ToTable("FunctionRuntimeInfos");
            modelBuilder.Entity<Wait>().ToTable("Waits");
            modelBuilder.Entity<EventWait>().ToTable("EventWaits");
            modelBuilder.Entity<AllEventsWait>().ToTable("AllEventsWaits");
            modelBuilder.Entity<AnyEventWait>().ToTable("AnyEventWaits");
            modelBuilder.Entity<FunctionWait>().ToTable("FunctionWaits");
            modelBuilder.Entity<AllFunctionsWait>().ToTable("AllFunctionsWaits");
            modelBuilder.Entity<AnyFunctionWait>().ToTable("AnyFunctionWaits");
            modelBuilder.Entity<TypeInfo>().ToTable("TypeInfos");
           
            
            
            modelBuilder.Entity<EventWait>()
                .Property(x => x.EventData)
                .HasConversion<ObjectToJsonConverter>();

            modelBuilder.Entity<FunctionRuntimeInfo>()
               .HasKey(x => x.FunctionId);
            modelBuilder.Entity<FunctionRuntimeInfo>()
                .Property(x => x.FunctionState)
                .HasConversion<ObjectToJsonConverter>();

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
    }
}
