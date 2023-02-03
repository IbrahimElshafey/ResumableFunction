using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Helpers;
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
        private readonly EventWait _wait;
        public LambdaExpression Result { get; private set; }
        private readonly ParameterExpression _functionInstanceArg;
        public RewriteMatchExpression(EventWait wait)
        {
            _wait = wait;
            _functionInstanceArg = Parameter(wait.CurrntFunction.GetType(), "functionInstance");

            var updatedBoy = (LambdaExpression)Visit(wait.MatchExpression);
            var functionType = typeof(Func<,,>)
                .MakeGenericType(wait.CurrntFunction.GetType(), updatedBoy.Parameters[0].Type, typeof(bool));
            Result = Lambda(functionType, updatedBoy.Body, _functionInstanceArg, updatedBoy.Parameters[0]);
            //Result = (LambdaExpression)Visit(Result);
        }


        //protected override Expression VisitConstant(ConstantExpression node)
        //{
        //    if (node.Type == _wait.CurrntFunction.GetType())
        //        return _functionInstanceArg;
        //    return base.VisitConstant(node);
        //}

        protected override Expression VisitMember(MemberExpression node)
        {
            //replace [FunctionClass].Data.Prop with [_dataParamter.Prop] or constant value
            var x = node.GetDataParamterAccess(_functionInstanceArg);
            if (x.IsFunctionData)
            {
                if (IsBasicType(node.Type))
                    return Constant(GetValue(x.NewExpression), node.Type);
                _wait.NeedFunctionDataForMatch = true;
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
            var getterLambda = Lambda(node, _functionInstanceArg);
            var getter = getterLambda.Compile();
            return getter?.DynamicInvoke(_wait.CurrntFunction);
        }
    }

}
