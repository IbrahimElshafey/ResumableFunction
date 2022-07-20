using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    //go to سيناريوهات.md for more details
    internal class WorkflowScenarioThree : WorkflowInstance<WorkflowScenarioThree_ContextData>
    {
        public WorkflowScenarioThree(IWorkflow workflow) : base(workflow)
        {
            /*
             * البداية
             * بعد البداية يمكن تنفيذ العمليات أ,ب,ج بالتتابع
             * بعد البداية يمكن تنفيذ العمليات س,ص,ع بالتتابع
             * بعد انتهاء التتابع الأول والثاني يتم تنفيذ العملية ع 
             */
            workflow.RegisterStep(
              new BasicEvent<dynamic>("Start"),
              AfterStart);

            workflow.RegisterStep(
            new StepTriggers()
                .AddEventTrigger(
                     new BasicEvent<dynamic>("A"),
                    eventData => ContextData.Id == eventData.Id)
                .AddEventTrigger(
                    new BasicEvent<dynamic>("B"),
                    eventData => ContextData.Id == eventData.Id)
                .AddEventTrigger(
                   new BasicEvent<dynamic>("C"),
                    eventData => ContextData.Id == eventData.Id),
            CollectEventsAbc,
            AfterAbcCollection);
        }

        private Task AfterAbcCollection(dynamic arg)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> CollectEventsAbc(object arg)
        {
            this.ContextData.AbcCounter += 1;
            await SaveState();
            return ContextData.AbcCounter == 3;
        }

        private Task AfterStart(dynamic start)
        {
            throw new NotImplementedException();
        }
    }

    internal class WorkflowScenarioThree_ContextData
    {
        public int AbcCounter { get; internal set; }
        public dynamic Id { get; internal set; }
    }
}
