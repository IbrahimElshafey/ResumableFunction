using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
    public class ProjectApprovalWaitMany : ProjectApproval
    {
        
        protected override async IAsyncEnumerable<EventWaitingResult> Start()
        {
            yield return WaitEvent(typeof(ProjectRequestedEvent), "ProjectRequested").SetProp(() => FunctionData.Project);
            if (FunctionData.Project is not null)
            {
                await AskOwnerToApprove(FunctionData.Project);
                await AskSponsorToApprove(FunctionData.Project);
                await AskManagerToApprove(FunctionData.Project);

                //different event types and use optional
                yield return WaitEvents(
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "ProjectRequested")
                       .Match<ProjectRequestedEvent>(result => result.Id == FunctionData.Project.Id)
                       .SetProp(() => FunctionData.Project)
                       .SetOptional(),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                       .SetProp(() => FunctionData.OwnerApprovalResult));

                //same type
                yield return WaitEvents(
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                         .SetProp(() => FunctionData.OwnerApprovalResult),
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                         .SetProp(() => FunctionData.SponsorApprovalResult),
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                         .SetProp(() => FunctionData.ManagerApprovalResult)
                     );

                //wait first matched event in group
                yield return WaitAnyEvent(
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                       .SetProp(() => FunctionData.OwnerApprovalResult),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                       .SetProp(() => FunctionData.SponsorApprovalResult),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                       .SetProp(() => FunctionData.ManagerApprovalResult)
                   );

                //wait all but if two of them matched then return
                yield return WaitEvents(
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                      .SetProp(() => FunctionData.OwnerApprovalResult),
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                      .SetProp(() => FunctionData.SponsorApprovalResult),
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
                      .SetProp(() => FunctionData.ManagerApprovalResult)
                  ).WhenCount(count => count == 2);

                var allManagerResponse = FunctionData.ManagerApprovalResult.Accepted == FunctionData.OwnerApprovalResult.Accepted == FunctionData.SponsorApprovalResult.Accepted;
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
