namespace ResumableFunction.Abstraction.InOuts
{
    /*
        {
          "provider": "EventproviderName",
          "dataType": "ResumableFunction.Abstraction.Samples.ManagerApprovalEvent",
          "data": {"ProjectId":"125", "Accepted":"true", "Rejected":"false"},
          "dataConverterName": ""
        }
     */
    public class PushedEvent
    {

        public string EventProvider { get; set; }
        /// <summary>
        /// Full type name of EventData as a string
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Will inherit <see cref="InOuts.IEventData"/> or object that convert to IEventData data using the <see cref="DataConverterName"/>
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Used method to convert Paylaod to the target type (EventDataType)
        /// Will be used if the EventDataType not the same as EventData.GetType().FullName
        /// The engine will load the converter an convert
        /// </summary>
        public string DataConverterName { get; set; }
    }



}
