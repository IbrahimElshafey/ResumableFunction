using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    public abstract partial class ResumableFunction<ContextData>
    {
        private object? _activeRunner;
        //todo:set private 
        public void SetActiveRunnerState(int state)
        {
            if (_activeRunner != null || GetActiveRunner() != null)
            {
                var stateField = _activeRunner?.GetType().GetField("<>1__state");
                if (stateField != null) 
                {
                    
                    stateField.SetValue(_activeRunner, state);
                }
            }
        }
        //todo:set private
        public int GetActiveRunnerState()
        {
            if (_activeRunner != null || GetActiveRunner() != null)
            {
                var stateField = _activeRunner?.GetType().GetField("<>1__state");
                if (stateField != null)
                {
                    return (int)stateField.GetValue(_activeRunner);
                }
            }
            return int.MinValue;
        }
        private IAsyncEnumerator<EventWaitingResult>? GetActiveRunner()
        {
            if (_activeRunner == null)
            {
                var FunctionRunnerType = GetType()
                   .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SuppressChangeType)
                   .FirstOrDefault(x => x.Name.StartsWith("<RunFunction>"));

                if (FunctionRunnerType == null) return null;
                ConstructorInfo? ctor = FunctionRunnerType.GetConstructor(
                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                   new Type[] { typeof(int) });

                if (ctor == null) return null;
                _activeRunner =
                    ctor.Invoke(new object[] { -2 });

                if (_activeRunner == null) return null;
                //set parent class who call
                var thisField = FunctionRunnerType.GetFields().FirstOrDefault(x => x.Name.EndsWith("__this"));
                //var thisField = FunctionRunnerType.GetField("<>4__this");
                thisField?.SetValue(_activeRunner, this);

                SetActiveRunnerState(int.MinValue);
                return _activeRunner as IAsyncEnumerator<EventWaitingResult>;
            }
            return _activeRunner as IAsyncEnumerator<EventWaitingResult>;
        }

        private void SetContextData(ContextData FunctionData, LambdaExpression contextProp, object eventData)
        {
            //todo:check type &&me.Type
            if (contextProp.Body is MemberExpression me)
            {
                var property = (PropertyInfo)me.Member;

                var FunctionDataParam = Expression.Parameter(typeof(object), "FunctionData");
                var eventDataParam = Expression.Parameter(typeof(object), "eventData");

                var isValueType = property.PropertyType.IsClass == false && property.PropertyType.IsInterface == false;

                Expression valueExpression;
                if (isValueType)
                    valueExpression = Expression.Unbox(eventDataParam, property.PropertyType);
                else
                    valueExpression = Expression.Convert(eventDataParam, property.PropertyType);

                var thisExpression = Expression.Property(Expression.Convert(FunctionDataParam, FunctionData.GetType()), property);


                Expression body = Expression.Assign(thisExpression, valueExpression);

                var block = Expression.Block(new[]
                {
                                    body,
                                    Expression.Empty ()
                                });

                var lambda = Expression.Lambda(block, FunctionDataParam, eventDataParam);

                var set = lambda.Compile() as Action<object, object>;
                if (set != null)
                    set(FunctionData, eventData);
            }

        }

    }


}
