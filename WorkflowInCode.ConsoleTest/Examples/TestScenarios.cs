using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class TestScenarios : WorkflowDefinition<dynamic>
    {
        public TestScenarios(IWorkflowEngine workflow, string name = null) : base(workflow, name)
        {
        }

        public override void RegisterSteps()
        {
            //WhenAllEventsMatched();
            //OrderedEvents();
            //OrderedEventsWithBack();
            //SplitAndMerge();
            //SplitAndCollect();
        }

        private void SplitAndCollect()
        {
            //event1 then (event2 or event3) then end
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) =>
                {
                    await Workflow.AddEventExpectation("Event2");
                    await Workflow.AddEventExpectation("Event3");
                });
            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event2"),
               async (eventData) =>
               {
                   await Workflow.PushInternalEvent("CollectEvents2or3");
               });

            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event3"),
               async (eventData) =>
               {
                   await Workflow.PushInternalEvent("CollectEvents2or3");
               });

            Workflow.RegisterStep(
               "Collect Events 2 or 3",
               new BasicEvent<dynamic>("CollectEvents2or3"),
               async (eventData) =>
               {
                   await new BasicCommand("SomeThing", "Data").Execute();
                   await Workflow.End();
               });
        }

        private void SplitAndMerge()
        {
            //event1 then (event2 and event3) then end
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) =>
                {
                    await Workflow.AddEventExpectation("Event2");
                    await Workflow.AddEventExpectation("Event3");
                });
            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event2"),
               async (eventData) =>
               {
                   await Workflow.PushInternalEvent("CollectEvents2and3");
               });

            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event3"),
               async (eventData) =>
               {
                   await Workflow.PushInternalEvent("CollectEvents2and3");
               });

            Workflow.RegisterStep(
               "Collect Events 2 and 3",
               new BasicEvent<dynamic>("CollectEvents2and3"),
               async (eventData) =>
               {
                   CurrentInstance.ContextData.CollectorCounter += 1;
                   if (CurrentInstance.ContextData.CollectorCounter == 2)
                   {
                       await new BasicCommand("SomeThing","Data").Execute();
                       await Workflow.End();
                   }
               });
        }

        private void OrderedEventsWithBack()
        {
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) => await Workflow.AddEventExpectation("Event2"));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => 
                {
                    await Workflow.AddEventExpectation("Event3");
                });
            
            Workflow.RegisterStep(
                "Event3 Received",
                new BasicEvent<dynamic>("Event3"),
                async (eventData) =>
                {
                    if(eventData.Rejected)
                        await Workflow.AddEventExpectation("Event1");
                    if(eventData.Accepted)
                        await Workflow.End();
                });
        }

        private void OrderedEvents()
        {
            //Task.WhenAll(event1,event2,event3);
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) => await Workflow.AddEventExpectation("Event2"));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => await Workflow.AddEventExpectation("Event3"));
            
            Workflow.RegisterStep(
                "Event3 Received",
                new BasicEvent<dynamic>("Event3"),
                async (eventData) => await Workflow.End());
            //Workflow.RegisterStep(
            //   "Collect Events",
            //   new BasicEvent<dynamic>("CollectEvents"),
            //   async (eventData) =>
            //   {
            //       CurrentInstance.ContextData.Counter += 1;
            //       if (CurrentInstance.ContextData.Counter == 3)
            //           await new BasicCommand("AfterAllEnded", "").Execute();
            //   });
        }

        private void WhenAllEventsMatched()
        {
            //Task.WhenAll(event1,event2,event3);
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) => await Workflow.PushInternalEvent("CollectEvents", "Event1"));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => await Workflow.PushInternalEvent("CollectEvents", "Event2"));
            
            Workflow.RegisterStep(
                "Event3 Received",
                new BasicEvent<dynamic>("Event3"),
                async (eventData) => await Workflow.PushInternalEvent("CollectEvents", "Event3"));
            
            Workflow.RegisterStep(
               "Collect Events",
               new InternalEvent<dynamic>("CollectEvents"),
               async (eventData) =>
               {
                   CurrentInstance.ContextData.Counter += 1;
                   if (CurrentInstance.ContextData.Counter == 3)
                       await new BasicCommand("AfterAllEnded", "").Execute();
               });
        }
    }
}
