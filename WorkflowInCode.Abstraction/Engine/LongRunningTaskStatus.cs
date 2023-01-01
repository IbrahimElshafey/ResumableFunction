namespace WorkflowInCode.Abstraction.Engine
{
    [Flags]
    public enum LongRunningTaskStatus
    {
        None = 1,
        Initiated = 1,
        Completed = 2,
        ErrorWhileInitiation = -1,
        ErrorWhileCompletion = -2,
    }
}