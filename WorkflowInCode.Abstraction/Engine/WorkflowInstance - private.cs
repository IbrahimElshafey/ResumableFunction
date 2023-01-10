using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public abstract partial class WorkflowInstance<ContextData>
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
        private IAsyncEnumerator<WorkflowEvent>? GetActiveRunner()
        {
            if (_activeRunner == null)
            {
                var workflowRunnerType = GetType()
                   .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SuppressChangeType)
                   .FirstOrDefault(x => x.Name.StartsWith("<RunWorkflow>"));

                if (workflowRunnerType == null) return null;
                ConstructorInfo? ctor = workflowRunnerType.GetConstructor(
                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                   new Type[] { typeof(int) });

                if (ctor == null) return null;
                _activeRunner =
                    ctor.Invoke(new object[] { -2 });

                if (_activeRunner == null) return null;
                //set parent class who call
                var thisField = workflowRunnerType.GetFields().FirstOrDefault(x => x.Name.EndsWith("__this"));
                //var thisField = workflowRunnerType.GetField("<>4__this");
                thisField?.SetValue(_activeRunner, this);

                SetActiveRunnerState(int.MinValue);
                return _activeRunner as IAsyncEnumerator<WorkflowEvent>;
            }
            return _activeRunner as IAsyncEnumerator<WorkflowEvent>;
        }

        private void SetContextData(ContextData instanceData, LambdaExpression contextProp, object eventData)
        {
            if (contextProp.Body is MemberExpression me)
            {
                var property = (PropertyInfo)me.Member;

                var instanceDataParam = Expression.Parameter(typeof(object), "instanceData");
                var eventDataParam = Expression.Parameter(typeof(object), "eventData");

                var isValueType = property.PropertyType.IsClass == false && property.PropertyType.IsInterface == false;

                Expression valueExpression;
                if (isValueType)
                    valueExpression = Expression.Unbox(eventDataParam, property.PropertyType);
                else
                    valueExpression = Expression.Convert(eventDataParam, property.PropertyType);

                var thisExpression = Expression.Property(Expression.Convert(instanceDataParam, instanceData.GetType()), property);


                Expression body = Expression.Assign(thisExpression, valueExpression);

                var block = Expression.Block(new[]
                {
                                    body,
                                    Expression.Empty ()
                                });

                var lambda = Expression.Lambda(block, instanceDataParam, eventDataParam);

                var set = lambda.Compile() as Action<object, object>;
                if (set != null)
                    set(instanceData, eventData);
            }

        }

    }


}
