# ToDo
* Validate wait types
* Write Implementation for:
	* Database Presist
		* Pass DbContext to Repositories
		* Repositories
		* Use unit of work pattern for one transaction save
* Use hangfire to 
	* Queue pushed events requests [Fire and forget]
	* Implement provider for time based events
* Test Engine Scenarios
	* Seqeunce
	* Wait all
	* Wait any
	* Wait function
	* Wait many functions
	* Wait any function
	* ReplayWait Go back [to] and go back [after] for types:
		* One Event Node
		* All event Wait
		* Any Event Wait
		* function
		* many functions
		* any function 


* Self queuing for 
	* Web API Provider failed requests
	* Engine calls to providers
	* Table for falied requests and service for play them


* Automatic Generator for
	* Event Classes
	* Flurl Interfaces


* Parameter check lib use


* Add Log
* Add Resumable Function History
* Add event provider that monitor network requests
* Add UI project to monitor and control functions



* Maintain list of start events


=======
# Done
* Match Function Expression options:
	* Translate use props from FunctionData to constants
	* Transalte expression to query and search the related FunctionData table/collection
	* Load instance data with each event and apply on it