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
    public class ProjectApproval2 : ProjectApproval
    {
        public ProjectApproval2(ProjectRequestedEvent p, ManagerApprovalEvent po, ManagerApprovalEvent ps, ManagerApprovalEvent pm) : base(p, po, ps, pm)
        {
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
                    new EventWaiting<ProjectRequestedEvent>(ProjectRequested, result => result.Id == InstanceData.Project.Id, () => InstanceData.Project),
                    new EventWaiting<ManagerApprovalEvent>(OwnerApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.OwnerApprovalResult),
                    new EventWaiting<ManagerApprovalEvent>(SponsorApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.SponsorApprovalResult),
                    new EventWaiting<ManagerApprovalEvent>(ManagerApproval, result => result.ProjectId == InstanceData.Project.Id, () => InstanceData.ManagerApprovalResult) { IsOptional = true }
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

    }
}
