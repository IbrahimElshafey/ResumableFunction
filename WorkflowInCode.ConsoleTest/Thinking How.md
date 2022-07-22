# What engine do when a new workflow is registered?
* Find all steps in the workflow and add them to the steps repository.
* Find all steps events and add them to the events repository.
* Subscribe to all events in the workflow.
* The engine will add an inactive instance to the instances repository and link it to the events that start the workflow. 

# What engine do when an event is received?
* When an event is received the engine will search for instances that wait for that type of event.
* The engine will load the workflow related classes from its DLLs if not laoded
* The engine will load the the activated instances context data to memory
* The engine will execuet the step actions mathched