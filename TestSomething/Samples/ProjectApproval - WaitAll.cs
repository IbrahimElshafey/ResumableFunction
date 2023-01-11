using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
    public class ProjectApproval2 : WorkflowInstance<ProjectApprovalContextData>
    {
        public ProjectRequestedEvent ProjectRequested;
        public ManagerApprovalEvent OwnerApproval;
        public ManagerApprovalEvent SponsorApproval;
        public ManagerApprovalEvent ManagerApproval;
        public ProjectApproval2(ProjectRequestedEvent p, ManagerApprovalEvent po, ManagerApprovalEvent ps, ManagerApprovalEvent pm)
        {
            ProjectRequested = p;
            OwnerApproval = po;
            SponsorApproval = ps;
            ManagerApproval = pm;
            InstanceData = new ProjectApprovalContextData();
        }

        protected override async IAsyncEnumerable<WorkflowEvent> RunWorkflow()
        {
            yield return WaitEvent(
                ProjectRequested,
                null,
                () => InstanceData.Project);
            if (InstanceData.Project is not null)
            {
                await AskOwnerToApprove(InstanceData.Project);
                await AskSponsorToApprove(InstanceData.Project);
                await AskManagerToApprove(InstanceData.Project);

                yield return WaitEvents(
                    //new EventWaiting<Project>(ProjectRequested, result => result.Id == InstanceData.Project.Id, () => InstanceData.Project),
                    new EventWaiting<ProjectApprovalResult>(OwnerApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.OwnerApprovalResult),
                    new EventWaiting<ProjectApprovalResult>(SponsorApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.SponsorApprovalResult),
                    new EventWaiting<ProjectApprovalResult>(ManagerApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.ManagerApprovalResult) { IsOptional = true }
                    );

                var allManagerResponse = InstanceData.ManagerApprovalResult.Accepted == InstanceData.OwnerApprovalResult.Accepted == InstanceData.SponsorApprovalResult.Accepted;
                if (allManagerResponse is true)
                {
                    Console.WriteLine("Final Acceptance");
                }
                else if (allManagerResponse is false)
                {
                    Console.WriteLine("Final Rejection");
                }
                else
                {
                    Console.WriteLine("Ask Upper Managment");
                }
            }
        }

        private async Task AskManagerToApprove(Project project)
        {
            await Task.Delay(1000);
        }

        private async Task AskSponsorToApprove(Project project)
        {
            await Task.Delay(1000);
        }

        private async Task AskOwnerToApprove(Project project)
        {
            await Task.Delay(1000);
        }
    }
}
