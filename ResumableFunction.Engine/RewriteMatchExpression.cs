using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine
{
    //todo: update it to replace event paramter type to PushedEvent and member access to dictionary access
    public class RewriteMatchExpression : ExpressionVisitor
    {
        private readonly object _functionData;
        public LambdaExpression Result { get; set; }

        public RewriteMatchExpression(object data, LambdaExpression expression)
        {
            _functionData = data;
            Result = (LambdaExpression)Visit(expression);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (IsParamterAccess(node))
                return base.VisitMember(node);
            if (IsFunctionDataAccess(node))
            {
                if (node.Type.IsPrimitive || node.Type == typeof(string))
                    return Expression.Constant(GetValue(node), node.Type);
                else
                    throw new NotSupportedException($"Error when evaluating match expression (VisitMember),Can't convert [`{node}`] to constant.");
            }
            else
                throw new NotSupportedException($"Error when evaluating match expression (VisitMember),Can't translate [`{node}`].");
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            //return Expression.Parameter(typeof(PushedEvent), node.Name);
            return base.VisitParameter(node);
        }
        private bool IsParamterAccess(MemberExpression node)
        {
            if (node.Expression?.NodeType == ExpressionType.Parameter)
                return true;
            else if (node.Expression?.NodeType == ExpressionType.MemberAccess)
                return IsParamterAccess((MemberExpression)node.Expression);
            return false;
        }

        private bool IsFunctionDataAccess(MemberExpression node)
        {

            //is function data access 
            if (node.Member.Name == "Data" && node.Type == _functionData.GetType())
                return true;
            else if (node.Expression?.NodeType == ExpressionType.MemberAccess)
                return IsFunctionDataAccess((MemberExpression)node.Expression);
            return false;
        }

        private object GetValue(MemberExpression member)
        {
            var getterLambda = Expression.Lambda(member, Expression.Parameter(_functionData.GetType(), "Data"));
            var getter = getterLambda.Compile();
            return getter?.DynamicInvoke(_functionData);
        }


    }
}
