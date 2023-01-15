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
    public class ProjectApprovalSubWorkflow : ProjectApproval
    {
        public ProjectApprovalSubWorkflow(ProjectApprovalContextData data) : base(data)
        {
        }

        protected override async IAsyncEnumerable<EventWaitingResult> RunWorkflow()
        {

            await Task.Delay(100);
            yield return WaitEvent(typeof(ProjectRequestedEvent), "ProjectRequested").SetProp(() => InstanceData.Project);

            yield return WaitSubWorkflow(SubWorkFlow);
            yield return WaitSubWorkflows(SubWorkFlow,SubWorkFlow);
            yield return WaitFirstSubWorkflow(SubWorkFlow,SubWorkFlow);

            Console.WriteLine("All three approved");
        }

        private async IAsyncEnumerable<SingleEventWaiting> SubWorkFlow()
        {
            await AskOwnerToApprove(InstanceData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "OwnerApproval")
                .Match<ManagerApprovalEvent>(result => result.ProjectId == InstanceData.Project.Id)
                .SetProp(() => InstanceData.OwnerApprovalResult);
            if (InstanceData.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Owner");
                yield break;
            }

            await AskSponsorToApprove(InstanceData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "SponsorApproval")
                .Match<ManagerApprovalEvent>(result => result.ProjectId == InstanceData.Project.Id)
                .SetProp(() => InstanceData.SponsorApprovalResult);
            if (InstanceData.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(InstanceData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "ManagerApproval")
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
