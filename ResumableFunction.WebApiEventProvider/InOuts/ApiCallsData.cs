namespace ResumableFunction.WebApiEventProvider.InOuts
{
    public class ApiCallsData
    {
        public bool IsStarted { get; set; }
        public DateTime LastChangeDate { get; set; }

        public HashSet<string> ActionPaths { get; set; } = new HashSet<string>();

        internal void Changed()
        {
            LastChangeDate = DateTime.Now;
        }
    }
}
