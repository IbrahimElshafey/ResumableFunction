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
    public class ProjectApprovalSubFunction : ProjectApproval
    {
        public ProjectApprovalSubFunction(ProjectApprovalContextData data) : base(data)
        {
        }

        protected override async IAsyncEnumerable<EventWaitingResult> RunFunction()
        {

            await Task.Delay(100);
            yield return WaitEvent(typeof(ProjectRequestedEvent), "ProjectRequested").SetProp(() => FunctionData.Project);

            yield return WaitSubFunction(SubFunction);
            yield return WaitSubFunctions(SubFunction,SubFunction);
            yield return WaitFirstSubFunction(SubFunction,SubFunction);

            Console.WriteLine("All three approved");
        }

        private async IAsyncEnumerable<SingleEventWaiting> SubFunction()
        {
            await AskOwnerToApprove(FunctionData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "OwnerApproval")
                .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                .SetProp(() => FunctionData.OwnerApprovalResult);
            if (FunctionData.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(FunctionData.Project, "Owner");
                yield break;
            }

            await AskSponsorToApprove(FunctionData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "SponsorApproval")
                .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                .SetProp(() => FunctionData.SponsorApprovalResult);
            if (FunctionData.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(FunctionData.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(FunctionData.Project);
            yield return WaitEvent(typeof(ManagerApprovalEvent), "ManagerApproval")
                .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                .SetProp(() => FunctionData.ManagerApprovalResult);
            if (FunctionData.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(FunctionData.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three approved");
        }
    }
}
