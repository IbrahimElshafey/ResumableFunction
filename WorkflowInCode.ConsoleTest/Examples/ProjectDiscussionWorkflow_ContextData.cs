namespace WorkflowInCode.ConsoleTest.Examples
{
    public class ProjectDiscussionWorkflow_ContextData
    {
        public dynamic RequestBasicData { get; internal set; }
        public List<dynamic> Members { get; internal set; }
        public List<dynamic> Topics { get; internal set; }
        public dynamic CurrentTopic { get; internal set; }
        public List<dynamic> ChefApprovals { get; internal set; }
        public List<dynamic> MemberApprovals { get; internal set; }
        public int CurrentTopicVotedMembersCount { get; internal set; }
        public dynamic ChefFinalApproval { get; internal set; }
    }
}
