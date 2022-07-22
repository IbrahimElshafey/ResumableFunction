using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class WorkflowScenarioTwo : WorkflowInstance<WorkflowScenarioOne_ContextData>
    {
        /*
         * بعد إضافة مشروع يتم ارسال دعوة مالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
        public WorkflowScenarioTwo(IWorkflow workflow) : base(workflow)
        {
            workflow.RegisterStep(
                new BasicEvent<dynamic>("ProjectAdded"),
                ProjectAdded);

            workflow.RegisterStep(
              new BasicEvent<dynamic>("OwnerApproval"),
              OwnerApproval,
              eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsApproved);

            workflow.RegisterStep(
              new BasicEvent<dynamic>("SponsorApproval"),
              SponsorApproval,
              eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsApproved);

            //stepEvent: Is the event that fire the action we want to execute 
            //stepAction: The code we execute after event fired
            //eventFilter: Filter to find the right workflow instance that must be loaded
            workflow.RegisterStep(
              stepEvent: new BasicEvent<dynamic>("PmoApproval"),
              stepAction:PmoApproval,
              eventFilter:eventData => ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsApproved);

            workflow.RegisterStep(
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
            ContextData.PmoApproval = pmoApproval;
            await SaveState();
            await Workflow.End();
        }

        private async Task SponsorApproval(dynamic sponsorApproval)
        {
            ContextData.SponsorApproval = sponsorApproval;
            await SaveState();
            await new BasicCommand("AskPmoApproval", ContextData.ProjectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep<dynamic>(PmoApproval);
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            ContextData.ProjectAddedEvent = projectAddedEvent;
            await SaveState();
            await new BasicCommand("AskOwnerApproval", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep<dynamic>(OwnerApproval);
        }

        private async Task OwnerApproval(dynamic ownerApproval)
        {
            ContextData.OwnerApproval = ownerApproval;
            await SaveState();
            await new BasicCommand("AskSponsorApproval", ownerApproval.Id).Execute();
            await Workflow.ExpectNextStep<dynamic>(SponsorApproval);
        }

        private async Task WhenAnyRejectResponse(dynamic approvalEvent)
        {
            await new BasicCommand("MarkProjectAsRejected", ContextData.ProjectAddedEvent.ProjectId).Execute();
            await Workflow.End();
        }
    }
}
