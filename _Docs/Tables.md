# Event Provider
* 
# Workflow Instance Histroy Record
* Name (Event Name)
* Data (Event Data)
* Timestamp

# Event Repository
* Unique Event Name
* Event class full path


# What engine do when an event is received?
* Event provider is reponsible to push events to the workflow engine
* When an event is received the engine will search for instances that wait for that type of event (Search in expected events for active instances).
* After search you will get list of workflow instances
* for each workflow instance
	* The engine will load the workflow code and execuste it with it's InstanceData




