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

    public abstract partial class ResumableFunctionInstance
    {
        protected Wait GoBackAfter<T>(string eventIdentifier = null, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            Wait eventToReplay = GetWait<T>(eventIdentifier, callerName);
            eventToReplay.ReplayType = ReplayType.ExecuteDontWait;
            return eventToReplay;
        }
        protected Wait GoBackTo<T>(string eventIdentifier = null, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            Wait eventToReplay = GetWait<T>(eventIdentifier, callerName);
            eventToReplay.ReplayType = ReplayType.WaitSameEventAgain;
            return eventToReplay;
        }

        protected Wait GoBackAfter(string eventIdentifier, [CallerMemberName] string callerName = "")
        {
            Wait eventToReplay = GetWait(eventIdentifier, callerName);
            eventToReplay.ReplayType = ReplayType.ExecuteDontWait;
            return eventToReplay;
        }
        protected Wait GoBackTo(string eventIdentifier, [CallerMemberName] string callerName = "")
        {
            Wait eventToReplay = GetWait(eventIdentifier, callerName);
            eventToReplay.ReplayType = ReplayType.WaitSameEventAgain;
            return eventToReplay;
        }

        private Wait GetWait(string eventIdentifier, string callerName)
        {
            var eventToReplay = FunctionRuntimeInfo.Waits
                 .Last(x =>
                 x.EventIdentifier == eventIdentifier &&
                 x.IsNode &&
                 x.InitiatedByFunctionName == callerName &&
                 x.Status == WaitStatus.Completed);
            if (eventToReplay is null)
                throw new Exception($"Event go back failed, no old wait exist.");
            return eventToReplay;
        }

        private Wait GetWait<T>(string eventIdentifier, string callerName) where T : class, IEventData, new()
        {
            var eventToReplay = FunctionRuntimeInfo.Waits
                 .LastOrDefault(x =>
                     x is EventWait ew &&
                     (eventIdentifier == null || ew.EventIdentifier == eventIdentifier) &&
                     ew.EventDataType == typeof(T) &&
                     ew.IsNode &&
                     x.InitiatedByFunctionName == callerName &&//go back to event in same function
                     x.Status == WaitStatus.Completed);
            if (eventToReplay is null)
                throw new Exception($"Event go back failed, no old wait exist.");
            return eventToReplay;
        }
    }

}
