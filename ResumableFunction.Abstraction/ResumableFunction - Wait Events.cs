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
        protected EventWait<T> WaitEvent<T>(string eventIdentifier, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            var result = new EventWait<T>(eventIdentifier) { InitiatedByFunction = callerName };
            SetCommonProps(result);
            return result;
        }

        protected AllEventsWait WaitEvents(string eventIdentifier, params EventWait[] events)
        {
            var result = new AllEventsWait
            {
                WaitingEvents = events,
                EventIdentifier = eventIdentifier,
                InitiatedByFunction = events[0].InitiatedByFunction
            };
            foreach (var item in result.WaitingEvents)
            {
                SetCommonProps(item);
                item.ParentGroupId = result.Id;
            }
            return result;
        }

        protected AnyEventWait WaitAnyEvent(
            string eventIdentifier, params EventWait[] events)
        {
            var result = new AnyEventWait
            {
                WaitingEvents = events,
                EventIdentifier = eventIdentifier,
                InitiatedByFunction = events[0].InitiatedByFunction
            };
            foreach (var item in result.WaitingEvents)
            {
                SetCommonProps(item);
                item.ParentGroupId = result.Id;
            }
            return result;
        }

        private void SetCommonProps(EventWait eventWaiting)
        {
            eventWaiting.FunctionRuntimeInfo = FunctionRuntimeInfo;
        }
    }

}
