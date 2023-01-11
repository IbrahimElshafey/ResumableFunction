**Project Status: Work in progress**

# Why this project?
* I want to write code that reflects the business requirements so that a developer handover another without needing business documents to understand the code.
* Most workflow engines can't be extended to support complex scenarios, for example, the below link contains a list of workflow patterns, which are elementary to implement by any developer if we just write code and not think about how communications work.
	http://www.workflowpatterns.com/patterns/
* The source code must be a source of truth about how project parts function, and handover a project with hundreds of classes and methods to a new developer does not tell him what business flow executed but a workflow method will simplify understanding of what happens under the hood.
*  With Pub/Sub loosely coupled services it's hard to trace what happened without implementing a complex architecture.


# Simple workflow scenario 
* If we assumed a very simple scenario where someone submits a request and Manager1, Manager2 and Manager3 approve the request sequentially.
* If we implement this as an API without any messaging (message broker) then we will have actions (SumbitRequest, AskManager_X_Approval, Manager_X_SumbitApproval)
* Each of these actions will call the other based on the workflow, And if the workflow changes to another scenario (for example send the request to the three managers in parallel and wait for them all) we must update these actions in different places.
* Using a messaging bus instead of direct calls does not tell us how the workflow goes and didn't solve the sparse update problem.
* Using a commercial workflow engine is expensive and as a rule, any technology that is drag and drop will not solve the problem because we can't control every bit.

# Search for a solution results
I evaluated the existing solutions and found that there is no solution that fits all scenarios,I found that D-Async is the best for what I need but I need a more simple generic solution.
* [D-Async (The best)](https://github.com/Dasync/Dasync)
* [MassTransit](https://masstransit-project.com/)
* [Durable Task Framework](https://github.com/Azure/durabletask)
* [Workflow Core](https://github.com/danielgerlag/workflow-core)
* [Infinitic (Kotlin)](https://github.com/infiniticio/infinitic)

# My Solution 
* I will use IAsyncEnumerable generated state machine to implement a method that can be paused and resumed.
* I will not save the state explicitly 
* I plan to support multiple external event sources such as (HTTP Listner which listens to NIC card and pushes events, Message brokers like RabbitMQ, File changes using file watcher, WebHooks to the engine, Time-based events, and more..)

# Example
Keep in mind that the work is in progress
```C#
        //ProjectApprovalContextData is the data that will bes saved to the database 
        //When the engine match an event it will load the related workflow class and set the 
        //InstanceData property by loading it from database
        //No other state saved just the InstanceData and workflow author must keep that in mind
        //We can't depend on automatic serialize for state becuse compiler may remove fields and variables we defined
        protected override async IAsyncEnumerable<WorkflowEvent> RunWorkflow()
        {
            //any class that inherit WorkflowInstance<T> has the methods
            //WaitEvent,WaitFirstEvent in a collection,WaitEvents and SaveInstanceData

            //the engine will wait for ProjectRequested event
            //no match function because it's the first one
            //context prop is prop in InstanceData that we will set with event result data
            yield return WaitEvent(
                    eventToWait: ProjectRequested,
                    matchFunction: null,
                    contextProp: () => InstanceData.Project);
            //the compiler will save state after executing the previous return
            //and wiating for the event
            //it will continue from the line below when event cames


            //InstanceData.Project is set by the previous event
            //we will initiate a task for Owner and wait to the Owner response
            //That matching function correlates the event to the right instance
            //The matching function will be translated to query language "MongoDB query for example" by the engine to search the active instance.
            await AskOwnerToApprove(InstanceData.Project);
            yield return WaitEvent(
                OwnerApproval,
                result => result.ProjectId == InstanceData.Project.Id,
                () => InstanceData.OwnerApprovalResult);
            if (InstanceData.OwnerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project,"Owner");
                yield break;
            }

            await AskSponsorToApprove(InstanceData.Project);
            yield return WaitEvent(
             SponsorApproval,
             result => result.ProjectId == InstanceData.Project.Id,
             () => InstanceData.SponsorApprovalResult);
            if (InstanceData.SponsorApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Sponsor");
                yield break;
            }

            await AskManagerToApprove(InstanceData.Project);
            yield return WaitEvent(
             ManagerApproval,
             result => result.ProjectId == InstanceData.Project.Id,
             () => InstanceData.ManagerApprovalResult);
            if (InstanceData.ManagerApprovalResult.Rejected)
            {
                await ProjectRejected(InstanceData.Project, "Manager");
                yield break;
            }

            Console.WriteLine("All three aproved");
        }
```
