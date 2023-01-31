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
            var result = new EventWait<T>(eventIdentifier)
            {
                InitiatedByFunctionName = callerName,
                IsNode = true
            };
            SetCommonProps(result);
            return result;
        }

        protected AllEventsWait WaitEvents(string eventIdentifier, params EventWait[] events)
        {
            return (AllEventsWait)ManyEvents(eventIdentifier, events);
        }

        protected AnyEventWait WaitAnyEvent(
            string eventIdentifier, params EventWait[] events)
        {
            return (AnyEventWait)ManyEvents(eventIdentifier, events);
        }

        private ManyWaits ManyEvents(string eventIdentifier, EventWait[] events)
        {
            var result = new ManyWaits
            {
                WaitingEvents = events.ToList(),
                EventIdentifier = eventIdentifier,
                InitiatedByFunctionName = events[0].InitiatedByFunctionName,
                IsNode = true,
            };
            foreach (var item in result.WaitingEvents)
            {
                SetCommonProps(item);
                item.ParentGroupId = result.Id;
                item.IsNode = false;
            }
            return result;
        }

        private void SetCommonProps(EventWait eventWaiting)
        {
            eventWaiting.FunctionRuntimeInfo = FunctionRuntimeInfo;
        }
    }

}
