namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    /// <summary>
    /// External event that occured outside such as 
    ///     RabbitMQ message recieved
    ///     Long pooling to get ne data from database
    ///     WebHook defined in the engine
    /// When engine recived an event it add it to it's internal events queue
    /// </summary>
    /// <typeparam name="EventData"></typeparam>
    public interface IExternalEvent<EventData> : IEvent<EventData>
    {
        /// <summary>
        /// The engine will execute this function when a workflow class activated
        /// The engine will parse the work flow to find all external events and subscribe to it
        /// </summary>
        /// <returns>Nothing or Error</returns>
        Task Subscribe();

        /// <summary>
        /// The engine will calls this method when a workflow deactivated 
        /// The workflow deactivated when engine admin mark it as inactive
        /// Or when new workflow version submitted and no instance is related to the old ones
        /// </summary>
        /// <returns>Nothing or Error</returns>
        Task Unsubscribe();

        /// <summary>
        /// The engine will call this method to determine if the event is defined before in old workflows
        /// ,This method must return uniqe string for the event.
        /// 
        /// For Example:the name must be the same if two diffrent work flow subscribe to the same RabbitMq topic
        /// in this case the method must return something like "RabbitMQ-TopicName" for the two events
        /// </summary>
        /// <returns></returns>
        string UniqueIdentifier { get; }
        int DataBaseIdentifier { get; }
    }
}
