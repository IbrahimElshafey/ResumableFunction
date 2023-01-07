using System.Formats.Asn1;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using WorkflowInCode.Abstraction.Samples;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var po = new ProjectApproval(
                new ProjectRequestedEvent() { EventData = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" } },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) });

            //event come to engine
            //Translate matching function to database query
            //If event is first then add workflow instance to the database
            //if not first then search the database for instance/s that match query and load workflow data context
            await foreach (var incomingEvent in po.RunWorkflow())
            {
                Console.WriteLine(incomingEvent.MatchFunction);
                Console.WriteLine(incomingEvent.ContextProp);
                SetContextData(po.InstanceData, incomingEvent.ContextProp, incomingEvent.EventData);
                Console.WriteLine(incomingEvent);
            }
        }

        private static void SetContextData(ProjectApprovalContextData instanceData, string contextProp, object eventData)
        {
            var piInstance = instanceData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(instanceData, eventData);
        }
    }
}
