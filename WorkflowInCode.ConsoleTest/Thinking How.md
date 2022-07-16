# Workflow engine responsiabilties
* Parse the workflow method and find that each branch has an end
* Find events in the function
* Subscribe to the events
* When event received the engine will run it aggainst related filter methods
* The engine will activate the instance
* Resume the execution
* Save the context before waiting for the next event
* Reload the context when event recevied and instance activated

https://docs.google.com/document/d/1TDzOILQo_QeXSzA9PI-HsqqBZ6SJu0EectyV89NJi_k/edit