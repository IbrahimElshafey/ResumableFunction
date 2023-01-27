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
using System.Reflection.Emit;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json;
using FastExpressionCompiler;
using ExposedObject;
using System.Net.Security;

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
            //MatchFunctionTranslation();

            //TestMatchTranslation();
            // using static System.Linq.Expressions.Expression

            //TestFunctionClassWrapper();
            //TestSetPropRewrite();

            //SaveExpressionAsJson();
            SaveExpressionAsBinary();
        }

        private static void SaveExpressionAsBinary()
        {
            var wait = new ProjectApproval().EventWait();
            wait.MatchExpression = new RewriteMatchExpression(wait).Result;
            DynamicMethod dynamicMatch = new DynamicMethod(
                 "DynamicIsMatch",
                 typeof(bool),
                 new[] { typeof(ProjectApprovalFunctionData), typeof(ManagerApprovalEvent) });
            ILGenerator il1 = dynamicMatch.GetILGenerator();
            wait.MatchExpression.CompileFastToIL(il1);
            var dynamicInvoker = (Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>)
                dynamicMatch.CreateDelegate(typeof(Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>));
            var result1 = dynamicInvoker((ProjectApprovalFunctionData)wait.ParentFunctionState?.Data, wait.EventData);

            DynamicMethod dynamicMatch2 = new DynamicMethod(
                "DynamicIsMatch",
                typeof(bool),
                new[] { typeof(ProjectApprovalFunctionData), typeof(ManagerApprovalEvent) });
            dynamic il = Exposed.From(dynamicMatch.GetILGenerator());
            var codeBytes = ((byte[])il.m_ILStream).Take((int)il.ILOffset).ToArray();
            var maxStackSize = il.m_maxDepth;
            var scope = (List<object>)Exposed.From(il.m_scope).m_tokens;//System.Reflection.Emit.DynamicScope
            //var signature = BitConverter.GetBytes(il.m_methodSigToken);
            var localSignature = Exposed.From(il.m_localSignature);
            var signature = ((byte[])localSignature.m_signature).Take((int)localSignature.m_currSig).ToArray();
            var ilInfo = dynamicMatch2.GetDynamicILInfo();

            ilInfo.SetCode(codeBytes, maxStackSize);
            ilInfo.SetLocalSignature(signature);
            foreach ( var item in scope)
            {
                switch(item)
                {
                    case RuntimeMethodHandle method:
                        ilInfo.GetTokenFor(method);
                        break;
                    case RuntimeFieldHandle fieldHandle:
                        ilInfo.GetTokenFor(fieldHandle);
                        break;
                    case string literal:
                        ilInfo.GetTokenFor(literal);
                        break;
                    case RuntimeTypeHandle runtimeTypeHandle:
                        ilInfo.GetTokenFor(runtimeTypeHandle);
                        break;
                    case DynamicMethod dynamicMethod:
                        ilInfo.GetTokenFor(dynamicMethod);
                        break;
                    default:
                        throw new Exception($"Can't GetTokenFor for `{item}`");
                        break;
                }
            }
            var isMatch = 
                (Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>)dynamicMatch2
                .CreateDelegate(typeof(Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>));
            var result2 = isMatch((ProjectApprovalFunctionData)wait.ParentFunctionState?.Data, wait.EventData);

            //https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-define-and-execute-dynamic-methods
            //https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbody.getilasbytearray?view=net-5.0
            //https://microsoft.public.dotnet.framework.clr.narkive.com/LO5UHhZe/injecting-il-byte-codes
            //https://github.com/aquilae/expression-json-serializer
            //https://github.com/dadhi/FastExpressionCompiler
            //https://github.com/wttech/ExposedObject
            //https://github.com/skolima/ExposedObject
        }

        private static void SaveExpressionAsJson()
        {
            var wait = new ProjectApproval().EventWait();
            wait.MatchExpression = new RewriteMatchExpression(wait).Result;
            var rr = wait.IsMatch();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(
                new ExpressionJsonConverter(Assembly.GetAssembly(wait.ParentFunctionState.InitiatedByClass)));

            var json = JsonConvert.SerializeObject(wait.MatchExpression, settings);
            var target = JsonConvert.DeserializeObject<LambdaExpression>(json, settings);
            wait.MatchExpression = target;
            rr = wait.IsMatch();

        }


        private static void TestSetPropRewrite()
        {
            var wait = new ProjectApproval().EventWait();
            var setPropRewrite = new RewriteSetPropExpression(wait).Result;
            var eventData = new ManagerApprovalEvent { Accepted = true, ProjectId = 11 };
            ProjectApprovalFunctionData functionData = new ProjectApprovalFunctionData();
            setPropRewrite.Compile().DynamicInvoke(functionData, eventData);
        }

        private static void TestFunctionClassWrapper()
        {
            var wrapper = new ResumableFunctionWrapper(new ProjectApproval().EventWait());
            Console.WriteLine(wrapper.FunctionClassInstance);
            Console.WriteLine(wrapper.FunctionClassInstance.GetType());
            Console.WriteLine(wrapper.Data);
            Console.WriteLine(wrapper.Data.GetType());
            Console.WriteLine(wrapper.FunctionState);
        }

        private static void MatchFunctionTranslation()
        {
            //var managerApprovalEvent = new ManagerApprovalEvent
            //{
            //    ProjectId = 11,
            //    PreviousApproval = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false }
            //};


            //var match1 = new ProjectApproval().Expression1();
            //var rewriteMatch = new RewriteMatchExpression(Data, match1);
            //var matchCompiled = rewriteMatch.Result.Compile();
            //var result = matchCompiled.DynamicInvoke(Data, managerApprovalEvent);
        }

        private static void TestMatchTranslation()
        {
            var wait = new EventWait<ManagerApprovalEvent>("OwnerApproval")
                                  .Match(result =>
                                      result.ProjectId > Math.Min(5, 10) &&
                                      result.PreviousApproval.Equals(Data.ManagerApprovalResult) &&
                                      result.ProjectId == Data.Project.Id)
                                  .SetProp(() => Data.OwnerApprovalResult)
                                  .SetOptional();
            var newExpresssion =
                new RewriteMatchExpression(wait).Result;
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

