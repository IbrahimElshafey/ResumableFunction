using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider;

public class ApiCallEvent : Dictionary<string, object>
{
    public ApiCallEvent()
    {
        Add(nameof(EventProviderName), null);
        Add(nameof(EventIdentifier), null);
    }

    public string EventProviderName
    {
        get => (string)this[nameof(EventProviderName)];
        set => this[nameof(EventProviderName)] = value;
    }

    public string EventIdentifier
    {
        get => (string)this[nameof(EventIdentifier)];
        set => this[nameof(EventIdentifier)] = value;
    }

    public ApiCallEvent AddArgs(IDictionary<string, object> actionArguments)
    {
        foreach (var arg in actionArguments)
        {
            Add(arg.Key, arg.Value);
        }
        return this;
    }
}
