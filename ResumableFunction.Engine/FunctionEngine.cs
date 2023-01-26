using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumableFunction.Engine
{
    public class FunctionEngine
    {
        private readonly IWaitsRepository _waitsRepository;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;
        private readonly IFunctionFolderRepository _functionFolderRepository;
        private readonly EngineSettings _settings;

        public FunctionEngine(
            IWaitsRepository waitsRepository,
            IFunctionRepository functionRepository,
            IEventProviderRepository eventProviderRepository,
            IFunctionFolderRepository functionFolderRepository,
            IOptions<EngineSettings> options)
        {
            _waitsRepository = waitsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
            _functionFolderRepository = functionFolderRepository;
            _settings = options.Value;
        }

        public async Task ScanFunctionsFolder()
        {
            var functionFolders = await _functionFolderRepository.GetFunctionFolders();
            foreach (var folder in functionFolders)
            {
                //check if folder DLLs need to be scan
                if (FolderNeedScan(folder) is false) continue;
                folder.LastScanDate = DateTime.Now;
                foreach (var dllName in folder.NeedScanDlls)
                {
                    var dllPath = Path.Combine(folder.Path, dllName);
                    //check if assembly use the same .net version
                    //get types in assembly without loading and register them
                    await RegisterTypes(GetTypes(dllPath));

                }
            }
        }

        private async Task RegisterTypes(Type[] types)
        {
            foreach (var type in types)
            {
                //find functions and call RegisterFunction
                if (type.IsSubclassOfRawGeneric(typeof(ResumableFunction<>)))
                    await _functionRepository.RegisterFunction(type);
                //find event providers and call RegisterEventProvider
                else if (typeof(IEventProviderHandler).IsAssignableFrom(type))
                    await _eventProviderRepository.RegsiterEventProvider(type);
            }
        }

        private Type[] GetTypes(string assemblyPath)
        {

            string[] runtimeDlls = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var resolver = new PathAssemblyResolver(new List<string>(runtimeDlls) { assemblyPath });

            using (var metadataContext = new MetadataLoadContext(resolver))
            {
                Assembly assembly = metadataContext.LoadFromAssemblyPath(assemblyPath);
                return assembly.GetTypes();
            }
        }

        private bool FolderNeedScan(FunctionFolder functionFolder)
        {
            var result = new FunctionFolder();
            //exclude "ResumableFunction.Abstraction"
            var dlls = Directory
                .GetFiles(functionFolder.Path, "*.dll")
                .Except(new[] { "ResumableFunction.Abstraction.dll" })
                .Select(x => new FileInfo(x));
            foreach (var dll in dlls)
            {
                if (dll.LastWriteTime > functionFolder.LastScanDate)
                    functionFolder.NeedScanDlls.Add(dll.Name);
            }
            return functionFolder.NeedScanDlls?.Any() ?? false;
        }

        /// <summary>
        /// Will execueted when a function instance run and ask for EventWaiting.
        /// </summary>
        public Task WaitRequested(WaitResult1 waitResult, object functionClass)
        {
            //todo:rerwite match expression and replace every FunctionData.Prop with constant value

            /// <br/>
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
            //engine search waits list with(ProviderName, EventType)
            foreach (var eventWait in await MatchedWaits(pushedEvent))
            {
                var state =
                    await _functionRepository.GetFunctionState(eventWait.FunctionId, eventWait.InitiatedByFunction);

                //load context data
                var functionData = await _functionRepository.GetFunctionData(eventWait.FunctionId);
                var functionClass = (ResumableFunction<object>)Activator.CreateInstance(eventWait.InitiatedByClass);
                functionClass.Data = functionData;
                var runner = new FunctionRunner(eventWait, state, functionClass);
                SetFunctionData(functionData, "propname", pushedEvent);

                //*  and start/resume active instance Function
                //get next event wait
                var waitResult = await runner.Run();
                await _functionRepository
                    .SaveFunctionData(functionData, eventWait.FunctionId, eventWait.InitiatedByClass.FullName);
                await WaitRequested(waitResult, functionClass);
                //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
            }
            //return Task.CompletedTask;
        }

        private async Task<List<EventWait>> MatchedWaits(PushedEvent pushedEvent)
        {
            var matchedEvents = await _waitsRepository.GetEventWaits(pushedEvent.EventIdentifier, pushedEvent.EventProviderName);
            //and pass payload to match expression
            // matchedEvents = matchedEvents.Where(x => x.IsMatch(pushedEvent)).ToList();
            //engine now know related function instances list
            return matchedEvents;
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
