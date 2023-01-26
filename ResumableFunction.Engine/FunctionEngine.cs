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
    public partial class FunctionEngine
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
            IOptions<EngineSettings> engineSettings)
        {
            _waitsRepository = waitsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
            _functionFolderRepository = functionFolderRepository;
            _settings = engineSettings.Value;
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
        private async Task WaitRequested(NextWaitResult waitResult, ResumableFunctionWrapper functionClass)
        {
            switch (waitResult.Result)
            {
                case null when waitResult.IsFinalExit:
                    await _functionRepository.MoveFunctionToRecycleBin(functionClass.FunctionState);
                    break;
                case EventWait eventWait:
                    await EventWaitRequested(eventWait, functionClass);
                    break;
            }


        }

        private async Task EventWaitRequested(EventWait eventWait, ResumableFunctionWrapper functionClass)
        {
            // * Rerwite match expression and set prop expresssion
            eventWait.MatchExpression = new RewriteMatchExpression(functionClass.Data, eventWait.MatchExpression).Result;
            eventWait.SetPropExpression = new RewriteSetPropExpression(eventWait).Result;
            // * Find event provider handler or load it.
            var eventProviderHandler = await _eventProviderRepository.GetByName(eventWait.EventProviderName);
            // * Start event provider if not started 
            await eventProviderHandler.Start();
            // * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
            await eventProviderHandler.SubscribeToEvent(eventWait.EventData);
            // * Save event to IActiveEventsRepository 
            await _waitsRepository.AddWait(eventWait); 
            // ** important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
            // but the provider will send this data back
        }
    }
}
