using System;
using System.Collections.Generic;
using System.Linq;
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
         */
    public class ProjectApproval2
    {
        public ProjectRequest ProjectRequest;
        public ManagerApproval OwnerApproval;
        public ManagerApproval SponsorApproval;
        public ManagerApproval ManagerApproval;
        public ProjectApproval2(ProjectRequest p, ManagerApproval po, ManagerApproval ps, ManagerApproval pm)
        {
            ProjectRequest = p;
            OwnerApproval = po;
            SponsorApproval = ps;
            ManagerApproval = pm;
            var wf = new WorkflowDefinition();
            wf.DefineProcesses(() => new LongRunningTask[]
            {
                ProjectRequest,
                OwnerApproval,
                SponsorApproval,
                ManagerApproval
            });

            wf.DefinePaths(
            () => Path("Project Approval",
                ProjectRequest.Result.Created,
                OwnerApproval.Initiate(ProjectRequest.Project).Result.Accepted,
                SponsorApproval.Initiate(ProjectRequest.Project).Result.Accepted,
                ManagerApproval.Initiate(ProjectRequest.Project).Result.Accepted),
            () => Path("Project Rejected",
                Path("Any Manager Send Reject",
                    OwnerApproval.Result.Rejected,
                    SponsorApproval.Result.Rejected,
                    ManagerApproval.Result.Rejected).FirstMatch(),
                ProjectRequest.InformAllAboutRejection()));
        }
    }
}
