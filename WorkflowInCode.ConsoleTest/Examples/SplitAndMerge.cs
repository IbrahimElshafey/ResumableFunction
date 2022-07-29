using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples.Inactive
{
    //go to سيناريوهات.md for more details
    internal class SplitAndMerge : WorkflowDefinition<WorkflowScenarioThree_ContextData>
    {
        public SplitAndMerge(IWorkflowEngine workflow) : base(workflow)
        {

        }



        private async Task CollectTwoParallelPaths(object arg)
        {
            CurrentInstance.ContextData.TwoParallelPaths += 1;
            await Workflow.SaveState();
            if (CurrentInstance.ContextData.TwoParallelPaths == 2)
                await Workflow.End();
        }



        private async Task CollectEventsXyz(object arg)
        {
            CurrentInstance.ContextData.XyzCounter += 1;
            await Workflow.SaveState();
            if (CurrentInstance.ContextData.XyzCounter == 3)
            {
                await Workflow.PushInternalEvent(new InternalEvent<object>("XyzPathFinished"));
            }
        }



        private async Task CollectEventsAbc(object eventData)
        {
            CurrentInstance.ContextData.AbcCounter += 1;
            await Workflow.SaveState();
            if (CurrentInstance.ContextData.AbcCounter == 3)
            {
                await Workflow.PushInternalEvent(new InternalEvent<object>("AbcPathFinished"));
            }
        }

        private async Task AfterStart(dynamic startEvent)
        {
            CurrentInstance.ContextData.Id += startEvent.Id;
            await Workflow.SaveState();
            await Workflow.ExpectNextStep("ABC_Branch");
            await Workflow.ExpectNextStep("XYZ_Branch");
        }

        public override void RegisterSteps()
        {
            /*
              * البداية
              * بعد البداية يمكن تنفيذ العمليات أ,ب,ج بالتتابع
              * بعد البداية يمكن تنفيذ العمليات س,ص,ع بالتتابع
              * بعد انتهاء التتابع الأول والثاني يتم الإنهاء 
              */
            Workflow.RegisterStep(
                    "Start",
                  new BasicEvent<dynamic>("Start"),
                  AfterStart);

            Workflow.RegisterStep(
                "ABC_Branch",
                new OrderedSequenceEvents()
                    .AddEventTrigger(
                         new BasicEvent<dynamic>("A"),
                        eventData => CurrentInstance.ContextData.Id == eventData.Id)
                    .AddEventTrigger(
                        new BasicEvent<dynamic>("B"),
                        eventData => CurrentInstance.ContextData.Id == eventData.Id)
                    .AddEventTrigger(
                       new BasicEvent<dynamic>("C"),
                        eventData => CurrentInstance.ContextData.Id == eventData.Id),
                CollectEventsAbc);

            Workflow.RegisterStep(
                "XYZ_Branch",
               new OrderedSequenceEvents()
                   .AddEventTrigger(
                        new BasicEvent<dynamic>("X"),
                       eventData => CurrentInstance.ContextData.Id == eventData.Id)
                   .AddEventTrigger(
                       new BasicEvent<dynamic>("Y"),
                       eventData => CurrentInstance.ContextData.Id == eventData.Id)
                   .AddEventTrigger(
                      new BasicEvent<dynamic>("Z"),
                       eventData => CurrentInstance.ContextData.Id == eventData.Id),
               CollectEventsXyz);

            Workflow.RegisterStep(
            "Merge_ABC_XYZ",
            new AllOfEvents()
               .AddEventTrigger(
                    new InternalEvent<dynamic>("XyzPathFinished"),
                   eventData => CurrentInstance.ContextData.Id == eventData.Id)
               .AddEventTrigger(
                   new InternalEvent<dynamic>("AbcPathFinished"),
                   eventData => CurrentInstance.ContextData.Id == eventData.Id),
           CollectTwoParallelPaths);
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
