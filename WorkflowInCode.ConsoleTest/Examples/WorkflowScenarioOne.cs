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
             stepTriggers:new StepTriggers()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"),(eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId),
             eventsCollectorFunction:CollectApprovalResponses,
             stepAction:ThreeApproved);

            workflow.RegisterStep(
                new StepTriggers()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId),
                CollectRejectResponses,
                OneRejected);
        }

        private async Task OneRejected(dynamic arg)
        {
            await new BasicCommand("MarkProjectAsRejected", ContextData.ProjectAddedEvent.ProjectId).Execute();
            await Workflow.End();
        }

        private async Task<bool> CollectRejectResponses(dynamic approvalEvent)
        {
            return approvalEvent.IsReject;
        }
        private async Task<bool> CollectApprovalResponses(dynamic approvalEvent)
        {
            if (approvalEvent.IsAccept)
            {
                ContextData.ThreeApprovalCounter += 1;
                await SaveState();
            }
            return ContextData.ThreeApprovalCounter == 3;
        }

        private async Task ThreeApproved(dynamic threeApproved)
        {
            await new BasicCommand("MarkProjectAsApproved",ContextData.ProjectAddedEvent.ProjectId).Execute();
            await Workflow.End();
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            ContextData.ProjectAddedEvent = projectAddedEvent;
            await SaveState();
            await new BasicCommand("SendInvitations", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep<dynamic>(ThreeApproved);
            await Workflow.ExpectNextStep<dynamic>(OneRejected);
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
