using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */

    public class ProjectApproval : WorkflowInstance<ProjectApprovalContextData>
    {
        public ProjectRequestedEvent ProjectRequested;
        public ManagerApprovalEvent OwnerApproval;
        public ManagerApprovalEvent SponsorApproval;
        public ManagerApprovalEvent ManagerApproval;
        public ProjectApproval(ProjectRequestedEvent p, ManagerApprovalEvent po, ManagerApprovalEvent ps, ManagerApprovalEvent pm)
        {
            ProjectRequested = p;
            OwnerApproval = po;
            SponsorApproval = ps;
            ManagerApproval = pm;
            InstanceData = new ProjectApprovalContextData();
        }

        protected override async IAsyncEnumerable<WorkflowEvent> RunWorkflow()
        {
            //any class that inherit WorkflowInstance<T> has the methods
            //WaitEvent,WaitFirstEvent in a collection,WaitEvents and SaveInstanceData

            //the engine will wait for ProjectRequested event
            //no match function because it's the first one
            //context prop is prop in InstanceData that we will set with event result data
            yield return WaitEvent(
                    eventToWait: ProjectRequested,
                    matchFunction: null,
                    contextProp: () => InstanceData.Project);
            //the compiler will save state after executing the previous return
            //and wiating for the event
            //it will continue from the line below when event cames


            //InstanceData.Project is set by the previous event
            //we will initiate a task for Owner and wait to the Owner response
            //That matching function correlates the event to the right instance
            //The matching function will be translated to query language "MongoDB query for example" by the engine to search the active instance.
            await AskOwnerToApprove(InstanceData.Project);
            yield return WaitEvent(
                OwnerApproval,
                result => result.ProjectId == InstanceData.Project.Id,
                () => InstanceData.OwnerApprovalResult);
            if (InstanceData.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project,"Owner");
                yield break;
            }

            await AskSponsorToApprove(InstanceData.Project);
            yield return WaitEvent(
             SponsorApproval,
             result => result.ProjectId == InstanceData.Project.Id,
             () => InstanceData.SponsorApprovalResult);
            if (InstanceData.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(InstanceData.Project);
            yield return WaitEvent(
             ManagerApproval,
             result => result.ProjectId == InstanceData.Project.Id,
             () => InstanceData.ManagerApprovalResult);
            if (InstanceData.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three aproved");
        }

        private async Task ProjectRejected(Project project, string v)
        {
            await Task.Delay(500);
        }

        private async Task AskManagerToApprove(Project project)
        {
            await Task.Delay(1000);
        }

        private async Task AskSponsorToApprove(Project project)
        {
            await Task.Delay(1000);
        }

        private async Task AskOwnerToApprove(Project project)
        {
            await Task.Delay(1000);
        }
    }
    public class ProjectApprovalContextData
    {
        public Project Project { get; set; }
        public ProjectApprovalResult OwnerApprovalResult { get; set; }
        public ProjectApprovalResult SponsorApprovalResult { get; set; }
        public ProjectApprovalResult ManagerApprovalResult { get; set; }
    }

    public class ManagerApprovalEvent : Event
    {
        public object EventData { get; set; }
    }

    public class ProjectRequestedEvent : Event
    {
        public object EventData { get; set; }
    }

    public record ProjectApprovalResult(int ProjectId, bool Accepted, bool Rejected);

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
    }
}
