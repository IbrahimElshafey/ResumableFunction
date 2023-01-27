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

            SaveExpressionToDisk();
        }

        private static void SaveExpressionToDisk()
        {
            ////var assembly = AssemblyDefinition.ReadAssembly();
            //Action<string> directAction = s => Console.WriteLine(s+"6258858585");
            //var actionBody = directAction.Method.GetMethodBody().GetILAsByteArray();
            //var dynamicMethod = new DynamicMethod("PrintString", typeof(void), new Type[] { typeof(string) });
           
            //var iLInfo = dynamicMethod.GetDynamicILInfo();
            //iLInfo.SetCode(actionBody, directAction.Method.GetMethodBody().MaxStackSize);
            //Action<string> invokePrintString = (Action<string>)dynamicMethod.CreateDelegate(typeof(Action<string>));
            //invokePrintString("56278888sds");
            ////ILGenerator il = dm.GetILGenerator();
            ////il.Emit(OpCodes.Conv_I8);
            ////il.Emit(OpCodes.Dup);
            ////il.Emit(OpCodes.Mul);
            ////il.Emit(OpCodes.Ret);
            

            Expression<Action<string>> matchExpression = s => Console.WriteLine(s);
            var bytes = Serialize(matchExpression);
            var compiled = matchExpression.Compile();
            var xx = compiled.GetMethodInfo().GetMethodBody().GetILAsByteArray();
            var body = compiled.GetMethodInfo().Module.Assembly;
            //https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-define-and-execute-dynamic-methods
            //https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbody.getilasbytearray?view=net-5.0
            //https://microsoft.public.dotnet.framework.clr.narkive.com/LO5UHhZe/injecting-il-byte-codes
        }
        //private static Func<int, int, int> CreateFromILBytes(byte[] bytes)
        //{
        //    //var asmName = new AssemblyName("DynamicAssembly");
        //    //var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
        //    //var module = asmBuilder.DefineDynamicModule("DynamicModule");
        //    //var typeBuilder = module.DefineType("DynamicType");
        //    //var method = typeBuilder.DefineMethod("DynamicMethod",
        //    //    MethodAttributes.Public | MethodAttributes.Static,
        //    //    typeof(int),
        //    //    new[] { typeof(int), typeof(int) });
        //    //method.CreateMethodBody(bytes, bytes.Length);
        //    //var type = typeBuilder.CreateType();
        //    //return (Func<int, int, int>)type.GetMethod("DynamicMethod").CreateDelegate(typeof(Func<int, int, int>));
        //}
        static byte[] Serialize(object obj)
        {
            using (var s = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                new BinaryFormatter().Serialize(s, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                return s.ToArray();
            }
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
            var managerApprovalEvent = new ManagerApprovalEvent
            {
                ProjectId = 11,
                PreviousApproval = new ManagerApprovalEvent { ProjectId = 11, Accepted = true, Rejected = false }
            };


            var match1 = new ProjectApproval().Expression1();
            var rewriteMatch = new RewriteMatchExpression(Data, match1);
            var matchCompiled = rewriteMatch.Result.Compile();
            var result = matchCompiled.DynamicInvoke(Data, managerApprovalEvent);
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
