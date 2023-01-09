using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace WorkflowInCode.Abstraction.Engine
{
    public abstract class WorkflowInstance<ContextData>
    {
        protected IWaitAnyEvent WaitFirstEvent<EventData>(
            params EventWaiting<EventData>[] events) => null;


        protected IWaitAllEvent WaitEvents<EventData>(
            params EventWaiting<EventData>[] events)
        {
            return null;
        }

        protected IEvent WaitEvent<EventData>(
            IEvent eventToWait,
            Expression<Func<EventData, bool>> matchFunction,
            Expression<Func<EventData>> contextProp)
        {
            eventToWait.MatchFunction = matchFunction;
            eventToWait.ContextProp = contextProp;
            return eventToWait;
        }

        public string InstanceId { get; protected set; }
        public ContextData InstanceData { get; protected set; }
        public async Task SaveInstanceData()
        {
        }
        public void test(IEvent dynamicEvent, IEvent intEvent, IEvent stringEvent)
        {
            //var x = Wait<dynamic>(dynamicEvent, o => o != null, (d, c) => c = d); ;
            //var y = Wait<int>(intEvent, x => x > 10);
            //var z = WhenAll<int>((intEvent, x => x == 10), (intEvent, x => x == 10), (intEvent, x => x == 10));
            //var zz = WhenAny<int>((intEvent, x => x == 10), (intEvent, x => x == 10), (intEvent, x => x == 10));
            //var a = WhenAll<object>((intEvent, x => x != null), (dynamicEvent, x => x != null), (stringEvent, x => x != null));
            //var aa = WhenAny<object>((intEvent, x => x != null), (dynamicEvent, x => x != null), (stringEvent, x => x != null));
        }

        public abstract IAsyncEnumerable<IEvent> RunWorkflow();

    }
    public record EventWaiting<EventData>(
            IEvent EventToWait,
            Expression<Func<EventData, bool>> MatchFunction,
            Expression<Func<EventData>> ContextProp)
    {
        public bool IsOptional { get; set; }
    }
}
