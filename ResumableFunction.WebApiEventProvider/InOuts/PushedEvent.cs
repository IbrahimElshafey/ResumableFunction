using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;

public class PushedEvent
{

    public string EventProvider { get; set; }
    /// <summary>
    /// Full type name of EventData as a string
    /// </summary>
    public string DataType => "ResumableFunction.Abstraction.InOuts.ApiInOutResult";

    /// <summary>
    /// Will inherit <see cref="InOuts.IEventData"/> or object that convert to IEventData data using the <see cref="DataConverterName"/>
    /// </summary>
    public dynamic Data { get; set; }

    /// <summary>
    /// Used method to convert Paylaod to the target type (EventDataType)
    /// Will be used if the EventDataType not the same as EventData.GetType().FullName
    /// The engine will load the converter an convert
    /// </summary>
    public string DataConverterName { get; set; } = "JsonToObject";
}
