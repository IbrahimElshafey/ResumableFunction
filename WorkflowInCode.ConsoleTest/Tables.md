# Workflow Table
* Workflow Name
* Start Event Name
* Active Instance Count
* Fault Instance Count
* Workflow assembly file
* Active (Yes/No)
* Version

# Workflow Instance
* Parent Workflow Id
* Instance Id (Name+GUID)
* Waiting for event (Next event)
* Status (Running,Ended)
* Version

# Workflow Instance Histroy
* Instance Id
* Type (Event Received/Command Execution Requested)
* Name (Event Name/Command Name)
* Data (Event Data/Command Parameters and step name if exist)
* Context Data

# Workflow events
* Parent Workflow Id
* Event Name
* Type (WebHook, RabbitMQ, Service Bus, etc)
* Class and assembly path