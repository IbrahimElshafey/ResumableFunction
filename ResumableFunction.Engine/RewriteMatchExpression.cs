using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;
namespace ResumableFunction.Engine
{
    public class RewriteMatchExpression : ExpressionVisitor
    {
        private readonly object _functionData;
        public LambdaExpression Result { get; private set; }
        private readonly ParameterExpression _dataParamter;
        public RewriteMatchExpression(object data, Expression expression)
        {
            _functionData = data;
            _dataParamter = Parameter(data.GetType(), "functionData");

            var updatedBoy = (LambdaExpression)Visit(expression);
            var functionType = typeof(Func<,,>).MakeGenericType(_functionData.GetType(), updatedBoy.Parameters[0].Type, typeof(bool));
            Result = Lambda(functionType, updatedBoy.Body, _dataParamter, updatedBoy.Parameters[0]);
            //Result = (LambdaExpression)Visit(Result);
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            //replace [FunctionClass].Data.Prop with [_dataParamter.Prop] or constant value
            var x = node.GetDataParamterAccess(_dataParamter, _functionData.GetType());
            if (x.IsFunctionData)
            {
                //if (IsBasicType(node.Type))
                //    return Constant(GetValue(x.NewExpression), node.Type);
                //NeedFunctionData = true;
                return x.NewExpression;
            }
            return base.VisitMember(node);
        }


        private bool IsBasicType(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }


        private object GetValue(MemberExpression node)
        {
            var getterLambda = Lambda(node, _dataParamter);
            var getter = getterLambda.Compile();
            return getter?.DynamicInvoke(_functionData);
        }
    }

}
