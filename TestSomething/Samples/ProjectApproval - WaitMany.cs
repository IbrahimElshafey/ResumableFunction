using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public override async IAsyncEnumerable<Wait> Start()
        {
            yield return WaitEvent<ProjectRequestedEvent>("ProjectRequested").SetProp(() => Data.Project);
            if (Data.Project is not null)
            {
                await AskOwnerToApprove(Data.Project);
                await AskSponsorToApprove(Data.Project);
                await AskManagerToApprove(Data.Project);

               


                //different event types and use optional
                yield return WaitEvents(
                   "WaitAllManagers",
                   new EventWait<ProjectRequestedEvent>("ProjectRequested")
                       .Match(result => result.Id == Data.Project.Id)
                       .SetProp(() => Data.Project)
                       .SetOptional(),
                   new EventWait<ManagerApprovalEvent>("OwnerApproval")
                       .Match(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.OwnerApprovalResult));

                //same type
                yield return WaitEvents(
                     "WaitAllManagers", 
                     new EventWait<ManagerApprovalEvent>("OwnerApproval")
                         .Match(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.OwnerApprovalResult),
                     new EventWait<ManagerApprovalEvent>("SponsorApproval")
                         .Match(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.SponsorApprovalResult),
                     new EventWait<ManagerApprovalEvent>("ManagerApproval")
                         .Match(result => result.ProjectId == Data.Project.Id)
                         .SetProp(() => Data.ManagerApprovalResult)
                     );

                //wait first matched event in group
                yield return WaitAnyEvent(
                   "WaitAnyManagerApproval", 
                   new EventWait<ManagerApprovalEvent>("OwnerApproval")
                       .Match(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.OwnerApprovalResult),
                   new EventWait<ManagerApprovalEvent>("SponsorApproval")
                       .Match(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.SponsorApprovalResult),
                   new EventWait<ManagerApprovalEvent>("ManagerApproval")
                       .Match(result => result.ProjectId == Data.Project.Id)
                       .SetProp(() => Data.ManagerApprovalResult)
                   );

                //wait all but if two of them matched then return
                yield return WaitEvents(
                  "WaitAllManagers",
                  new EventWait<ManagerApprovalEvent>("OwnerApproval")
                      .Match(result => result.ProjectId == Data.Project.Id)
                      .SetProp(() => Data.OwnerApprovalResult),
                  new EventWait<ManagerApprovalEvent>("SponsorApproval")
                      .Match(result => result.ProjectId == Data.Project.Id)
                      .SetProp(() => Data.SponsorApprovalResult),
                  new EventWait<ManagerApprovalEvent>("ManagerApproval")
                      .Match(result => result.ProjectId == Data.Project.Id)
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
