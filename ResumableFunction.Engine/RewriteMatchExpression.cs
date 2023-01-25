using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;
namespace ResumableFunction.Engine
{
    public class RewriteMatchExpression : ExpressionVisitor
    {
        private readonly object _functionData;
        public bool NeedFunctionData { get; private set; }
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
            var x = IsFunctionDataAccess(node);
            if (x.IsFunctionData)
            {
                if (IsBasicType(node.Type))
                    return Constant(GetValue(x.NewExpression), node.Type);
                NeedFunctionData = true;
                return x.NewExpression;
            }
            return base.VisitMember(node);
        }


        private bool IsBasicType(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        private (bool IsFunctionData, MemberExpression? NewExpression) IsFunctionDataAccess(MemberExpression node)
        {
            var propAccessStack = new Stack<MemberInfo>();
            var isFunctionData = IsDataAccess(node);
            if (isFunctionData)
            {
                propAccessStack.Pop();
                var newAccess = MakeMemberAccess(_dataParamter, propAccessStack.Pop());
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
                //is function data access 
                if (currentNode.Member.Name == "Data" && currentNode.Type == _functionData.GetType())
                    return true;
                else if (currentNode.Expression?.NodeType == ExpressionType.MemberAccess)
                    return IsDataAccess((MemberExpression)currentNode.Expression);
                return false;
            }
        }
        private object GetValue(MemberExpression node)
        {
            var getterLambda = Lambda(node, _dataParamter);
            var getter = getterLambda.Compile();
            return getter?.DynamicInvoke(_functionData);
        }
    }
    //public class RewriteMatchExpression : ExpressionVisitor
    //{
    //    private readonly object _functionData;
    //    public LambdaExpression Result { get; private set; }

    //    public RewriteMatchExpression(object data, LambdaExpression expression)
    //    {
    //        _functionData = data;
    //        Result = (LambdaExpression)Visit(expression);
    //    }

    //    protected override Expression VisitMember(MemberExpression node)
    //    {
    //        if (IsParamterAccess(node))
    //            return base.VisitMember(node);
    //        if (IsFunctionDataAccess(node))
    //        {
    //            if (node.Type.IsPrimitive || node.Type == typeof(string))
    //                return Expression.Constant(GetValue(node), node.Type);
    //            else
    //                throw new NotSupportedException($"Error when evaluating match expression (VisitMember),Can't convert [`{node}`] to constant.");
    //        }
    //        else
    //            throw new NotSupportedException($"Error when evaluating match expression (VisitMember),Can't translate [`{node}`].");
    //    }
    //    protected override Expression VisitParameter(ParameterExpression node)
    //    {
    //        //return Expression.Parameter(typeof(PushedEvent), node.Name);
    //        return base.VisitParameter(node);
    //    }
    //    private bool IsParamterAccess(MemberExpression node)
    //    {
    //        if (node.Expression?.NodeType == ExpressionType.Parameter)
    //            return true;
    //        else if (node.Expression?.NodeType == ExpressionType.MemberAccess)
    //            return IsParamterAccess((MemberExpression)node.Expression);
    //        return false;
    //    }

    //    private bool IsFunctionDataAccess(MemberExpression node)
    //    {

    //        //is function data access 
    //        if (node.Member.Name == "Data" && node.Type == _functionData.GetType())
    //            return true;
    //        else if (node.Expression?.NodeType == ExpressionType.MemberAccess)
    //            return IsFunctionDataAccess((MemberExpression)node.Expression);
    //        return false;
    //    }

    //    private object GetValue(MemberExpression member)
    //    {
    //        var getterLambda = Expression.Lambda(member, Expression.Parameter(_functionData.GetType(), "Data"));
    //        var getter = getterLambda.Compile();
    //        return getter?.DynamicInvoke(_functionData);
    //    }


    //}
}
