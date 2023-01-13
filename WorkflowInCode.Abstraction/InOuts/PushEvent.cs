namespace WorkflowInCode.Abstraction.InOuts
{
    public class PushedEvent
    {
        /// <summary>
        /// Full type name of EventData as a string
        /// </summary>
        public string EventDataType { get; set; }
        public IEventData EventData { get; set; }

        /// <summary>
        /// Used method to convert Paylaod to the target type
        /// Will be used if the EventDataType not the same as EventData.GetType().FullName
        /// The engine will load the converter an convert
        /// </summary>
        public string ConvertMethod { get; set; }
    }



}
