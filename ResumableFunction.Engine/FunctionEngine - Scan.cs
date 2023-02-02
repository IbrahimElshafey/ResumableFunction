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

        public async Task ScanFunctionsFolder()
        {
            var functionFolders = await _functionFolderRepository.GetFunctionFolders();
            await ScanEachFolder(functionFolders);
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
                    await RegisterTypes(GetTypes(dllPath));

                }
            }
        }

        private async Task RegisterTypes(Type[] types)
        {
            foreach (var type in types)
            {
                //find functions and call RegisterFunction
                if (type.IsSubclassOf(typeof(ResumableFunctionInstance)))
                    if (await _functionRepository.RegisterFunction(type))
                    {
                        //create new instance and add strart it
                        var instance = (ResumableFunctionInstance)Activator.CreateInstance(type);
                        if (instance != null)
                        {
                            var runner = instance.Start().GetAsyncEnumerator();
                            if (await runner.MoveNextAsync())
                            {
                                var firstWait = runner.Current;
                                await GenericWaitRequested(firstWait);
                            }
                            else
                            {
                                //Log no waits exist for function type x
                            }

                        }
                        //add first wait to waits list
                    }
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


    }
}
