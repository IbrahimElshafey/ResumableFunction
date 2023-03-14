using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.EfDataImplementation;
using ResumableFunction.Engine.InOuts;

namespace ResumableFunction.Engine
{
    public partial class FunctionEngine
    {
        private readonly IWaitsRepository _waitsRepository;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;
        private readonly IFunctionFolderRepository _functionFolderRepository;
        private readonly EngineSettings _settings;
        private readonly EngineDataContext _context;

        public FunctionEngine(
            IWaitsRepository waitsRepository,
            IFunctionRepository functionRepository,
            IEventProviderRepository eventProviderRepository,
            IFunctionFolderRepository functionFolderRepository,
            IOptions<EngineSettings> engineSettings,
            EngineDataContext dbContext)
        {
            _waitsRepository = waitsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
            _functionFolderRepository = functionFolderRepository;
            _settings = engineSettings.Value;
            _context = dbContext;
        }

        public async Task ScanFunctionsFolders()
        {
            await ScanEachFolder(
                await _functionFolderRepository.GetFunctionFolders()
                );
        }

        internal async Task ScanEachFolder(List<FunctionFolder> functionFolders)
        {
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
                    await RegisterTypes(GetTypes(dllPath), folder);
                }
            }
        }

        private async Task RegisterTypes(Type[] types, FunctionFolder folder)
        {
            var functionTypes = new List<Type>();
            var eventProviderTypes = new List<Type>();
            foreach (var type in types)
            {
                //find functions and call RegisterFunction
                if (type.IsSubclassOf(typeof(ResumableFunctionInstance)))
                    functionTypes.Add(type);
                   
                //find event providers and call RegisterEventProvider
                else if (typeof(IEventProviderHandler).IsAssignableFrom(type))
                    eventProviderTypes.Add(type);
                    
            }
            foreach (var eventProviderType in eventProviderTypes)
            {
                await _eventProviderRepository.RegsiterEventProvider(eventProviderType, folder);
            }
            foreach (var functionType in functionTypes)    
            {
                await RegisterResumableFunction(folder, functionType);
            }
            await _context.SaveChangesAsync();
        }

        private async Task RegisterResumableFunction(FunctionFolder folder, Type type)
        {
            var instance = (ResumableFunctionInstance)Activator.CreateInstance(type)!;
            if (instance != null && await _functionRepository.RegisterFunction(instance, folder))
            {
                var functionClass = new ResumableFunctionWrapper(instance);
                var runner = functionClass.GetCurrentRunner();
                if (runner != null && await runner.MoveNextAsync())
                {
                    var firstWait = runner.Current;
                    firstWait.IsFirst = true;
                    firstWait.StateAfterWait = functionClass.GetActiveRunnerState();
                    await GenericWaitRequested(firstWait);
                }
                //Log no waits exist for function type x
            }
        }
        private Type[] GetTypes(string assemblyPath)
        {

            //string[] runtimeDlls = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            //var resolver = new PathAssemblyResolver(new List<string>(runtimeDlls) { assemblyPath });

            //_metadataContext = new MetadataLoadContext(resolver);
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            return assembly.GetTypes();
        }

        private bool FolderNeedScan(FunctionFolder functionFolder)
        {
            //exclude "ResumableFunction.Abstraction"
            if (Directory.Exists(functionFolder.Path))
            {
                var dlls = Directory
                .GetFiles(functionFolder.Path, "*.dll")
                .Where(dllPath => !dllPath.EndsWith("ResumableFunction.Abstraction.dll"))
                .Select(x => new FileInfo(x))
                .ToList();
                foreach (var dll in dlls)
                {
                    if (dll.LastWriteTime > functionFolder.LastScanDate)
                        functionFolder.NeedScanDlls.Add(dll.Name);
                }
                return functionFolder.NeedScanDlls?.Any() ?? false;
            }
            return false;
        }


    }
}
