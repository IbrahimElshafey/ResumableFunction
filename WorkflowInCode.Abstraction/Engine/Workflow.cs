using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WorkflowInCode.Abstraction.Engine
{
    //public static class Workflow
    //{
    //    public static Input InputOf<Input, Output>(Func<Input,Output> func) { return default; }
    //    public static IWorkFlowPath Path(string path, params object[] nodes) { return null; }
    //}


    public abstract class Workflow
    {
        public Output[] WhenAny<Output>(params LongRunningTask<Output>[] tasks) => null;
        public Output[] WaitAll<Output>(params LongRunningTask<Output>[] tasks) => null;

        public object[] WhenAny(params LongRunningTask[] tasks) => null;
        public object[] WaitAll(params LongRunningTask[] tasks) => null;

        public Output Wait<Output>(LongRunningTask<Output> task) => default;
        public Output Wait<Output>(ISubscribedEvent<Output> task) => default;

        public Workflow()
        {
        }

        public void LogError(object message) { }
        public void LogWarning(object message) { }
        public void LogInformation(object message) { }

    }
}
    