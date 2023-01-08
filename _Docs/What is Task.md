# What is a task?
The task is a long-running process that starts when one or more events
are raised, it waits for the recipient to take an action, and the
recipient may be a human or a system. when the recipient takes an
action an event is raised.

# What are the task's main components?
* Start event/s
* Recipient/s
* One Outcome Event of many defined events
* Task event/s collector

# Task event/s collector
Is a component that is responsible to collect the events and determine if the task should be started or not?