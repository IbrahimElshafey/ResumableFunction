**Project Status: Under Analysis**

# Why this project?
I want to write code that reflects the business requirements so that a developer handover another without needing business documents to understand the code.

A project is a set of classes and methods and this didn't tell us about the flow.


I evaluated the existing solutions and found that there is no solution that fits all scenarios
* [MassTransit](https://masstransit-project.com/)
* [Durable Task Framework](https://github.com/Azure/durabletask)
* [D-Async (The best)](https://github.com/Dasync/Dasync)
* [Workflow Core](https://github.com/danielgerlag/workflow-core)
* [Infinitic (Kotlin)](https://github.com/infiniticio/infinitic)

# I will use IAsyncEnumerable generated state machne to implement a method that can be paused and resumed 
