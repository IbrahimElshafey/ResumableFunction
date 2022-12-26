namespace WorkflowInCode.Abstraction.Engine
{
    public interface ISubscribeEvent<T>
    {
        T EventData { get; }
        void Subscribe();
    }
}