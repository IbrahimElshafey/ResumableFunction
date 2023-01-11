using System.Formats.Asn1;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using TestSomething.MayUse;
using WorkflowInCode.Abstraction.Engine;
using WorkflowInCode.Abstraction.Engine.InOuts;
using WorkflowInCode.Abstraction.Samples;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var projectApprovalWorkflow = new ProjectApproval(
                new ProjectRequestedEvent() { EventData = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" } },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, false, true) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) });

            //projectApprovalWorkflow.InstanceData.Project = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" };
            //projectApprovalWorkflow.InstanceData.SponsorApprovalResult = new ProjectApprovalResult(122, true, false);
            //projectApprovalWorkflow.InstanceData.OwnerApprovalResult = new ProjectApprovalResult(122, true, false);
            //projectApprovalWorkflow.InstanceData.ManagerApprovalResult = new ProjectApprovalResult(122, true, false);
            var incomingEvent = await projectApprovalWorkflow.Run();
            incomingEvent = await projectApprovalWorkflow.Run();
            incomingEvent = await projectApprovalWorkflow.Run();
            incomingEvent = await projectApprovalWorkflow.Run();
            incomingEvent = await projectApprovalWorkflow.Run();

            //var status = projectApprovalWorkflow.GetActiveRunnerState();
            //projectApprovalWorkflow.SetActiveRunnerState(-5);
            //incomingEvent = await projectApprovalWorkflow.Run();
            return;
            
        }

        ///To use Expression trees <see cref="PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))
        private static void SetContextData(ProjectApprovalContextData instanceData, string contextProp, object eventData)
        {

            var piInstance = instanceData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(instanceData, eventData);
            //save data to database
        }
    }
}
