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
            yield return WaitEvent(typeof(ProjectRequestedEvent), "ProjectRequested").SetProp(() => Data.Project);
            if (Data.Project is not null)
            {
                await AskOwnerToApprove(Data.Project);
                await AskSponsorToApprove(Data.Project);
                await AskManagerToApprove(Data.Project);

                //different event types and use optional
                yield return WaitEvents(
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "ProjectRequested")
                       .Match<ProjectRequestedEvent>(result => result.Id == Data.Project.Id)
                       .SetProp(() => Data.Project)
                       .SetOptional(),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.OwnerApprovalResult));

                //same type
                yield return WaitEvents(
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.OwnerApprovalResult),
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.SponsorApprovalResult),
                     new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                         .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.ManagerApprovalResult)
                     );

                //wait first matched event in group
                yield return WaitAnyEvent(
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.OwnerApprovalResult),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.SponsorApprovalResult),
                   new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                       .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.ManagerApprovalResult)
                   );

                //wait all but if two of them matched then return
                yield return WaitEvents(
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "OwnerApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                      .SetProp(() => Data.OwnerApprovalResult),
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "SponsorApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                      .SetProp(() => Data.SponsorApprovalResult),
                  new SingleEventWaiting(typeof(ManagerApprovalEvent), "ManagerApproval")
                      .Match<ManagerApprovalEvent>(result => result.ProjectId == Data.Project.Id)
                      .SetProp(() => Data.ManagerApprovalResult)
                  ).WhenCount(count => count == 2);

                var allManagerResponse = Data.ManagerApprovalResult.Accepted == Data.OwnerApprovalResult.Accepted == Data.SponsorApprovalResult.Accepted;
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
