using ResumableFunction.Abstraction.InOuts;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ResumableFunction.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    //ProjectApprovalFunctionData is the data that will bes saved to the database 
    //When the engine match an event it will load the related Function class and set the 
    //FunctionData property by loading it from database
    //No other state saved just the FunctionData and Function author must keep that in mind
    //We can't depend on automatic serialize for state becuse compiler may remove fields and variables we defined
    public class ProjectApproval : ResumableFunction<ProjectApprovalFunctionData>
    {
        public override Task OnFunctionEnd()
        {
            Console.WriteLine("ProjectApproval.OnFunctionEnd called.");
            return base.OnFunctionEnd();
        }

        //any inherited ResumableFunction must implement 'Start'
        public override async IAsyncEnumerable<Wait> Start()
        {
            Console.WriteLine(this.GetType());
            //yield return await Function(() => SubFunction());
            yield return await Functions(
              "Wait All Functions to End", 
              FunctionGroup(
                () => SubFunction1(),
                () => SubFunction2(),
                () => SubFunction3()));
            //any class that inherit FunctionInstance<T> has the methods
            //WaitEvent,WaitFirstEvent in a collection,WaitEvents and SaveFunctionData

            //the engine will wait for ProjectRequested event
            //no match function because it's the first one
            //context prop is prop in FunctionData that we will set with event result data
            yield return
                WaitEvent<ProjectRequestedEvent>("ProjectCreated")
                .SetData(() => Data.Project);
            //the compiler will save state after executing the previous return
            //and wiating for the event
            //it will continue from the line below when event cames


            //FunctionData.Project is set by the previous event
            //we will initiate a task for Owner and wait to the Owner response
            //That matching function correlates the event to the right instance
            //The matching function will be translated to query language "MongoDB query for example" by the engine to search the active instance.
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

        protected async Task ProjectRejected(ProjectRequestedEvent project, string v)
        {
            await Task.Delay(500);
        }

        protected async Task AskManagerToApprove(ProjectRequestedEvent project)
        {
            await Task.Delay(1000);
        }

        protected async Task AskSponsorToApprove(ProjectRequestedEvent project)
        {
            await Task.Delay(1000);
        }

        protected async Task AskOwnerToApprove(ProjectRequestedEvent project)
        {
            await Task.Delay(1000);
        }
        private async IAsyncEnumerable<Wait> SubFunction3()
        {
            yield return WaitAnyEvent(
                "WaitAnyManagerApproval",
                new EventWait<ManagerApprovalEvent>("OwnerApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.OwnerApprovalResult),
                new EventWait<ManagerApprovalEvent>("SponsorApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.SponsorApprovalResult),
                new EventWait<ManagerApprovalEvent>("ManagerApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.ManagerApprovalResult)
                );
        }
        private async IAsyncEnumerable<Wait> SubFunction2()
        {
            yield return WaitEvents(
                "WaitAllManagers",
                new EventWait<ManagerApprovalEvent>("OwnerApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.OwnerApprovalResult),
                new EventWait<ManagerApprovalEvent>("SponsorApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.SponsorApprovalResult),
                new EventWait<ManagerApprovalEvent>("ManagerApproval_SubFunction")
                    .Match(result => result.ProjectId == Data.Project.Id)
                    .SetData(() => Data.ManagerApprovalResult)
                );
        }
        private async IAsyncEnumerable<Wait> SubFunction1()
        {
            await AskOwnerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("OwnerApproval_SubFunction")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.OwnerApprovalResult);
            if (Data.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Owner_SubFunction");
                yield break;
            }

            await AskSponsorToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("SponsorApproval_SubFunction")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.SponsorApprovalResult);
            if (Data.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Sponsor_SubFunction");
                yield break;
            }

            await AskManagerToApprove(Data.Project);
            yield return WaitEvent<ManagerApprovalEvent>("ManagerApproval_SubFunction")
                .Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.ManagerApprovalResult);
            if (Data.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(Data.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three approved");
        }

        internal EventWait EventWait()
        {
            var result =
                WaitEvent<ManagerApprovalEvent>("OwnerApproval_SubFunction")
                .Match(result => result.ProjectId == Data.Project.Id && result.ProjectId == int.Parse("11"))
                //.Match(result => result.ProjectId == Data.Project.Id)
                .SetData(() => Data.OwnerApprovalResult);
            //.SetData(() => Data.Approvals);
            result.FunctionRuntimeInfo = new FunctionRuntimeInfo
            {
                Data = new ProjectApprovalFunctionData
                {
                    Project = new ProjectRequestedEvent { Id = 11 },
                    SponsorApprovalResult = new ManagerApprovalEvent
                    {
                        ProjectId = 11,
                        Accepted = true,
                        Rejected = false
                    }
                },
                DataType = typeof(ProjectApprovalFunctionData),
                FunctionId = Guid.NewGuid(),
                InitiatedByClassType = GetType(),
                //FunctionsStates = new Dictionary<string, int> { { "Start", 1 } }
            };
            result.EventData = new ManagerApprovalEvent
            {
                ProjectId = 11,
                Accepted = true,
                Rejected = false
            };
            result.InitiatedByFunctionName = nameof(Start);
            return result;
        }
        internal Expression Expression1()
        {
            Expression<Func<ManagerApprovalEvent, bool>> match1 =
                (result) =>
                    result.ProjectId > Math.Min(5, 10) &&
                    //result.PreviousApproval.Equals(Data.ManagerApprovalResult) &&
                    result.ProjectId == Data.Project.Id;
            return match1;
        }
    }
}
