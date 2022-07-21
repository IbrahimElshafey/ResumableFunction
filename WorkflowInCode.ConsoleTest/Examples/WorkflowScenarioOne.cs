using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class WorkflowScenarioOne : WorkflowInstance<WorkflowScenarioOne_ContextData>
    {



        public WorkflowScenarioOne(IWorkflow workflow) : base(workflow)
        {
            /*
            * بعد إضافة مشروع يتم ارسال دعوات لمدير المشروع ومالك المشروع وراعي المشروع
            * بعد موافقة الثلاثة يتم اعتماد المشروع
            * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
            */
            workflow.RegisterStep(
                new BasicEvent<dynamic>("ProjectAdded"),
                ProjectAdded);

            //stepTriggers: Multiple events that activate the step
            //eventsCollectorFunction: A method that collect events
            //stepAction: The code the engine execute when the eventsCollectorFunction return true
            workflow.RegisterStep(
             stepTriggers: new AllOfEvents()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsAccepted)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsAccepted)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsAccepted),
             eventsCollectorFunction: CollectApprovalResponses);

            workflow.RegisterStep(
             stepTriggers: new AnyOneOfEvents()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"),
                    (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected),
             eventsCollectorFunction: CollectRejectResponses);
        }

        private async Task CollectRejectResponses(dynamic approvalEvent)
        {
            await new BasicCommand("MarkProjectAsRejected", ContextData.ProjectAddedEvent.ProjectId).Execute();
            await Workflow.End();
        }
        private async Task CollectApprovalResponses(dynamic approvalEvent)
        {
            ContextData.ThreeApprovalCounter += 1;
            await SaveState();
            if (ContextData.ThreeApprovalCounter == 3)
            {
                await new BasicCommand("MarkProjectAsApproved", ContextData.ProjectAddedEvent.ProjectId).Execute();
                await Workflow.End();
            }
        }


        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            ContextData.ProjectAddedEvent = projectAddedEvent;
            await SaveState();
            await new BasicCommand("SendInvitations", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep<dynamic>(CollectApprovalResponses);
            await Workflow.ExpectNextStep<dynamic>(CollectRejectResponses);
        }
    }
    class WorkflowScenarioOne_ContextData
    {
        public dynamic ProjectAddedEvent { get; internal set; }
        public int ThreeApprovalCounter { get; internal set; }
        public dynamic PmoApproval { get; internal set; }
        public dynamic SponsorApproval { get; internal set; }
        public dynamic OwnerApproval { get; internal set; }
    }
}
