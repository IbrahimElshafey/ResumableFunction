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
            yield return WaitEvent(
                eventToWait:ProjectRequested,
                matchFunction:null,
                contextProp: () => InstanceData.Project);
            if (InstanceData.Project is not null)
            {
                await AskOwnerToApprove(InstanceData.Project);
                yield return WaitEvent(
                    OwnerApproval,
                    result => result.ProjectId == InstanceData.Project.Id,
                    () => InstanceData.OwnerApprovalResult);

                if (InstanceData.OwnerApprovalResult.Accepted)
                {
                    await AskSponsorToApprove(InstanceData.Project);
                    yield return WaitEvent(
                     SponsorApproval,
                     result => result.ProjectId == InstanceData.Project.Id,
                     () => InstanceData.SponsorApprovalResult);
                    if (InstanceData.SponsorApprovalResult.Accepted)
                    {
                        await AskManagerToApprove(InstanceData.Project);
                        yield return WaitEvent(
                         ManagerApproval,
                         result => result.ProjectId == InstanceData.Project.Id,
                         () => InstanceData.ManagerApprovalResult);
                        Console.WriteLine("Continue");
                    }

                }
            }
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
        public Project Project { get;  set; }
        public ProjectApprovalResult OwnerApprovalResult { get;  set; }
        public ProjectApprovalResult SponsorApprovalResult { get;  set; }
        public ProjectApprovalResult ManagerApprovalResult { get;  set; }
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
