﻿namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{

    public class WorkflowInstance<WorkflowContextData>

    {
        public string UserDefinedId { get; private set; }
        public WorkflowInstanceStatus WorkflowInstanceStatus { get; private set; }
        public Guid WorkflowId { get; private set; }
       
        public WorkflowContextData ContextData { get; private set; }
    }
}
