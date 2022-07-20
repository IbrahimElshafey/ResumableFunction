# Why this project?
I want to write workflow in code.
I evaluated the existing solution and found that there is no solution that fits all scenario
* MassTransit
* Durable Task Framework
* D-Async
all are intelligent but do not solve the problem well

# How?
* External events occurred
* We define a workflow based on the events in code
* The workflow engine subscribes to events and when an event is received it searches for the workflows that are related to that event
* The engine will get the active running instances for each matched workflow, and for each instance, the engine will check if the instance waits for the event received.
* If the workflow instance already waits for the future event the workflow engine will run/resume the workflow step related.
* If the event is a start event then the engine will start a new instance of the matched workflows.
* If the event didn’t match with any instance then the engine will add it to the errors queue

# What is external events?


# The main parts to define workflow in code are:
## External Events
* Any pub/sub event like (RabbitMQ, Redis Pub/Sub, Service Bus,...etc)
* The engine will provide an API to inject/add/append event directly, this will enable me (as an example) to write a windows service that monitors legacy system database log and send events directly to the engine. 

## Commands

## Quries

## Event Translator

## Internal Events

## Workflow Step

# How to define workflow in code based on events?
