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
        protected EventWait<T> WaitEvent<T>(string eventIdentifier, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            var result = new EventWait<T>(eventIdentifier)
            {
                InitiatedByFunctionName = callerName,
                IsNode = true,
                WaitType = WaitType.EventWait
            };
            SetCommonProps(result);
            return result;
        }

        protected ManyEventsWait WaitEvents(string eventIdentifier, params EventWait[] events)
        {
            var result = ManyEvents(eventIdentifier, events);
            result.WaitType = WaitType.AllEventsWait;
            return result;
        }

        protected ManyEventsWait WaitAnyEvent(
            string eventIdentifier, params EventWait[] events)
        {
            var result = ManyEvents(eventIdentifier, events);
            result.WaitType = WaitType.AnyEventWait;
            return result;
        }

        private ManyEventsWait ManyEvents(string eventIdentifier, EventWait[] events)
        {
            var result = new ManyEventsWait
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

        private void SetCommonProps(Wait eventWaiting)
        {
            eventWaiting.FunctionRuntimeInfo = FunctionRuntimeInfo;
        }
    }

}
