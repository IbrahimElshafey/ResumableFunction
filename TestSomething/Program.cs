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
using System.Collections.ObjectModel;
using static System.Linq.Expressions.Expression;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Test
{
    public static partial class Program
    {
        public static ProjectApprovalFunctionData Data =>
            new ProjectApprovalFunctionData
            {
                Project = new ProjectRequestedEvent { Id = 11 },
                ManagerApprovalResult = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false }
            };
        static async Task Main(string[] args)
        {
            var managerApprovalEvent = new ManagerApprovalEvent
            {
                ProjectId = 11,
                PreviousApproval = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false }
            };


            var match1 = new ProjectApproval().Expression1();
            var rewriteMatch = new RewriteMatchExpression(Data, match1);
            var matchCompiled = rewriteMatch.Result.Compile();
            var result = rewriteMatch.NeedFunctionData ?
                matchCompiled.DynamicInvoke(Data, managerApprovalEvent) :
                matchCompiled.DynamicInvoke(null, managerApprovalEvent);

            //TestMatchTranslation();
            // using static System.Linq.Expressions.Expression





        }
        private static void TestMatchTranslation()
        {
            var wait = new EventWait<ManagerApprovalEvent>("OwnerApproval")
                                  .Match<ProjectApprovalFunctionData>((data, result) =>
                                      result.ProjectId > Math.Min(5, 10) &&
                                      result.PreviousApproval.Equals(data.ManagerApprovalResult) &&
                                      result.ProjectId == data.Project.Id)
                                  .SetProp(() => Data.OwnerApprovalResult)
                                  .SetOptional();
            var newExpresssion =
                wait.NeedFunctionDataForMatch ?
                wait.MatchExpression :
                new RewriteMatchExpression(Data, wait.MatchExpression).Result;
            var matchCompiled = newExpresssion.Compile();
            var pushedEvent = new PushedEvent();
            pushedEvent["ProjectId"] = 11;
            pushedEvent["PreviousApproval"] =
                new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false };

            //var isMatch =
            //    wait.NeedFunctionDataForMatch ?
            //    wait.IsMatch(Data, pushedEvent.ToObject(typeof(ManagerApprovalEvent))) :
            //    wait.IsMatch(pushedEvent.ToObject(typeof(ManagerApprovalEvent)));
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
