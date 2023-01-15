namespace WorkflowInCode.Abstraction.InOuts
{
    public class PushedEvent
    {
        /// <summary>
        /// Full type name of EventData as a string
        /// </summary>
        public string EventDataType { get; set; }

        /// <summary>
        /// Will inherit <see cref="IEventData"/> or object that convert to IEventData data using the <see cref="EventDataConverterName"/>
        /// </summary>
        public object EventData { get; set; }

        /// <summary>
        /// Used method to convert Paylaod to the target type (EventDataType)
        /// Will be used if the EventDataType not the same as EventData.GetType().FullName
        /// The engine will load the converter an convert
        /// </summary>
        public string ConvertEventProvider { get; set; }
        public string EventDataConverterName { get; internal set; }
    }



}
