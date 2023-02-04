using Microsoft.Extensions.DependencyInjection;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.EfDataImplementation;
using ResumableFunction.Engine.InOuts;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using static System.Linq.Expressions.Expression;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ResumableFunction.Engine.Helpers
{
    public static class Extensions
    {
        public static void AddFunctionEngine(this IMvcBuilder mvcBuilder, EngineSettings settings)
        {
            mvcBuilder.AddApplicationPart(typeof(EventReceiverController).Assembly).AddControllersAsServices();
            var services = mvcBuilder.Services;
            services.AddDbContext<EngineDataContext>(options =>
            {
                var config = settings;
                if (config.ProviderName.ToLower().Equals("sqlite"))
                {
                    options.UseSqlite(
                        config.SqliteConnection!,
                        //x => x.MigrationsAssembly(typeof(SqliteMarker).Assembly.GetName().Name)
                        x => x.MigrationsAssembly("ResumableFunction.Engine.Data.Sqlite")
                    );
                }
                if (config.ProviderName.ToLower().Equals("sqlserver"))
                {
                    options.UseSqlServer(
                        config.SqlServerConnection!,
                        x => x.MigrationsAssembly("ResumableFunction.Engine.Data.SqlServer")
                    );
                }
            });

            services.AddScoped<IWaitsRepository, WaitsRepository>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();
            services.AddScoped<IEventProviderRepository, EventProviderRepository>();
            services.AddScoped<IFunctionFolderRepository, FunctionFolderRepository>();
            services.AddScoped<FunctionEngine, FunctionEngine>();
        }

        //public static T ToObject<T>(this JsonElement element)
        //{
        //    var json = element.GetRawText();
        //    return JsonSerializer.Deserialize<T>(json);
        //}

        public static object ToObject(this PushedEvent element, Type toConvertTo)
        {
            var json = JsonConvert.SerializeObject(element);
            //new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            return JsonConvert.DeserializeObject(json, toConvertTo);
        }
        //todo:if int return is not correct
        public static (bool IsFunctionData, MemberExpression? NewExpression) GetDataParamterAccess(
            this MemberExpression node,
            ParameterExpression functionInstanceArg)
        {
            var propAccessStack = new Stack<MemberInfo>();
            var isFunctionData = IsDataAccess(node);
            if (isFunctionData)
            {
                var newAccess = MakeMemberAccess(functionInstanceArg, propAccessStack.Pop());
                while (propAccessStack.Count > 0)
                {
                    var currentProp = propAccessStack.Pop();
                    newAccess = MakeMemberAccess(newAccess, currentProp);
                }
                return (true, newAccess);
            }
            return (false, null);

            bool IsDataAccess(MemberExpression currentNode)
            {
                propAccessStack.Push(currentNode.Member);
                var subNode = currentNode.Expression;
                if (subNode == null) return false;
                //is function data access 
                var isFunctionDataAccess = subNode.NodeType == ExpressionType.Constant && subNode.Type == functionInstanceArg.Type;
                if (isFunctionDataAccess)
                    return true;
                else if (subNode.NodeType == ExpressionType.MemberAccess)
                    return IsDataAccess((MemberExpression)subNode);
                return false;
            }
        }

        private static Assembly _currentFunctionAssembly;
        public static Assembly GetCurrentFunctionAssembly() => _currentFunctionAssembly;
        public static void SetCurrentFunctionAssembly(Assembly assembly)
            => _currentFunctionAssembly = assembly;
    }
}
