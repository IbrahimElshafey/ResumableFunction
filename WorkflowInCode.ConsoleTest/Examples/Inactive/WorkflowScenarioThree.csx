using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples.Inactive
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
             * بعد انتهاء التتابع الأول والثاني يتم الإنهاء 
             */
            workflow.RegisterStep(
                  new BasicEvent<dynamic>("Start"),
                  AfterStart);

            workflow.RegisterStep(
                new OrderedSequenceEvents()
                    .AddEventTrigger(
                         new BasicEvent<dynamic>("A"),
                        eventData => ContextData.Id == eventData.Id)
                    .AddEventTrigger(
                        new BasicEvent<dynamic>("B"),
                        eventData => ContextData.Id == eventData.Id)
                    .AddEventTrigger(
                       new BasicEvent<dynamic>("C"),
                        eventData => ContextData.Id == eventData.Id),
                CollectEventsAbc);

            workflow.RegisterStep(
               new OrderedSequenceEvents()
                   .AddEventTrigger(
                        new BasicEvent<dynamic>("X"),
                       eventData => ContextData.Id == eventData.Id)
                   .AddEventTrigger(
                       new BasicEvent<dynamic>("Y"),
                       eventData => ContextData.Id == eventData.Id)
                   .AddEventTrigger(
                      new BasicEvent<dynamic>("Z"),
                       eventData => ContextData.Id == eventData.Id),
               CollectEventsXyz);

            workflow.RegisterStep(
            new AnyOneOfEvents()
               .AddEventTrigger(
                    new BasicEvent<dynamic>("XyzPathFinished"),
                   eventData => ContextData.Id == eventData.Id)
               .AddEventTrigger(
                   new BasicEvent<dynamic>("AbcPathFinished"),
                   eventData => ContextData.Id == eventData.Id),
           CollectTwoParallelPaths);
        }



        private async Task CollectTwoParallelPaths(object arg)
        {
            ContextData.TwoParallelPaths += 1;
            await SaveState();
            if (ContextData.TwoParallelPaths == 2)
                await Workflow.End();
        }



        private async Task CollectEventsXyz(object arg)
        {
            ContextData.XyzCounter += 1;
            await SaveState();
            if (ContextData.XyzCounter == 3)
            {
                await Workflow.PushInternalEvent(new InternalEvent<object>("XyzPathFinished", new { WorkflowId }));
            }
        }



        private async Task CollectEventsAbc(object eventData)
        {
            ContextData.AbcCounter += 1;
            await SaveState();
            if (ContextData.AbcCounter == 3)
            {
                await Workflow.PushInternalEvent(new InternalEvent<object>("AbcPathFinished", new { WorkflowId }));
            }
        }

        private async Task AfterStart(dynamic startEvent)
        {
            ContextData.Id += startEvent.Id;
            await SaveState();
            await Workflow.ExpectNextStep<dynamic>(CollectEventsAbc);
            await Workflow.ExpectNextStep<dynamic>(CollectEventsXyz);
        }
    }

    internal class WorkflowScenarioThree_ContextData
    {
        public int AbcCounter { get; internal set; }
        public dynamic Id { get; internal set; }
        public int XyzCounter { get; internal set; }
        public int TwoParallelPaths { get; internal set; }
    }
}
