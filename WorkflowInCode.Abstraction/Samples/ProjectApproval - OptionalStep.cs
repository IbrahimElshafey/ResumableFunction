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
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
         */
    public class ProjectApproval
    {
        public ProjectSubmitter ProjectSubmitter;
        public ManagerApprovalProcess ProjectOwner;
        public ManagerApprovalProcess ProjectSponsor;
        public ManagerApprovalProcess ProjectManager;
        public ProjectApproval(ProjectSubmitter p, ManagerApprovalProcess po, ManagerApprovalProcess ps, ManagerApprovalProcess pm)
        {
            ProjectSubmitter = p;
            ProjectOwner = po;
            ProjectSponsor = ps;
            ProjectManager = pm;
            //
            Path("Project Approval",
                ProjectSubmitter.Created,
                Path("",
                    ProjectOwner.AskApproval(ProjectSubmitter.Project).Accepted,
                    ProjectSponsor.AskApproval(ProjectSubmitter.Project)).Parallel(),
                ProjectManager.AskApproval(ProjectSubmitter.Project).Accepted).Sequential();

            Path("Project Rejected",
                Path("Any Manager Send Reject", ProjectOwner.Rejected,
                    ProjectSponsor.Rejected,
                    ProjectManager.Rejected),
                ProjectSubmitter.InformAllAboutRejection());
        }
    }

    public interface ManagerApprovalProcess : IWorkFlowProcess
    {
       

        ManagerApprovalProcess AskApproval(Project project);

        
        ManagerApprovalProcess SendApproval(Project project);

        
        bool Accepted { get; }

        
        bool Rejected { get; }

    }
    public interface ProjectSubmitter : IWorkFlowProcess
    {
        [PersistData]
        Project Project { get; }
        ProjectSubmitter Create(Project project);

        
        bool Created { get; }

        
        bool InformAllAboutRejection();
    }

    public class ProjectApprovalResult
    {
        public int ProjectId { get; set; }
        public bool Accepted { get; set; }
        public bool Rejected { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
    }
}
