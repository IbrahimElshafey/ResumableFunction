using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
using static WorkflowInCode.Abstraction.Engine.Workflow;
namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    public class ProjectApproval
    {
        public ProjectRequest ProjectRequest;
        public ManagerApproval OwnerApproval;
        public ManagerApproval SponsorApproval;
        public ManagerApproval ProjectManagerApproval;
        public ProjectApproval(ProjectRequest p, ManagerApproval po, ManagerApproval ps, ManagerApproval pm)
        {
            ProjectRequest = p;
            OwnerApproval = po;
            SponsorApproval = ps;
            ProjectManagerApproval = pm;
            //
            var projectApprovalWorkFlow = new WorkflowDefinition();
            projectApprovalWorkFlow.DefineProcesses(() => new LongRunningTask[]
            {
                ProjectRequest,
                OwnerApproval,
                SponsorApproval,
                ProjectManagerApproval
            });

            projectApprovalWorkFlow.DefinePaths(
                () => Path("Project Approval",
                ProjectRequest.Result.Created,
                Path("",
                    OwnerApproval.Initiate(ProjectRequest.Project).Result.Accepted,
                    SponsorApproval.Initiate(ProjectRequest.Project)).Parallel(),
                    ProjectManagerApproval.Initiate(ProjectRequest.Project).Result.Accepted).Sequential(),

                () => Path("Project Rejected",
                Path("Any Manager Send Reject",
                    OwnerApproval.Result.Rejected,
                    SponsorApproval.Result.Rejected,
                    ProjectManagerApproval.Result.Rejected).FirstMatch(),
                ProjectRequest.InformAllAboutRejection()));
            
        }
    }

    public class ManagerApproval : LongRunningTask<Project, ProjectApprovalResult>
    {
    }

    public class ProjectRequest:LongRunningTask<Project, ProjectCreationResult>
    {
        [PersistData]
        public Project Project { get; }
        public bool InformAllAboutRejection() => true;
    }

    public record ProjectCreationResult(bool Created,bool CreationFailed);
    public record ProjectApprovalResult(int ProjectId, bool Accepted, bool Rejected);

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
    }
}
