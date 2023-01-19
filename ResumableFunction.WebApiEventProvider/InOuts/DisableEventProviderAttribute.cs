namespace ResumableFunction.Abstraction.WebApiEventProvider.InOuts
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DisableEventProviderAttribute : Attribute { }
}
