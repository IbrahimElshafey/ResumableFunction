using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    public class ProjectApproval3 : ProjectApproval
    {
        public ProjectApproval3(ProjectRequestedEvent p, ManagerApprovalEvent po, ManagerApprovalEvent ps, ManagerApprovalEvent pm) : base(p, po, ps, pm)
        {
        }

        protected override async IAsyncEnumerable<EventWaiting> RunWorkflow()
        {

            await Task.Delay(100);
            yield return WaitEvent(ProjectRequested).SetProp(() => InstanceData.Project);

            yield return WaitSubWorkflow(SubWorkFlow);
            yield return WaitSubWorkflows(SubWorkFlow,SubWorkFlow);
            yield return WaitFirstSubWorkflow(SubWorkFlow,SubWorkFlow);

            Console.WriteLine("All three approved");
        }

        private async IAsyncEnumerable<EventWaiting> SubWorkFlow()
        {
            await AskOwnerToApprove(InstanceData.Project);
            yield return WaitEvent(OwnerApproval)
                .Match<ManagerApprovalEvent>(result => result.ProjectId == InstanceData.Project.Id)
                .SetProp(() => InstanceData.OwnerApprovalResult);
            if (InstanceData.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Owner");
                yield break;
            }

            await AskSponsorToApprove(InstanceData.Project);
            yield return WaitEvent(SponsorApproval)
                .Match<ManagerApprovalEvent>(result => result.ProjectId == InstanceData.Project.Id)
                .SetProp(() => InstanceData.SponsorApprovalResult);
            if (InstanceData.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(InstanceData.Project);
            yield return WaitEvent(ManagerApproval)
                .Match<ManagerApprovalEvent>(result => result.ProjectId == InstanceData.Project.Id)
                .SetProp(() => InstanceData.ManagerApprovalResult);
            if (InstanceData.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three approved");
        }
    }
}
