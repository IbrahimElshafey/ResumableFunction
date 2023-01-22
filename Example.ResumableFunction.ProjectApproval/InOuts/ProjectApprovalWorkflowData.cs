using Example.InOuts;

namespace Example.ProjectApproval.InOuts
{
    public class ProjectApprovalWorkflowData
    {
        public ProjectSumbitted ProjectSumbitted { get; internal set; }
        public ManagerApprovalEvent ManagerApprovalEvent { get; internal set; }
    }
}