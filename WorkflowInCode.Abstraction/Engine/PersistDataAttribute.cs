namespace WorkflowInCode.Abstraction.Engine
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    sealed class PersistDataAttribute : Attribute
    {
    }
}
