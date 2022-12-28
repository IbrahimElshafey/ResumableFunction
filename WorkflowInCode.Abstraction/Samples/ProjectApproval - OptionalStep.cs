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
                ProjectOwner.AskApproval().Accept,
                ProjectSponsor.AskApproval().NoWait,
                ProjectManager.AskApproval().Accept);

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
        [ProcessNode(WaitEvent = null)]
        ManagerResponse AskApproval();
        
        [ProcessOutputNode(nameof(AskApproval))]
        ProcessOutputNode Accept { get; }
        ProcessOutputNode Reject { get; }

    }
    public interface Project : IProcess
    {
        public Project Created { get; }

        ProcessOutputNode InformAllAboutRejection();
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class ProcessNodeAttribute : Attribute
    {
        public Type WaitEvent { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    sealed class ProcessOutputNodeAttribute : Attribute
    {
        public string ProcessNodeName { get; }
        public ProcessOutputNodeAttribute(string processNodeName)
        {
            ProcessNodeName = processNodeName;
        }
    }
}
