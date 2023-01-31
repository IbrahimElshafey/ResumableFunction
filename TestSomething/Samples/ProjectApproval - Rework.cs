using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    public class ProjectApprovalSubFunction_Rework : ProjectApproval
    {
        private const string ProjectSubmittedForReview = "ProjectRequested";

        public override async IAsyncEnumerable<Wait> Start()
        {

            await Task.Delay(100);
            yield return WaitEvent<ProjectRequestedEvent>(ProjectSubmittedForReview).SetData(() => Project);

            await AskOwnerToApprove(Project);
            yield return WaitEvent<ManagerApprovalEvent>("OwnerApproval")
                .Match(result => result.ProjectId == Project.Id)
                .SetData(() => OwnerApprovalResult);
            if (OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(Project, "Owner");
                yield return Replay<ProjectRequestedEvent>();
            }

            await AskSponsorToApprove(Project);
            yield return WaitEvent<ManagerApprovalEvent>("SponsorApproval")
                .Match(result => result.ProjectId == Project.Id)
                .SetData(() => SponsorApprovalResult);
            if (SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(Project, "Sponsor");
                yield return Replay("OwnerApproval");
            }

            await AskManagerToApprove(Project);
            yield return WaitEvent<ManagerApprovalEvent>("ManagerApproval")
                .Match(result => result.ProjectId == Project.Id)
                .SetData(() => ManagerApprovalResult);
            if (ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(Project, "Manager");
                yield return Replay("SponsorApproval");
            }

            Console.WriteLine("All three approved");
        }

    }
}
