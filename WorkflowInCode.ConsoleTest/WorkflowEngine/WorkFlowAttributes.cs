using WorkflowInCode.ConsoleTest.WorkflowEngine;

internal class WorklowTag : Attribute
{
}

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal class WorkFlowStep : Attribute
{
   
}
