**Project Status: Under Analysis**

# Why this project?
I want to write code that reflects the business requirements so that a developer handover another without needing business documents to understand the code.

A project is a set of classes and methods and this didn't tell us about the flow.


I evaluated the existing solutions and found that there is no solution that fits all scenarios
* [MassTransit](https://masstransit-project.com/)
* [Durable Task Framework](https://github.com/Azure/durabletask)
* [D-Async](https://github.com/Dasync/Dasync)
* [Workflow Core](https://github.com/danielgerlag/workflow-core)
* [Infinitic (Kotlin)](https://github.com/infiniticio/infinitic)

All these solutions are intelligent and awesome but do not solve the problem well and easily.




# Events-based workflow?
* External events occurred
* We define a workflow based on the events in code
* We define workflow steps that triggered by one or more events
    ![Workflow based on events!](./img/define_workflow_steps.png)
* The step action will be executed when the event is received, step action is a method like the below:
    ![Workflow based on events!](./img/step_action_example.png)
    Or
    ![Workflow based on events!](./img/step_action_example2.png)
* The image below shows how workflow and events connections (Entity Model)
    ![Workflow based on events!](./img/Workflow_State_Data_Model.png)

# What engine do when a new workflow is registered?
* Find all steps in the workflow and add them to the steps repository.
* Find all steps events and add them to the events repository.
* Subscribe to all events in the workflow.
* The engine will add an inactive instance to the instances repository and link it to the events that start the workflow. 

# What engine do when an event is received?
* When an event is received the engine will search for instances that wait for that type of event.
* The engine will load the workflow related classes from its DLLs if not laoded
* The engine will load the the load instances context data to memory
* The engine will execuet the step actions mathched

# The main parts to define a workflow in code are:
## External Events
* Any pub/sub event like (RabbitMQ, Redis Pub/Sub, Service Bus,...etc)
* The engine will provide an API to inject/add/append events directly, this will enable scenarios like writing a job/service that monitors a legacy system database log and send events directly to the engine. 

## Commands
Is when we ask the system/external service to do something
* Raise event to other parties (RabbitMQ, Service Bus, etc)
* Direct call an API endpoint on other parties
* Save data to a Database table
* Execute any custom logic

## Queries
Ask system/external service to return some data we need to know to execute a workflow.

## Workflow Step
Some code will be executed when one or more events are received, after the workflow engine receive an event it will trigger the right workflow instance.

## Internal Events
The idea here is that you may receive a very generic event such as "request_updated" that contains "RequestId" but no other useful data, so you listen to that event and publish other meaningful events that you use inside the workflow.

# How to define workflow in code based on events?
