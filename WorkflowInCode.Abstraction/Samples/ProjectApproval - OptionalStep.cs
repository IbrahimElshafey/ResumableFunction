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
         * موافقة راعي المشروع ليست إجبارية, قد يرد أو لا يرد أبداً
         * 
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
            ProjectOwner = po;
            ProjectSponsor = ps;
            ProjectManager = pm;
            //
            Path("Project Approval",
                Project.Created,
                ProjectOwner.WakeUp().Accept,
                ProjectSponsor.WakeUp().NoWait,
                ProjectManager.WakeUp().Accept);

            Path("Project Rejected",
                Combine("Any Manager Send Reject", Selection.FirstOne,
                    ProjectOwner.Reject,
                    ProjectSponsor.Reject,
                    ProjectManager.Reject),
                Project.InformAllAboutRejection()); ;
        }
    }

    public interface ManagerResponse : IProcess
    {
        [WaitResult]
        new ManagerResponse WakeUp();
        ProcessOutputNode Accept { get; }
        ProcessOutputNode Reject { get; }

    }
    public interface Project : IProcess
    {
        public Project Created { get; }

        ProcessOutputNode InformAllAboutRejection();
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class WaitResultAttribute : Attribute
    {
    }
}
