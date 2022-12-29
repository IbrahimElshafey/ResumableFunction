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
        public ProjectSubmitter Project;
        public ManagerApprovalProcess ProjectOwner;
        public ManagerApprovalProcess ProjectSponsor;
        public ManagerApprovalProcess ProjectManager;
        public ProjectApproval2(ProjectSubmitter p, ManagerApprovalProcess po, ManagerApprovalProcess ps, ManagerApprovalProcess pm)
        {
            Project = p;
            ProjectOwner = po;
            ProjectSponsor = ps;
            ProjectManager = pm;
            var wf = new WorkflowDefinition();
            wf.DefineProcesses(() => new IWorkFlowProcess[]
            {
                Project,
                ProjectOwner,
                ProjectSponsor,
                ProjectManager
            });
            wf.DefinePaths(
            () => Path("Project Approval",
                Project.Created,
                ProjectOwner.AskApproval().Accepted,
                ProjectSponsor.AskApproval().Accepted,
                ProjectManager.AskApproval().Accepted),
            () => Path("Project Rejected",
                Path("Any Manager Send Reject", ProjectOwner.Rejected,
                    ProjectSponsor.Rejected,
                    ProjectManager.Rejected),
                Project.InformAllAboutRejection()));
        }
    }
}
