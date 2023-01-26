using ResumableFunction.Abstraction.InOuts;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
namespace ResumableFunction.Engine
{
    public class RewriteSetPropExpression
    {
        public LambdaExpression Result { get; private set; }
        public RewriteSetPropExpression(EventWait wait)
        {
            var contextProp = wait.SetPropExpression;
            if (contextProp.Body is not MemberExpression me)
                throw new Exception("When you call `EventWait.SetProp` the body must be `MemberExpression`");

            var property = (PropertyInfo)me.Member;

            var functionDataParam = Parameter(wait.ParentFunctionState.DataType, "functionData");
            var dataPramterAccess = me.GetDataParamterAccess(functionDataParam, wait.ParentFunctionState.DataType);
            if (dataPramterAccess.IsFunctionData)
            {
                var eventDataParam = Parameter(wait.EventDataType, "eventData");
                Expression body = Assign(dataPramterAccess.NewExpression, eventDataParam);
                var block = Block(new[] { body, Empty() });

                Result = Lambda(block, functionDataParam, eventDataParam);
            }
            else
                throw new Exception("When you call `EventWait.SetProp` the body must be `MemberExpression` that use the `Data` property.");
        }
    }

}
