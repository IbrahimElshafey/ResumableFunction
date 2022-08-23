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
            SplitAndCollectOne();
        }

        private void SplitAndCollectOne()
        {
            //event1 then (event2 or event3) then end
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) =>
                {
                    await Workflow.ExpectNextStep("Event2 Received");
                    await Workflow.ExpectNextStep("Event3 Received");
                });
            Workflow.RegisterStep(
               "Event2 Received",
               new BasicEvent<dynamic>("Event2"),
               async (eventData) =>
               {
                   await CollectEvents2or3(eventData);
               });

            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event3"),
               async (eventData) =>
               {
                   //await Workflow.PushInternalEvent("CollectEvents2or3");
                   await CollectEvents2or3(eventData);
               });

         
            async Task CollectEvents2or3(object eventData)
            {
                await new BasicCommand("After one event of Event2 and Event3 Matched", "Data").Execute();
                await Workflow.RemoveExpectations();
                await Workflow.End();//will throw exception if expectation list is not empty
            }
        }

        private void SplitAndMerge()
        {
            //event1 then (event2 and event3) then end
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) =>
                {
                    await Workflow.ExpectNextStep("Event2 Received");
                    await Workflow.ExpectNextStep("Event3 Received");
                });
            Workflow.RegisterStep(
               "Event2 Received",
               new BasicEvent<dynamic>("Event2"),
               async (eventData) =>
               {
                   await CollectEvents2and3(eventData);
               });

            Workflow.RegisterStep(
               "Event3 Received",
               new BasicEvent<dynamic>("Event3"),
               async (eventData) =>
               {
                   await CollectEvents2and3(eventData);
               });
            async Task CollectEvents2and3(object eventData)
            {
                CurrentInstance.ContextData.CollectorCounter += 1;
                if (CurrentInstance.ContextData.CollectorCounter == 2)
                {
                    await new BasicCommand("SomeThing", "Data").Execute();
                    await Workflow.End();
                }
            }
        }

        private void OrderedEventsWithBack()
        {
            Workflow.RegisterStep(
                "Event1 Received",
                new BasicEvent<dynamic>("Event1"),
                async (eventData) => await Workflow.ExpectNextStep("Event2 Received"));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => 
                {
                    await Workflow.ExpectNextStep("Event3 Received");
                });
            
            Workflow.RegisterStep(
                "Event3 Received",
                new BasicEvent<dynamic>("Event3"),
                async (eventData) =>
                {
                    if(eventData.Rejected)
                        await Workflow.ExpectNextStep("Event1 Received");
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
                async (eventData) => await Workflow.ExpectNextStep("Event2 Received"));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => await Workflow.ExpectNextStep("Event3 Received"));
            
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
                async (eventData) => await CollectEvents(eventData));
            
            Workflow.RegisterStep(
                "Event2 Received",
                new BasicEvent<dynamic>("Event2"),
                async (eventData) => await CollectEvents(eventData));
            
            Workflow.RegisterStep(
                "Event3 Received",
                new BasicEvent<dynamic>("Event3"),
                async (eventData) => await CollectEvents(eventData));
            
            async Task CollectEvents(object eventData)
            {
                CurrentInstance.ContextData.Counter += 1;
                if (CurrentInstance.ContextData.Counter == 3)
                    await new BasicCommand("AfterAllEnded", "").Execute();
            }
        }
    }
}
