using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace Test
{
    public static partial class Program
    {
        public class InlineRewiteExpression : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _whenMatch;
            private readonly Func<Expression> _replaceWith;

            public Expression Result { get; private set; }
            public InlineRewiteExpression(
                Expression expressionToUpdate,
                Func<Expression, bool> whenMatch,
                Func<Expression> replaceWith)
            {
                _whenMatch = whenMatch;
                _replaceWith = replaceWith;
                Result = Visit(expressionToUpdate);
            }



            [return: NotNullIfNotNull("node")]
            public override Expression? Visit(Expression? node)
            {
                if (_whenMatch(node))
                    return _replaceWith();
                return base.Visit(node);
            }
        }
    }

}
