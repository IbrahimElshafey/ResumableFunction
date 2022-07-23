using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ProjectApprovalWorkflow : WorkflowDefinition<ProjectApprovalWorkflow_ContextData>
    {
        /*
         * بعد إضافة مشروع يتم ارسال دعوة مالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
        public ProjectApprovalWorkflow(IWorkflowEngine workflow) : base(workflow)
        {
            workflow.RegisterGlobalEventFilter((eventData) => eventData.ProjectId = CurrentInstance.ContextData.ProjectAddedEvent.ProjectId);
            
            workflow.RegisterStep(
                "ProjectAdded",
                new BasicEvent<dynamic>("ProjectAdded"),
                ProjectAdded);

            workflow.RegisterStep(
              "OwnerApproval",
              new BasicEvent<dynamic>("OwnerApproval"),
              OwnerApproval);

            workflow.RegisterStep(
                "SponsorApproval",
              new BasicEvent<dynamic>("SponsorApproval"),
              SponsorApproval);

            //stepEvent: Is the event that fire the action we want to execute 
            //stepAction: The code we execute after event fired
            //eventFilter: Filter to find the right workflow instance that must be loaded
            workflow.RegisterStep(
              "PmoApproval",
              stepEvent: new BasicEvent<dynamic>("PmoApproval"),
              stepAction: PmoApproval);

            workflow.RegisterStep(
              "AnyOneReject",
              new AnyOneOfEvents()
                  .AddEventTrigger(
                       new BasicEvent<dynamic>("OwnerApproval"),
                      eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                  .AddEventTrigger(
                      new BasicEvent<dynamic>("SponsorApproval"),
                      eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                  .AddEventTrigger(
                     new BasicEvent<dynamic>("PmoApproval"),
                      eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected),
              WhenAnyRejectResponse);
        }

        private async Task PmoApproval(dynamic pmoApproval)
        {
            if (CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == pmoApproval.ProjectId && pmoApproval.IsApproved)
            {
                CurrentInstance.ContextData.PmoApproval = pmoApproval;
                await Workflow.SaveState();
                await Workflow.End();
            }
        }

        private async Task SponsorApproval(dynamic sponsorApproval)
        {
            if (CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == sponsorApproval.ProjectId && sponsorApproval.IsApproved)
            {
                CurrentInstance.ContextData.SponsorApproval = sponsorApproval;
                await Workflow.SaveState();
                await new BasicCommand("AskPmoApproval", CurrentInstance.ContextData.ProjectAddedEvent.Id).Execute();
                await Workflow.ExpectNextStep("PmoApproval");
            }
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            CurrentInstance.ContextData.ProjectAddedEvent = projectAddedEvent;
            await Workflow.SaveState();
            await new BasicCommand("AskOwnerApproval", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep("OwnerApproval");
        }

        private async Task OwnerApproval(dynamic ownerApproval)
        {
            if (CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == ownerApproval.ProjectId && ownerApproval.IsApproved)
            {
                CurrentInstance.ContextData.OwnerApproval = ownerApproval;
                await Workflow.SaveState();
                await new BasicCommand("AskSponsorApproval", ownerApproval.Id).Execute();
                await Workflow.ExpectNextStep("SponsorApproval");
            }
        }

        private async Task WhenAnyRejectResponse(dynamic approvalEvent)
        {
            await new BasicCommand("MarkProjectAsRejected", CurrentInstance.ContextData.ProjectAddedEvent.ProjectId).Execute();
            await Workflow.End();
        }
    }

    class ProjectApprovalWorkflow_ContextData
    {
        public dynamic ProjectAddedEvent { get; internal set; }
        public int ThreeApprovalCounter { get; internal set; }
        public dynamic PmoApproval { get; internal set; }
        public dynamic SponsorApproval { get; internal set; }
        public dynamic OwnerApproval { get; internal set; }
    }
}
