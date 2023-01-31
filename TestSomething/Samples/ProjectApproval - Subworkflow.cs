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

        public override async IAsyncEnumerable<Wait> Start()
        {

            await Task.Delay(100);
            yield return WaitEvent<ProjectRequestedEvent>("ProjectRequested").SetData(() => Data.Project);

            yield return await Function("Function Name", () => SubFunction());

            yield return await Functions(
                "Wait All Functions to End",
                FunctionGroup(
                    () => SubFunction(),
                    () => SubFunction()));

            yield return await AnyFunction(
                "Wait any function to end",
                FunctionGroup(
                    () => SubFunction(),
                    () => SubFunction()));

            Console.WriteLine("All three approved");
        }

        private async IAsyncEnumerable<EventWait> SubFunction()
        {
            await AskOwnerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("OwnerApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.OwnerApprovalResult);
            if (Data.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Owner");
                yield break;
            }

            await AskSponsorToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("SponsorApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.SponsorApprovalResult);
            if (Data.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("ManagerApproval")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.ManagerApprovalResult);
            if (Data.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three approved");
        }
    }
}
