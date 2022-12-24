namespace WorkflowInCode.Abstraction.Engine
{
    public interface IEvent<T>
    {
        T EventData { get; }
        void Fire();
    }
}