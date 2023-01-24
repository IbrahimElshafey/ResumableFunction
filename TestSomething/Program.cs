using System.Reflection;
using TestSomething.MayUse;
using ResumableFunction.Abstraction.Samples;
using ResumableFunction.Abstraction.WebApiProvider;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine;
using System.Runtime.InteropServices;
using ResumableFunction.Abstraction.InOuts;
using System.Text.Json;
using System.Dynamic;
using TestSomething;
using System.Linq.Expressions;

namespace Test
{
    public static class Program
    {
        public static ProjectApprovalFunctionData Data =>
            new ProjectApprovalFunctionData
            {
                Project = new ProjectRequestedEvent { Id = 11 },
                ManagerApprovalResult = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false }
            };
        static async Task Main(string[] args)
        {
            var wait = new SingleEventWait<ManagerApprovalEvent>("OwnerApproval")
                      .Match(result =>
                      result.ProjectId > Math.Min(5, 10) &&
                      result.ProjectId == Data.Project.Id)
                      //.Match(result => result.ProjectId == 11)
                      .SetProp(() => Data.OwnerApprovalResult)
                      .SetOptional();
            var newExpresssion = new RewriteMatchExpression(Data).Modify(wait.MatchExpression);
            var matchCompiled = newExpresssion.Compile();
            var pushedEvent = new PushedEvent();
            pushedEvent["ProjectId"] = 11;
            pushedEvent["PreviousApproval"] = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false };
            var isMatch = wait.IsMatch(pushedEvent.ToObject(typeof(ManagerApprovalEvent)));
        }

        private static void LoadAssemblyTypes()
        {
            var assemblyPath = "D:\\Workflow\\WorkflowInCode\\ResumableFunction.WebApiEventProvider\\bin\\Debug\\net7.0\\ResumableFunction.WebApiEventProvider.dll";
            string[] runtimeDlls = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var resolver = new PathAssemblyResolver(new List<string>(runtimeDlls) { assemblyPath });
            using (var metadataContext = new MetadataLoadContext(resolver))
            {
                Assembly assembly = metadataContext.LoadFromAssemblyPath(assemblyPath);
                var types = assembly.GetTypes();
            }
        }

        private static async Task TestWebApiEventProviderClient()
        {
            var x = new TestWebApiEventProvider();
            await x.Start();
        }

        ///To use Expression trees <see cref="PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))


        public class TestWebApiEventProvider : WebApiEventProviderHandler
        {
            protected override string ApiUrl => "https://localhost:7241/";

            protected override string ApiProjectName => "Example.Api";
        }
    }

}
