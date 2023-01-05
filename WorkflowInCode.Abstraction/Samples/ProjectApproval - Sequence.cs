using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    public class ProjectApproval : Workflow
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

            Wait(ProjectRequest);
            if (ProjectRequest.Result != null)
            {
                var ownerInitiation = OwnerApproval.Initiate(ProjectRequest.Result);
                if (ownerInitiation is false) { LogError("Owner Approval task failed to initiate"); return; }
                var approvalResult = Wait(OwnerApproval);
                if (approvalResult.Accepted)
                {
                    var sponsorInitiatio = SponsorApproval.Initiate(ProjectRequest.Result);
                    if (sponsorInitiatio is false) { LogError("Sponsor Approval task failed to initiate"); return; }
                    approvalResult = Wait(SponsorApproval);
                    if(approvalResult.Accepted)

                }

            }
            //
            //var projectApprovalWorkFlow = new WorkflowDefinition();
            //projectApprovalWorkFlow.DefineProcesses(() => new LongRunningTask[]
            //{
            //    ProjectRequest,
            //    OwnerApproval,
            //    SponsorApproval,
            //    ProjectManagerApproval
            //});

            //projectApprovalWorkFlow.DefinePaths(
            //    () => Path("Project Approval",
            //    ProjectRequest.Created,
            //    Path("",
            //        OwnerApproval.Initiate(ProjectRequest.Project).Result.Accepted,
            //        SponsorApproval.Initiate(ProjectRequest.Project)).Parallel(),
            //        ProjectManagerApproval.Initiate(ProjectRequest.Project).Result.Accepted).Sequential(),

            //    () => Path("Project Rejected",
            //    Path("Any Manager Send Reject",
            //        OwnerApproval.Result.Rejected,
            //        SponsorApproval.Result.Rejected,
            //        ProjectManagerApproval.Result.Rejected).FirstMatch(),
            //    ProjectRequest.InformAllAboutRejection()));

        }
    }


    public class ManagerApproval : LongRunningTask<bool, ProjectApprovalResult>
    {
    }

    public class ProjectRequest : ISubscribedEvent<Project>
    {
        public bool InformAllAboutRejection() => true;

        public Project Result => null;
    }

    public record ProjectCreationResult(bool Created, bool CreationFailed);
    public record ProjectApprovalResult(int ProjectId, bool Accepted, bool Rejected);

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
    }
}
