using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ProjectApprovalWorkflow : WorkflowDefinition<ProjectApprovalWorkflow_ContextData>
    {
        /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
        public ProjectApprovalWorkflow(IWorkflowEngine workflow) : base(workflow)
        {
            
        }
        public override void RegisterSteps()
        {
            Workflow.RegisterGlobalEventFilter((eventData) => eventData.ProjectId = CurrentInstance.ContextData.ProjectAddedEvent.ProjectId);

            Workflow.RegisterStep(
                "ProjectAdded",
                new BasicEvent<dynamic>("ProjectAdded"),
                ProjectAdded);

            Workflow.RegisterStep(
              "OwnerApproval",
              new BasicEvent<dynamic>("OwnerApproval"),
              OwnerApproval);

            Workflow.RegisterStep(
                "SponsorApproval",
              new BasicEvent<dynamic>("SponsorApproval"),
              SponsorApproval);

            //stepEvent: Is the event that fire the action we want to execute 
            //stepAction: The code we execute after event fired
            //eventFilter: Filter to find the right workflow instance that must be loaded
            Workflow.RegisterStep(
              "PmoApproval",
              stepEvent: new BasicEvent<dynamic>("PmoApproval"),
              stepAction: PmoApproval);

            //Workflow.RegisterStep(
            //  "AnyOneReject",
            //  new AnyOneOfEvents()
            //      .AddEventTrigger(
            //           new BasicEvent<dynamic>("OwnerApproval"),
            //          eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
            //      .AddEventTrigger(
            //          new BasicEvent<dynamic>("SponsorApproval"),
            //          eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected)
            //      .AddEventTrigger(
            //         new BasicEvent<dynamic>("PmoApproval"),
            //          eventData => CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == eventData.ProjectId && eventData.IsRejected),
            //  WhenAnyRejectResponse);
        }

        private async Task PmoApproval(dynamic pmoApproval)
        {
            CurrentInstance.ContextData.PmoApproval = pmoApproval;
            await Workflow.SaveState();
            if (pmoApproval.IsApproved)
            {
                await Workflow.End();
            }
            else
            {
                await WhenAnyRejectResponse(pmoApproval);
            }
        }

        private async Task SponsorApproval(dynamic sponsorApproval)
        {
            CurrentInstance.ContextData.SponsorApproval = sponsorApproval;
            await Workflow.SaveState();
            if (sponsorApproval.IsApproved)
            {
                await new BasicCommand("AskPmoApproval", CurrentInstance.ContextData.ProjectAddedEvent.Id).Execute();
                await Workflow.ExpectNextStep(nameof(PmoApproval));
            }
            else
            {
                await WhenAnyRejectResponse(sponsorApproval);
            }
        }

        private async Task ProjectAdded(dynamic projectAddedEvent)
        {
            CurrentInstance.ContextData.ProjectAddedEvent = projectAddedEvent;
            await Workflow.SaveState();
            
            await new BasicCommand("AskOwnerApproval", projectAddedEvent.Id).Execute();
            await Workflow.ExpectNextStep(nameof(OwnerApproval));
        }

        private async Task OwnerApproval(dynamic ownerApproval)
        {
            if (CurrentInstance.ContextData.ProjectAddedEvent.ProjectId == ownerApproval.ProjectId && ownerApproval.IsApproved)
            {
                CurrentInstance.ContextData.OwnerApproval = ownerApproval;
                await Workflow.SaveState();
                if (ownerApproval.IsApproved)
                {
                    await new BasicCommand("AskSponsorApproval", ownerApproval.Id).Execute();
                    await Workflow.ExpectNextStep(nameof(SponsorApproval));
                }
                else
                {
                    await WhenAnyRejectResponse(ownerApproval);
                }
            }
        }

        private async Task WhenAnyRejectResponse(object rejectResponse)
        {
            await new BasicCommand("MarkProjectAsRejected", rejectResponse).Execute();
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
