using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider;
using System.Reflection;

public class ApiCallEvent : Dictionary<string, object>
{
    private const string ResultKey = "Result";

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

    public ApiCallEvent AddArgs(IDictionary<string, object> actionArguments, bool flatObject)
    {
        foreach (var arg in actionArguments)
        {
            if (arg.Value == null) continue;
            if (flatObject)
                if (IsSimple(arg.Value))
                    Add(arg.Key, arg.Value);
                else
                    AddComplex(arg.Key, arg.Value);
            else
                Add(arg.Key, arg.Value);
        }
        return this;
    }

    internal void AddResult(object? value, bool flatObject)
    {
        if (value == null) return;
        Add(ResultKey, value);
        if (flatObject)
            if (IsSimple(value))
                Add("Result", value);
            else
                AddComplex("Result", value);
        else
            Add("Result", value);
    }

    private void AddComplex(string key, object value)
    {
        if (value == null) return;
        Type myType = value.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

        foreach (PropertyInfo prop in props)
        {
            object propValue = prop.GetValue(value, null);
            if (propValue == null) continue;
            if (IsSimple(propValue))
                Add($"{key}_{prop.Name}", propValue);
            else
                AddComplex($"{key}_{prop.Name}", propValue);
        }
    }

    bool IsSimple(object value)
    {
        return value.GetType().IsPrimitive
          || value.GetType().Equals(typeof(string));
    }
}
