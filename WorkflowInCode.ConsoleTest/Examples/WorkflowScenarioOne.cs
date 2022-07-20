using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    //go to سيناريوهات.md for more details
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

            workflow.RegisterStep(
                new StepTriggers()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"),(eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .SetCollectorMethod(CollectApprovalResponses),
                ThreeApproved);

            workflow.RegisterStep(
                new StepTriggers()
                .AddEventTrigger(
                    new BasicEvent<dynamic>("PmoApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("OwnerApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("SponsorApproval"), (eventData) => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId)
                .SetCollectorMethod(CollectRejectResponses),
                OneRejected);
        }

        private async Task OneRejected(Task arg)
        {
            await new BasicCommand("MarkProjectAsRejected", ContextData.ProjectAddedEvent.ProjectId).Initiate();
            await Workflow.End();
        }

        private bool CollectRejectResponses(dynamic approvalEvent)
        {
            return approvalEvent.IsReject;
        }
        private bool CollectApprovalResponses(dynamic approvalEvent)
        {
            if (approvalEvent.IsAccept)
            {
                ContextData.ThreeApprovalCounter += 1;
                SaveState();
            }
            return ContextData.ThreeApprovalCounter == 3;
        }

        private async Task ThreeApproved(dynamic threeApproved)
        {
            await new BasicCommand("MarkProjectAsApproved",ContextData.ProjectAddedEvent.ProjectId).Initiate();
            await Workflow.End();
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            ContextData.ProjectAddedEvent = projectAddedEvent;
            await SaveState();
            await new BasicCommand("SendInvitations", projectAddedEvent.Id).Initiate();
            await Workflow.ExpectNextStep(ThreeApproved);
            await Workflow.ExpectNextStep(OneRejected);
        }
    }
    class WorkflowScenarioOne_ContextData
    {
        public dynamic ProjectAddedEvent { get; internal set; }
        public int ThreeApprovalCounter { get; internal set; }
    }
}
