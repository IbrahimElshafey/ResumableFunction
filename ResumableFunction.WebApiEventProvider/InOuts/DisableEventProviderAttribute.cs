namespace ResumableFunction.Abstraction.WebApiEventProvider.InOuts
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class EnableEventProviderAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DisableEventProviderAttribute : Attribute { }
}
