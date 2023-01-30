using ResumableFunction.Abstraction.InOuts;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
namespace ResumableFunction.Engine
{
    public class RewriteSetDataExpression
    {
        public LambdaExpression Result { get; private set; }
        public RewriteSetDataExpression(EventWait wait)
        {
            var contextProp = wait.SetDataExpression;
            if (contextProp.Body is not MemberExpression me)
                throw new Exception("When you call `EventWait.SetProp` the body must be `MemberExpression`");

            var property = (PropertyInfo)me.Member;

            var functionDataParam = Parameter(wait.FunctionRuntimeInfo.DataType, "functionData");
            var dataPramterAccess = me.GetDataParamterAccess(functionDataParam, wait.FunctionRuntimeInfo.DataType);
            if (dataPramterAccess.IsFunctionData && dataPramterAccess.NewExpression != null)
            {
                var eventDataParam = Parameter(wait.EventDataType, "eventData");
                var isGenericList = dataPramterAccess.NewExpression.Type.IsGenericType &&
                    dataPramterAccess.NewExpression.Type.GetGenericTypeDefinition() == typeof(List<>);
                if (isGenericList)
                {
                    var body = Call(dataPramterAccess.NewExpression, dataPramterAccess.NewExpression.Type.GetMethod("Add"), eventDataParam);
                    Result = Lambda(body, functionDataParam, eventDataParam);
                }
                else
                {
                    Expression body = Assign(dataPramterAccess.NewExpression, eventDataParam);
                    var block = Block(new[] { body, Empty() });
                    Result = Lambda(block, functionDataParam, eventDataParam);
                }

            }
            else
                throw new Exception("When you call `EventWait.SetProp` the body must be `MemberExpression` that use the `Data` property.");
        }
    }

}
