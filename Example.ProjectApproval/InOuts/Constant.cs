using Example.ProjectApproval;

namespace Example.InOuts
{
    public class Constant
    {
        public static string EventProviderName => new ExampleApiEventProvider().EventProviderName;
        public const string ProjectSumbittedEvent = "POST#/api/project/SumbitProject";
        public const string ManagerApprovalEvent = "POST#/api/project/ManagerApproval";
    }
}