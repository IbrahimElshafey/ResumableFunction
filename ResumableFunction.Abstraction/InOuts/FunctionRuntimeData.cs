namespace ResumableFunction.Abstraction.InOuts
{
    public class FunctionRuntimeData
    {
        public Guid InstanceId { get; set; }
        public string Status { get; set; }//WaitingStartEvents, Active, Inactive, Finished,ErrorOccured

        /// <summary>
        /// Sub functions that is called by current function
        /// </summary>
        public List<FunctionRunner> ActiveRunners { get; set; }
    }
}