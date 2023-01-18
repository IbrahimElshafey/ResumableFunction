using System.Formats.Asn1;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using TestSomething.MayUse;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Abstraction.Samples;
using ResumableFunction.Engine;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            //var projectApprovalFunction = new ProjectApproval(
            //    new ProjectRequestedEvent() { DueDate = DateTime.Now, Id = 122, Name = "Project1" },
            //    new ManagerApprovalEvent(122, true, false),
            //    new ManagerApprovalEvent(122, true, false),
            //    new ManagerApprovalEvent(122, true, false)
            //    );
            var testDefaults = new TestDefaults();
            //await testDefaults.FunctionEngine.RegisterAssembly(Assembly.GetExecutingAssembly());
            //await testDefaults.FunctionEngine.RegisterEventProvider(typeof(SimpleEventProvider));
            //await testDefaults.FunctionEngine.RegisterFunction(typeof(ProjectApproval));


            var projectApprovalFunction = new ProjectApproval(new ProjectApprovalContextData(), testDefaults.FunctionEngine);
            //projectApprovalFunction.FunctionData.Project = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" };
            //projectApprovalFunction.FunctionData.SponsorApprovalResult = new ProjectApprovalResult(122, true, false);
            //projectApprovalFunction.FunctionData.OwnerApprovalResult = new ProjectApprovalResult(122, true, false);
            //projectApprovalFunction.FunctionData.ManagerApprovalResult = new ProjectApprovalResult(122, true, false);
            var incomingEvent = await projectApprovalFunction.Run();
            Console.WriteLine(incomingEvent);
            incomingEvent = await projectApprovalFunction.Run();
            Console.WriteLine(incomingEvent);
            incomingEvent = await projectApprovalFunction.Run();
            Console.WriteLine(incomingEvent);
            incomingEvent = await projectApprovalFunction.Run();
            Console.WriteLine(incomingEvent);
            incomingEvent = await projectApprovalFunction.Run();
            //Console.WriteLine(incomingEvent);

            Console.ReadLine();

        }
        private static void MethodX<T>(params T[] objects)
        {
        }

        ///To use Expression trees <see cref="PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))
        private static void SetContextData(ProjectApprovalContextData FunctionData, string contextProp, object eventData)
        {

            var piInstance = FunctionData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(FunctionData, eventData);
            //save data to database
        }
    }
}
