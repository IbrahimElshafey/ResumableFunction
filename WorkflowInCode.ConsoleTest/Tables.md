# Workflow Defintition
* Id
* Workflow Name
* (Many) Workflow Steps
* Workflow class full path
* Global Events Filter
* Active (Yes/No)
* Version

# Workflow Instance
* Id
* User Defined Id //some uinque ID the user define an use at such as "Project_123"
* Parent Workflow Defintition Id (FK)
* Context Data (we can define another table to save data chnages history)
* Status (WaitingStartEvents, Active, Inactive, Finished)
* (Many) Expected Steps 

# Workflow Instance Histroy Record
* Id
* Workflow Instance Id (FK)
* Type (Event/Command/Query)
* Name (Event Name/Command Name/Query Name)
* Data (Event Data/Command Parameters/Query Parameters)
* Timestamp

# Event Repository
* Id
* (Many)Workflow Steps
* Unique Event Name
* Event class full path

# Workflow Step
* Id
* Workflow Defintition Id (FK)
* (Many) Events that fire this step
* Step Name
* Step Filter Method
* Step Action Method

# Expected Events Table
* Id
* Event Id (FK)
* Event Name (FK)
* Workflow Instance Id (FK)
* Step Id (FK)

# What engine do when an event is received?
* When an event is received the engine will search for instances that wait for that type of event (Search in expected events table).
* After search you will get list where each item contains(workflow instance,workfow step)
* for each item in the list we will do
	* The engine will apply the workfow step filter method to the event data and workflow instance context data
	* Check if instance should be activated
	* For activated instances the workflow engine will execuete the step action method that linked to the received event




