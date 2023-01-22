**Project Status: Work in progress**
# What is Resumable Function?
A function/method/procedure/routine that paused when match an event waiting statement and resumed to the same place when event fired and matched.

# Key parts
* Engine: component responsible for running and resume function execution.
* Event Provider: is a component that push events to the engine.
* Queung service: is a way to separate engine and providers.
* Event: Plain object but contains a property for it's provider.

# Event Waiting,we can wait
* Single Event (similar to `await` in `async\await`)
* First event in a group (similar to `Task.WhenAny()`)
* A group of event (similar to `Task.WhenAll()`)
* Call another resumable function (call another async method)
* Wait one or more resumable function to complete.
* Wait first resumable function to complete.

# what are the expected types and resources for events?
* Any implementation for `IEventProvider` interface that push events to the engine such as:
* A WEB proxy listen to server in outs HTTP calls.
* File watcher.
* Long pooling service that monitor a database table.
* Timer service.

# Why this project?
* I want to write code that reflects the business requirements so that a developer handover another without needing business documents to understand the code.
* Most Function engines can't be extended to support complex scenarios, for example, the below link contains a list of Function patterns, which are elementary to implement by any developer if we just write code and not think about how communications work.
	http://www.Functionpatterns.com/patterns/
* The source code must be a source of truth about how project parts function, and handover a project with hundreds of classes and methods to a new developer does not tell him what business flow executed but a Function method will simplify understanding of what happens under the hood.
*  With Pub/Sub loosely coupled services it's hard to trace what happened without implementing a complex architecture.


# Simple resumable function scenario 
* If we assumed a very simple scenario where someone submits a request and Manager1, Manager2 and Manager3 approve the request sequentially.
* If we implement this as an API without any messaging (message broker) then we will have actions (SumbitRequest, AskManager_X_Approval, Manager_X_SumbitApproval)
* Each of these actions will call the other based on the Function, And if the Function changes to another scenario (for example send the request to the three managers in parallel and wait for them all) we must update these actions in different places.
* Using a messaging bus instead of direct calls does not tell us how the Function goes and didn't solve the sparse update problem.
* Using a commercial Function engine is expensive and as a rule, any technology that is drag and drop will not solve the problem because we can't control every bit.

# Search for a solution results
I evaluated the existing solutions and found that there is no solution that fits all scenarios,I found that D-Async is the best for what I need but I need a more simple generic solution.
* [D-Async (The best)](https://github.com/Dasync/Dasync)
* [MassTransit](https://masstransit-project.com/)
* [Durable Task Framework](https://github.com/Azure/durabletask)
* [Function Core](https://github.com/danielgerlag/Function-core)
* [Infinitic (Kotlin)](https://github.com/infiniticio/infinitic)

# My Solution 
* I will use IAsyncEnumerable generated state machine to implement a method that can be paused and resumed.
* I will not save the state implicitly ,Because we can't depend on automatic serialization for state because compiler may remove/rename fields and variables we defined.


# Example
Keep in mind that the work is in progress
```C#
//ProjectApprovalFunctionData is the data that will bes saved to the database 

//When the engine match an event it will load the related Function class
// and set the FunctionData property by loading it from database.

//No other state saved just the FunctionData and Function author must keep that in mind.

//We can't depend on automatic serialize for state becuse compiler may remove fields and variables we defined.
public class ProjectApproval : ResumableFunction<ProjectApprovalFunctionData>
{
	//any inherited ResumableFunction must implement 'RunFunction'
	protected override async IAsyncEnumerable<EventWaitingResult> RunFunction()
	{
		//any class that inherit FunctionInstance<T> has the methods
		//WaitEvent,WaitFirstEvent in a collection,WaitEvents and SaveFunctionData

		//the engine will wait for ProjectRequested event
		//no match function because it's the first one
		//context prop is prop in FunctionData that we will set with event result data
		yield return WaitEvent(typeof(ProjectRequestedEvent),"ProjectCreated").SetProp(() => FunctionData.Project);
		//the compiler will save state after executing the previous return
		//and wiating for the event
		//it will continue from the line below when event cames


		//FunctionData.Project is set by the previous event
		//we will initiate a task for Owner and wait to the Owner response
		//That matching function correlates the event to the right instance
		//The matching function will be translated to query language "MongoDB query for example" by the engine to search the active instance.
		await AskOwnerToApprove(FunctionData.Project);
		yield return WaitEvent(typeof(ManagerApprovalEvent), "OwnerApproval")
			.Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
			.SetProp(() => FunctionData.OwnerApprovalResult);
		if (FunctionData.OwnerApprovalResult.Rejected)
		{
			await ProjectRejected(FunctionData.Project, "Owner");
			yield break;
		}

		await AskSponsorToApprove(FunctionData.Project);
		yield return WaitEvent(typeof(ManagerApprovalEvent), "SponsorApproval")
			.Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
			.SetProp(() => FunctionData.SponsorApprovalResult);
		if (FunctionData.SponsorApprovalResult.Rejected)
		{
			await ProjectRejected(FunctionData.Project, "Sponsor");
			yield break;
		}

		await AskManagerToApprove(FunctionData.Project);
		yield return WaitEvent(typeof(ManagerApprovalEvent), "ManagerApproval")
			.Match<ManagerApprovalEvent>(result => result.ProjectId == FunctionData.Project.Id)
			.SetProp(() => FunctionData.ManagerApprovalResult);
		if (FunctionData.ManagerApprovalResult.Rejected)
		{
			await ProjectRejected(FunctionData.Project, "Manager");
			yield break;
		}

		Console.WriteLine("All three approved");
	}
}
```
