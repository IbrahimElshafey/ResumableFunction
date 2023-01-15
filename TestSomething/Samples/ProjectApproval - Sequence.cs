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
    //ProjectApprovalContextData is the data that will bes saved to the database 
    //When the engine match an event it will load the related workflow class and set the 
    //InstanceData property by loading it from database
    //No other state saved just the InstanceData and workflow author must keep that in mind
    //We can't depend on automatic serialize for state becuse compiler may remove fields and variables we defined
    public class ProjectApproval : WorkflowInstance<ProjectApprovalContextData>
    {
        public ProjectApproval(ProjectApprovalContextData data) : base(data)
        {
        }

        //public ProjectApproval(ProjectRequestedEvent p, ManagerApprovalEvent po, ManagerApprovalEvent ps, ManagerApprovalEvent pm)
        //{
        //    ProjectRequested = p;
        //    OwnerApproval = po;
        //    SponsorApproval = ps;
        //    ManagerApproval = pm;
        //    InstanceData = new ProjectApprovalContextData();
        //}

        protected override async IAsyncEnumerable<EventWaitingResult> RunWorkflow()
        {
            //any class that inherit WorkflowInstance<T> has the methods
            //WaitEvent,WaitFirstEvent in a collection,WaitEvents and SaveInstanceData

            //the engine will wait for ProjectRequested event
            //no match function because it's the first one
            //context prop is prop in InstanceData that we will set with event result data
            yield return WaitEvent(typeof(ProjectRequestedEvent),"ProjectCreated").SetProp(() => InstanceData.Project);
            //the compiler will save state after executing the previous return
            //and wiating for the event
            //it will continue from the line below when event cames


            //InstanceData.Project is set by the previous event
            //we will initiate a task for Owner and wait to the Owner response
            //That matching function correlates the event to the right instance
            //The matching function will be translated to query language "MongoDB query for example" by the engine to search the active instance.
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
    }
    public class ProjectApprovalContextData
    {
        public ProjectRequestedEvent Project { get; set; }
        public ManagerApprovalEvent OwnerApprovalResult { get; set; }
        public ManagerApprovalEvent SponsorApprovalResult { get; set; }
        public ManagerApprovalEvent ManagerApprovalResult { get; set; }
    }


    public class ManagerApprovalEvent: IEventData
    {
       public int ProjectId{get;set;}
       public bool Accepted{get;set;}
       public bool Rejected{get;set;}
        public string EventProviderName => Const.CurrentEventProvider;
    }

    public class ProjectRequestedEvent:IEventData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }

        public string EventProviderName => Const.CurrentEventProvider;
    }

    public class Const
    {
        public const string CurrentEventProvider = "WebHookProvider";
    }
}
