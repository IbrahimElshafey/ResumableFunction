using Microsoft.Extensions.DependencyInjection;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.EfDataImplementation;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using static System.Linq.Expressions.Expression;
namespace ResumableFunction.Engine
{
    public static class Extensions
    {
        public static void AddFunctionEngine(this IServiceCollection services)
        {
            services.AddScoped<IWaitsRepository, WaitsRepository>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();
            services.AddScoped<IEventProviderRepository, EventProviderRepository>();
            services.AddScoped<IFunctionFolderRepository, FunctionFolderRepository>();
            services.AddScoped<FunctionEngine, FunctionEngine>();
        }

        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        public static object ToObject(this PushedEvent element, Type toConvertTo)
        {
            var json = JsonSerializer.Serialize(element);
            return JsonSerializer.Deserialize(json, toConvertTo, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static (bool IsFunctionData, MemberExpression? NewExpression) GetDataParamterAccess(
            this MemberExpression node,
            ParameterExpression functionInstanceArg)
        {
            var propAccessStack = new Stack<MemberInfo>();
            var isFunctionData = IsDataAccess(node);
            if (isFunctionData)
            {
                var newAccess = MakeMemberAccess(functionInstanceArg, propAccessStack.Pop());
                for (int i = 0; i < propAccessStack.Count; i++)
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
    }
}
