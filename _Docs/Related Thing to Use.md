* Background tasks with hosted services in ASP.NET Core
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
# Dynamic class loading (we will not need this)
* [Using reflection] (https://www.codeproject.com/Articles/13747/Dynamically-load-a-class-and-execute-a-method-in-N)
* [Using dynamic runtime] ()
* Using Expresssion Trees
	* https://agileobjects.co.uk/readable-expression-trees-debug-visualizer

	
# Find a service bus
	*Zebus https://github.com/Abc-Arbitrage/Zebus (no broker)

	* Silverback https://silverback-messaging.net/ A simple but feature-rich message bus "Broker" for .NET core (Apache Kafka, MQTT and RabbitMQ)
	
	* [*]SlimMessageBus https://github.com/zarusz/SlimMessageBus (Apache Kafka, Azure EventHub, MQTT/Mosquitto, Redis Pub/Sub),and provides request-response implementation over message queues.
		* wit Mosquitto http://www.steves-internet-guide.com/install-mosquitto-broker/#manual
		* http://www.steves-internet-guide.com/mosquitto_pub-sub-clients/

	* https://github.com/zarusz/SlimMessageBus
	* Use https://github.com/Cysharp/MessagePipe to send events to the engine
	* https://stackoverflow.com/questions/58549763/how-should-ipc-be-handled-in-net-core