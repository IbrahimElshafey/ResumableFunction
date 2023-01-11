

# Workflow Instance Histroy Record
* Id
* Workflow Instance Id (FK)
* Type (Event/Command/Query)
* Name (Event Name/Command Name/Query Name)
* Data (Event Data/Command Parameters/Query Parameters)
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




