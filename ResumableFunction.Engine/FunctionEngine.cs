using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumableFunction.Engine
{
    public class FunctionEngine
    {
        private readonly IWaitsRepository _waitsRepository;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;

        public FunctionEngine(
            IWaitsRepository waitsRepository,
            IFunctionRepository functionRepository,
            IEventProviderRepository eventProviderRepository)
        {
            _waitsRepository = waitsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
        }

        public Task ScanFunctionsFolder()
        {
            //find event providers and call RegisterEventProvider
            //find functions and call RegisterFunction
            //find event data converters and call RegisterEventDataConverter
            return Task.CompletedTask;
        }

        public Task RequestWait<FunctionData>(SingleEventWait eventWaiting, ResumableFunction<FunctionData> function) where FunctionData : class, new()
        {
            //todo:rerwite match expression and replace every FunctionData.Prop with constant value

            /// Will execueted when a Function instance run and return ask for EventWaiting.<br/>
            /// * Find event provider or load it.<br/>
            /// * Start event provider if not started <br/>
            /// * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
            /// * Save event to IActiveEventsRepository <br/>
            /// ** important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
            /// but the provider will send this data back
            return Task.CompletedTask;
        }


        public async Task WhenProviderPushEvent(PushedEvent pushedEvent)
        {
            //pushed event  comes to the engine from event provider 
            //pushed event contains properties (ProviderName,EventIdentifier,EventData as props)
            //*  
            //* 
            //*  and start/resume active instance Function
            //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider



            //engine search waits list with(ProviderName, EventType)
            var matchedEvents = await _waitsRepository.GetEventWaits(pushedEvent);
            //and pass payload to match expression
            matchedEvents = matchedEvents.Where(x => x.IsMatch(pushedEvent)).ToList();
            //engine now know related function instances list
            foreach (var eventWait in matchedEvents)
            {
                var state = await _functionRepository
                    .GetFunctionState(eventWait.FunctionId, eventWait.InitiatedByFunction);

                //load context data
                var functionData = await _functionRepository.GetFunctionData(eventWait.FunctionId);
                var functionClass = (ResumableFunction<object>)Activator.CreateInstance(eventWait.InitiatedByClass);
                functionClass.Data = functionData;
                var runner = new FunctionRunner(eventWait, state, functionClass);
                SetFunctionData(functionData, "propname", pushedEvent);
                await _functionRepository.SaveFunctionData(functionData, eventWait.FunctionId, eventWait.InitiatedByClass.FullName);

                //get next event wait
                var waitResult = await runner.Run();
            }
            //return Task.CompletedTask;
        }

        /// <summary>
        /// will be called by the engine after event received
        /// </summary>
        //public void SetFunctionData(object data)
        //{
        //    //todo:check type && me.Type
        //    var contextProp = _currentWait.SetPropExpression;
        //    if (contextProp.Body is MemberExpression me)
        //    {
        //        var property = (PropertyInfo)me.Member;

        //        var FunctionDataParam = Expression.Parameter(typeof(object), "functionData");
        //        var eventDataParam = Expression.Parameter(typeof(object), "eventData");

        //        var isValueType = property.PropertyType.IsClass == false && property.PropertyType.IsInterface == false;

        //        Expression valueExpression;
        //        if (isValueType)
        //            valueExpression = Expression.Unbox(eventDataParam, property.PropertyType);
        //        else
        //            valueExpression = Expression.Convert(eventDataParam, property.PropertyType);

        //        var thisExpression = Expression.Property(Expression.Convert(FunctionDataParam, typeof(FunctionData)), property);


        //        Expression body = Expression.Assign(thisExpression, valueExpression);

        //        var block = Expression.Block(new[]
        //        {
        //                            body,
        //                            Expression.Empty ()
        //                        });

        //        var lambda = Expression.Lambda(block, FunctionDataParam, eventDataParam);

        //        var set = lambda.Compile() as Action<object, object>;
        //        if (set != null && _function.Data != null)
        //            set(_function.Data, data);
        //    }

        //}

        private void SetFunctionData(object functionData, string contextProp, object eventData)
        {

            var piInstance = functionData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(functionData, eventData);
            //save data to database
        }
    }
}
