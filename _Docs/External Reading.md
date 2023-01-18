
# Function Patterns
* http://www.Functionpatterns.com/patterns/control/
* http://www.Functionpatterns.com/patterns/
* https://www.ariscommunity.com/users/sstein/2010-07-20-bpmn-2-Function-patterns





# Generate API controller for class
* https://github.com/Testura/Testura.Code
# Generate swagger definition for created API



# Event Types
* API Called [HTTP Listener]
* API Call to the engine [WebHook]
* RabbitMQ or any service bus [Subscribe to event]
* File/Folder Events [Watcher]
* Time Event [Timer Service Event]
	* https://www.hangfire.io/
* Any implementation for IEventProvider interface



Proxy Creation
https://stackoverflow.com/questions/15733900/dynamically-creating-a-proxy-class
https://devblogs.microsoft.com/dotnet/migrating-realproxy-usage-to-dispatchproxy/

# Async 
* Lazy Task
* https://itnext.io/writing-lazy-task-using-new-features-of-c-7-7e9b3f2fda07
* 

# IL Rewrite 
* https://github.com/Fody/Fody
* MonoCeceil

# Converting Expression Trees to Source Code
* https://bagoum.medium.com/c-heresy-converting-expression-trees-to-source-code-1082ba8963a6

# Get all method calls
* https://stackoverflow.com/questions/57118269/get-all-method-calls

# Database to save ContextData
https://www.litedb.org/


# Generic Varince
https://agirlamonggeeks.com/2019/06/04/cannot-implicitly-convert-type-abc-to-iabc-contravariance-vs-covariance-part-2/


# Find a service bus
	*Zebus https://github.com/Abc-Arbitrage/Zebus (no broker)

	* Silverback https://silverback-messaging.net/ A simple but feature-rich message bus "Broker" for .NET core (Apache Kafka, MQTT and RabbitMQ)
	
	* SlimMessageBus https://github.com/zarusz/SlimMessageBus (Apache Kafka, Azure EventHub, MQTT/Mosquitto, Redis Pub/Sub),and provides request-response implementation over message queues.
		* wit Mosquitto http://www.steves-internet-guide.com/install-mosquitto-broker/#manual
		* http://www.steves-internet-guide.com/mosquitto_pub-sub-clients/

	* https://github.com/zarusz/SlimMessageBus
	* Use https://github.com/Cysharp/MessagePipe to send events to the engine
	* https://stackoverflow.com/questions/58549763/how-should-ipc-be-handled-in-net-core

# Resolve by name
https://stackoverflow.com/questions/39072001/dependency-injection-resolving-by-name

# RPC
* CoreRPC https://github.com/kekekeks/CoreRPC
* https://github.com/RandomEngy/PipeMethodCalls