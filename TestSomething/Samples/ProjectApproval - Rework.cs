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
            yield return WaitEvent<ProjectRequestedEvent>(ProjectSubmittedForReview).SetData(() => Data.Project);

            await AskOwnerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("OwnerApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.OwnerApprovalResult);
            if (Data.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Owner");
                yield return Replay<ProjectRequestedEvent>();
            }

            await AskSponsorToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("SponsorApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.SponsorApprovalResult);
            if (Data.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Sponsor");
                yield return Replay("OwnerApproval");
            }

            await AskManagerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("ManagerApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.ManagerApprovalResult);
            if (Data.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Manager");
                yield return Replay("SponsorApproval");
            }

            Console.WriteLine("All three approved");
        }

    }
}
