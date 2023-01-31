using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{

    public abstract partial class ResumableFunction<FunctionData>
    {
        //Must be a node with no parents
        protected Wait Replay<T>(string eventIdentifier = null, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            var eventToReplay = FunctionRuntimeInfo.FunctionWaits
                 .LastOrDefault(x =>
                     x is EventWait ew &&
                     (eventIdentifier == null || ew.EventIdentifier == eventIdentifier) &&
                     ew.EventDataType == typeof(T) &&
                     ew.IsNode &&
                     x.InitiatedByFunctionName == callerName &&
                     x.Status == WaitStatus.Completed);
            if (eventToReplay is null)
                throw new Exception($"Event replay failed, no old wait exist.");
            eventToReplay.Status = WaitStatus.Waiting;
            return eventToReplay;
        }

        protected Wait Replay(string eventIdentifier = "", [CallerMemberName] string callerName = "")
        {
            var eventToReplay = FunctionRuntimeInfo.FunctionWaits
                 .Last(x =>
                 x.EventIdentifier == eventIdentifier &&
                 x.IsNode &&
                 x.InitiatedByFunctionName == callerName &&
                 x.Status == WaitStatus.Completed);
            if (eventToReplay is null)
                throw new Exception($"Event replay failed, no old wait exist.");
            eventToReplay.Status = WaitStatus.Waiting;
            return eventToReplay;
        }
    }

}
