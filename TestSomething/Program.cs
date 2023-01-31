using System.Reflection;
using TestSomething.MayUse;
using ResumableFunction.Abstraction.Samples;
using ResumableFunction.Abstraction.WebApiProvider;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine;
using System.Runtime.InteropServices;
using ResumableFunction.Abstraction.InOuts;
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
using Newtonsoft.Json.Bson;
using SDILReader;
using Newtonsoft.Json.Serialization;

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
            TestSetPropRewrite();

            //SaveExpressionAsJson();
            //SaveExpressionAsBinary();
            //Program2.Main1();
        }


        private static void SaveExpressionAsBinary()
        {

            var wait = new ProjectApproval().EventWait();
            wait.MatchExpression = new RewriteMatchExpression(wait).Result;
            DynamicMethod dynamicMatch = new DynamicMethod(
                 "DynamicIsMatch",
                 typeof(bool),
                 new[] { typeof(ProjectApprovalFunctionData), typeof(ManagerApprovalEvent) });
            ILGenerator ilGenerator = dynamicMatch.GetILGenerator();
            wait.MatchExpression.CompileFastToIL(ilGenerator);
            var dynamicInvoker = (Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>)
                dynamicMatch.CreateDelegate(typeof(Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>));
            //dynamicInvoker.GetMethodInfo().GetMethodBody().GetILAsByteArray();
            var result1 = dynamicInvoker((ProjectApprovalFunctionData)wait.FunctionRuntimeInfo?.Data, wait.EventData);


            //save method to disk
            var originalIlGnerator = Exposed.From(ilGenerator);
            DynamicMethod dynamicMatch2 = new DynamicMethod(
               "DynamicIsMatch2",
               typeof(bool),
               new[] { typeof(ProjectApprovalFunctionData), typeof(ManagerApprovalEvent) });
            var newIlgenerator = Exposed.From(dynamicMatch2.GetILGenerator());

            newIlgenerator.m_ILStream = originalIlGnerator.m_ILStream;
            //easy serialize
            newIlgenerator.m_RelocFixupCount = originalIlGnerator.m_RelocFixupCount;
            newIlgenerator.m_RelocFixupList = originalIlGnerator.m_RelocFixupList;
            newIlgenerator.m_curDepth = originalIlGnerator.m_curDepth;
            newIlgenerator.m_currExcStackCount = originalIlGnerator.m_currExcStackCount;
            newIlgenerator.m_depthAdjustment = originalIlGnerator.m_depthAdjustment;
            newIlgenerator.m_exceptionCount = originalIlGnerator.m_exceptionCount;
            newIlgenerator.m_fixupCount = originalIlGnerator.m_fixupCount;
            newIlgenerator.m_labelCount = originalIlGnerator.m_labelCount;
            newIlgenerator.m_length = originalIlGnerator.m_length;
            newIlgenerator.m_localCount = originalIlGnerator.m_localCount;
            newIlgenerator.m_maxDepth = originalIlGnerator.m_maxDepth;
            newIlgenerator.m_methodSigToken = originalIlGnerator.m_methodSigToken;
            newIlgenerator.m_targetDepth = originalIlGnerator.m_targetDepth;


            //hard serialize
            //newIlgenerator.m_ScopeTree = originalIlGnerator.m_ScopeTree;

            newIlgenerator.m_fixupData = originalIlGnerator.m_fixupData;
            newIlgenerator.m_labelList = originalIlGnerator.m_labelList;
            newIlgenerator.m_localSignature = originalIlGnerator.m_localSignature;
            newIlgenerator.m_scope = originalIlGnerator.m_scope;
            var dynamicInvoker2 = (Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>)
              dynamicMatch2.CreateDelegate(typeof(Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>));
            var result2 = dynamicInvoker2((ProjectApprovalFunctionData)wait.FunctionRuntimeInfo?.Data, wait.EventData);
            //TestBinarySaveOne(wait, dynamicMatch);

            //https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-define-and-execute-dynamic-methods
            //https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbody.getilasbytearray?view=net-5.0
            //https://microsoft.public.dotnet.framework.clr.narkive.com/LO5UHhZe/injecting-il-byte-codes
            //https://github.com/aquilae/expression-json-serializer
            //https://github.com/dadhi/FastExpressionCompiler
            //https://github.com/wttech/ExposedObject
            //https://github.com/skolima/ExposedObject
            //https://www.codeproject.com/Articles/14058/Parsing-the-IL-of-a-Method-Body

            string SerializeTest(object o)
            {
                return JsonConvert.SerializeObject(
                o,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new PrivateField() }
                );
            }
        }
        public class PrivateField : DefaultContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var result = base.GetSerializableMembers(objectType);
                result.AddRange(objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                return result;
            }
        }
        private static void TestBinarySaveOne(EventWait wait, DynamicMethod dynamicMatch)
        {
            dynamic il = Exposed.From(dynamicMatch.GetILGenerator());
            var codeBytes = ((byte[])il.m_ILStream).Take((int)il.ILOffset).ToArray();
            var maxStackSize = il.m_maxDepth;
            var scope = (List<object>)Exposed.From(il.m_scope).m_tokens;//System.Reflection.Emit.DynamicScope
            //var signature = BitConverter.GetBytes(il.m_methodSigToken);
            var localSignature = Exposed.From(il.m_localSignature);
            var signature = ((byte[])localSignature.m_signature).Take((int)localSignature.m_currSig).ToArray();

            DynamicMethod dynamicMatch2 = new DynamicMethod(
                "DynamicIsMatch2",
                typeof(bool),
                new[] { typeof(ProjectApprovalFunctionData), typeof(ManagerApprovalEvent) });
            var dynamicILInfo = dynamicMatch2.GetDynamicILInfo();

            dynamicILInfo.SetLocalSignature(signature);
            foreach (var item in scope.Skip(2))
            {
                var token = -1;
                switch (item)
                {
                    case RuntimeMethodHandle method:
                        var x = typeof(ManagerApprovalEvent).GetMethod("get_ProjectId").MethodHandle;
                        if (method == x)
                            token = dynamicILInfo.GetTokenFor(x);
                        else
                            token = dynamicILInfo.GetTokenFor(method);
                        break;
                    case RuntimeFieldHandle fieldHandle:
                        token = dynamicILInfo.GetTokenFor(fieldHandle);
                        break;
                    case string literal:
                        //todo:must update IL with new reference
                        token = dynamicILInfo.GetTokenFor(literal);
                        //byte[] bytes = BitConverter.GetBytes(token);
                        //if (BitConverter.IsLittleEndian)
                        //    Array.Reverse(bytes);
                        break;
                    case RuntimeTypeHandle runtimeTypeHandle:
                        token = dynamicILInfo.GetTokenFor(runtimeTypeHandle);
                        break;
                    case DynamicMethod dynamicMethod:
                        token = dynamicILInfo.GetTokenFor(dynamicMethod);
                        break;
                        //default:
                        //    throw new Exception($"Can't GetTokenFor for `{item}`");
                        //    break;
                }
            }
            //Exposed.From(ilInfo).m_scope = il.m_scope;
            dynamicILInfo.SetCode(codeBytes, maxStackSize);
            var isMatch =
                (Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>)dynamicMatch2
                .CreateDelegate(typeof(Func<ProjectApprovalFunctionData, ManagerApprovalEvent, bool>));
            var result2 = isMatch((ProjectApprovalFunctionData)wait.FunctionRuntimeInfo?.Data, wait.EventData);


            MethodBodyReader mr = new MethodBodyReader(codeBytes, isMatch.Method.Module);
            // get the text representation of the msil
            string msil = mr.GetBodyCode();
            // or parse the list of instructions of the MSIL
            for (int i = 0; i < mr.instructions.Count; i++)
            {
                // do something with mr.instructions[i]
            }
        }

        //private void test()
        //{
        //    var targetAsm = AssemblyDefinition.ReadAssembly("target_path");
        //    var mr1 = targetAsm.MainModule.Import(typeof(ProjectApproval).GetMethod("Test"));
        //    var targetType = targetAsm.MainModule.Types.FirstOrDefault(e => e.Name == "Target");
        //    MethodDefinition? m2 = targetType.Methods.FirstOrDefault(e => e.Name == "Test");
        //    var m1 = mr1.Resolve();
        //    var m1IL = m1.Body.GetILProcessor();
        //    foreach (var i in m1.Body.Instructions.ToList())
        //    {
        //        var ci = i;
        //        if (i.Operand is MethodReference)
        //        {
        //            var mref = i.Operand as MethodReference;
        //            ci = m1IL.Create(i.OpCode, targetType.Module.Import(mref));
        //        }
        //        else if (i.Operand is TypeReference)
        //        {
        //            var tref = i.Operand as TypeReference;
        //            ci = m1IL.Create(i.OpCode, targetType.Module.Import(tref));
        //        }
        //        if (ci != i)
        //        {
        //            m1IL.Replace(i, ci);
        //        }
        //    }
        //}

        private static void SaveExpressionAsJson()
        {
            var wait = new ProjectApproval().EventWait();
            wait.MatchExpression = new RewriteMatchExpression(wait).Result;
            var rr = wait.IsMatch();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(
                new ExpressionJsonConverter(Assembly.GetAssembly(wait.FunctionRuntimeInfo.InitiatedByClassType)));

            var json = JsonConvert.SerializeObject(wait.MatchExpression, settings);
            var target = JsonConvert.DeserializeObject<LambdaExpression>(json, settings);
            wait.MatchExpression = target;
            rr = wait.IsMatch();
            var length = System.Text.ASCIIEncoding.Unicode.GetByteCount(json);
        }


        private static void TestSetPropRewrite()
        {
          
            var wait = new ProjectApproval().EventWait();
            var setPropRewrite = new RewriteSetDataExpression(wait).Result;
            wait.SetDataExpression = setPropRewrite;
            wait.SetData();
        }

        private static void TestFunctionClassWrapper()
        {
            var wrapper = new ResumableFunctionWrapper(new ProjectApproval().EventWait());
            Console.WriteLine(wrapper.FunctionClassInstance);
            Console.WriteLine(wrapper.FunctionClassInstance.GetType());
            Console.WriteLine(wrapper.Data);
            Console.WriteLine(wrapper.Data.GetType());
            Console.WriteLine(wrapper.FunctionRuntimeInfo);
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
                                  .SetData(() => Data.OwnerApprovalResult)
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

