using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Helpers;
using ResumableFunction.Engine.InOuts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        public DbSet<ManyEventsWait> ManyEventsWaits { get; set; }
        public DbSet<FunctionWait> FunctionWaits { get; set; }
        public DbSet<ManyFunctionsWait> ManyFunctionsWaits { get; set; }
        public DbSet<FunctionFolder> FunctionFolders { get; set; }
        public DbSet<TypeInformation> TypeInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Wait>()
            //.HasDiscriminator(x => x.WaitType)
            //.HasValue<EventWait>(WaitType.EventWait)
            //.HasValue<ManyEventsWait>(WaitType.AllEventsWait);

            modelBuilder.Entity<FunctionFolder>().HasData(
                _functionFolders.Select(x => new FunctionFolder { Id = x.Length, Path = x }).ToArray());

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

        private string[] _functionFolders = new string[] {
            @"C:\ResumableFunction\Example.ProjectApproval\bin\Debug\net7.0"};

    }
}
