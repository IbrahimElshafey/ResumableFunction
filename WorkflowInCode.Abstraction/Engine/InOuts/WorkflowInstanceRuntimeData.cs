﻿namespace WorkflowInCode.Abstraction.Engine.InOuts
{
    public class WorkflowInstanceRuntimeData
    {
        public string InstanceId { get; set; }
        public string Status { get; set; }//WaitingStartEvents, Active, Inactive, Finished,Error Occured

        public List<WorkflowRunner> ActiveRunners { get; set; }
    }

    public class WorkflowRunner
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public WorkflowEvent WaitingEvent { get; set; }
    }
}