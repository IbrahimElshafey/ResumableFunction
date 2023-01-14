namespace WorkflowInCode.Abstraction.InOuts
{
    public class WorkflowRuntimeData
    {
        public Guid InstanceId { get; set; }
        public string Status { get; set; }//WaitingStartEvents, Active, Inactive, Finished,ErrorOccured
        public List<WorkflowRunner> ActiveRunners { get; set; }
    }
}