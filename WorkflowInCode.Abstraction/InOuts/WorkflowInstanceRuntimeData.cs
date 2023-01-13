namespace WorkflowInCode.Abstraction.InOuts
{
    public class WorkflowInstanceRuntimeData
    {
        public Guid InstanceId { get; set; }
        public Type InstanceDataType { get; set; }
        public string Status { get; set; }//WaitingStartEvents, Active, Inactive, Finished,ErrorOccured
        public List<WorkflowRunner> ActiveRunners { get; set; }
    }

    public class WorkflowRunner
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public SingleEventWaiting WaitingEvent { get; set; }
    }
}