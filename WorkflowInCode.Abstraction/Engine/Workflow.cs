﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace WorkflowInCode.Abstraction.Engine
{
    public abstract class WorkflowInstance<ContextData>
    {
        protected ISubscribedEvent WhenAny<EventData>(
            params (ISubscribedEvent eventToWait, Expression<Func<EventData, bool>> matchFunction)[] events) => null;


        protected ISubscribedEvent WhenAll<EventData>(
            params (ISubscribedEvent eventToWait, Expression<Func<EventData, bool>> matchFunction)[] events) => null;

        protected ISubscribedEvent Wait<EventData>(
            ISubscribedEvent EventToWait,
            Expression<Func<EventData, bool>> MatchFunction,
            string ContextProp)
        {
            EventToWait.MatchFunction = MatchFunction;
            EventToWait.ContextProp = ContextProp;
            return EventToWait;
        }
        protected ISubscribedEvent WaitStartEvent<EventData>(
            ISubscribedEvent EventToWait,
            string ContextProp)
        {
            EventToWait.ContextProp = ContextProp;
            return EventToWait;
        }


        public ContextData InstanceData { get; protected set; }
        public void test(ISubscribedEvent dynamicEvent, ISubscribedEvent intEvent, ISubscribedEvent stringEvent)
        {
            //var x = Wait<dynamic>(dynamicEvent, o => o != null, (d, c) => c = d); ;
            //var y = Wait<int>(intEvent, x => x > 10);
            //var z = WhenAll<int>((intEvent, x => x == 10), (intEvent, x => x == 10), (intEvent, x => x == 10));
            //var zz = WhenAny<int>((intEvent, x => x == 10), (intEvent, x => x == 10), (intEvent, x => x == 10));
            //var a = WhenAll<object>((intEvent, x => x != null), (dynamicEvent, x => x != null), (stringEvent, x => x != null));
            //var aa = WhenAny<object>((intEvent, x => x != null), (dynamicEvent, x => x != null), (stringEvent, x => x != null));
        }

        public abstract IAsyncEnumerable<ISubscribedEvent> RunWorkflow();


    }
    public record EventWaiting<EventData, ContextData>(
            ISubscribedEvent EventToWait,
            Expression<Func<EventData, bool>> MatchFunction,
            Expression<Action<EventData, ContextData>> SetData);
    public record EventWaiting<ContextData>(
           ISubscribedEvent EventToWait,
           Expression<Func<object, bool>> MatchFunction,
           Expression<Action<object, ContextData>> SetData);
}
