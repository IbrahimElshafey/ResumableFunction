using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
using static WorkflowInCode.Abstraction.Engine.WorkflowEngine;
namespace WorkflowInCode.Abstraction.Samples
{
    /*
         * بعد إضافة مشروع يتم ارسال دعوة لمالك المشروع
         * ننتظر موافقة مالك المشروع ثم موافقة راعي المشروع ثم موافقة مدير المشروع بشكل متتابع
         * إذا رفض أحدهم يتم إلغاء المشروع وإعلام الآخرين
         */
    public class ProjectApproval
    {
        public Project Project;
        public ManagerResponse ProjectOwner;
        public ManagerResponse ProjectSponsor;
        public ManagerResponse ProjectManager;
        public ProjectApproval(Project p, ManagerResponse po, ManagerResponse ps, ManagerResponse pm)
        {
            Project = p;
            ProjectOwner=po;
            ProjectSponsor=ps;
            ProjectManager=pm;
            //
            Path("Project Approval", 
                Project.Created,
                ProjectOwner.WakeUp().Accept,
                ProjectSponsor.WakeUp().Accept,
                ProjectManager.WakeUp().Accept);

            Path("Project Rejected",
                SelectOf("Any Manager Send Reject", Selection.FirstOne, 
                    ProjectOwner.Reject,
                    ProjectSponsor.Reject,
                    ProjectManager.Reject),
                Project.InformAllAboutRejection()); ;
        }
    }

    public interface ManagerResponse : IProcess
    {
        new ManagerResponse WakeUp();
        ManagerResponse Accept { get; }
        ManagerResponse Reject { get; }

    }
    public interface Project : IProcess
    {
        public Project Created { get;}

        Project InformAllAboutRejection();
    }
}
