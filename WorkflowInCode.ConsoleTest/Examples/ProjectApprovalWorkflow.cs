using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ProjectApprovalWorkflow : WorkflowInstance<ProjectApprovalWorkflow_ContextData>
    {
        /*
         * بعد إضافة مشروع يتم ارسال دعوة مالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
        public ProjectApprovalWorkflow(IWorkflow workflow) : base(workflow)
        {
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
                      eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                  .AddEventTrigger(
                      new BasicEvent<dynamic>("SponsorApproval"),
                      eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
                  .AddEventTrigger(
                     new BasicEvent<dynamic>("PmoApproval"),
                      eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected),
              WhenAnyRejectResponse);
        }

        private async Task PmoApproval(dynamic pmoApproval)
        {
            if (ContextData.ProjectAddedEvent.ProjectId == pmoApproval.ProjectId && pmoApproval.IsApproved)
            {
                ContextData.PmoApproval = pmoApproval;
                await SaveState();
                await Workflow.End();
            }
        }

        private async Task SponsorApproval(dynamic sponsorApproval)
        {
            if (ContextData.ProjectAddedEvent.ProjectId == sponsorApproval.ProjectId && sponsorApproval.IsApproved)
            {
                ContextData.SponsorApproval = sponsorApproval;
                await SaveState();
                await new BasicCommand("AskPmoApproval", ContextData.ProjectAddedEvent.Id).Execute();
                await Workflow.ExpectNextStep("PmoApproval");
            }
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            ContextData.ProjectAddedEvent = projectAddedEvent;
            await SaveState();
            await new BasicCommand("AskOwnerApproval", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep("OwnerApproval");
        }

        private async Task OwnerApproval(dynamic ownerApproval)
        {
            if (ContextData.ProjectAddedEvent.ProjectId == ownerApproval.ProjectId && ownerApproval.IsApproved)
            {
                ContextData.OwnerApproval = ownerApproval;
                await SaveState();
                await new BasicCommand("AskSponsorApproval", ownerApproval.Id).Execute();
                await Workflow.ExpectNextStep("SponsorApproval");
            }
        }

        private async Task WhenAnyRejectResponse(dynamic approvalEvent)
        {
            await new BasicCommand("MarkProjectAsRejected", ContextData.ProjectAddedEvent.ProjectId).Execute();
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
