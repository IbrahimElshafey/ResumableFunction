using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.InOuts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine
{
    public class FunctionRunner<FunctionData>
    {
        private readonly string _currentRunner;
        private readonly int _state;
        private readonly ResumableFunction<FunctionData> _function;
        private readonly SingleEventWaiting _currentWait;
        private object? _activeRunner;

        public FunctionRunner(
            ResumableFunction<FunctionData> resumableFunction,
            SingleEventWaiting currentWait,
            int state)
        {
            _function = resumableFunction;
            _currentWait = currentWait;
            _currentRunner = _currentWait.InitiatedByFunction;
            _state = state;
        }

        public async Task<WaitResult> Run()
        {
            var functionRunner = GetCurrentRunner();
            SetActiveRunnerState(_state);
            if (functionRunner is null)
                throw new Exception($"Can't initiate runner `{_currentWait.InitiatedByFunction}` for {_currentWait.EventType.FullName}");
            if (await functionRunner.MoveNextAsync())
            {
                var incommingWait = functionRunner.Current;
                var state = GetActiveRunnerState();
                return new WaitResult(incommingWait, state, false);
            }
            else
            {
                //if current Function runner name is "RunFunction"
                //await _function.OnFunctionEnd();
                var state = GetActiveRunnerState();
                return new WaitResult(null, state, true);
            }
        }



        private void SetActiveRunnerState(int state)
        {
            if (_activeRunner != null || GetCurrentRunner() != null)
            {
                var stateField = _activeRunner?.GetType().GetField("<>1__state");
                if (stateField != null)
                {

                    stateField.SetValue(_activeRunner, state);
                }
            }
        }
        private int GetActiveRunnerState()
        {
            if (_activeRunner != null || GetCurrentRunner() != null)
            {
                var stateField = _activeRunner?.GetType().GetField("<>1__state");
                if (stateField != null)
                {
                    return (int)stateField.GetValue(_activeRunner);
                }
            }
            return int.MinValue;
        }

        private IAsyncEnumerator<EventWaitingResult>? GetCurrentRunner()
        {
            if (_activeRunner == null)
            {
                var FunctionRunnerType = GetType()
                   .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SuppressChangeType)
                   .FirstOrDefault(x => x.Name.StartsWith($"<{_currentRunner}>"));

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

                //set in start state
                SetActiveRunnerState(int.MinValue);
                return _activeRunner as IAsyncEnumerator<EventWaitingResult>;
            }
            return _activeRunner as IAsyncEnumerator<EventWaitingResult>;
        }

        /// <summary>
        /// will be called by the engine after event received
        /// </summary>
        public void SetFunctionData(object data)
        {
            //todo:check type && me.Type
            var contextProp = _currentWait.SetPropExpression;
            if (contextProp.Body is MemberExpression me)
            {
                var property = (PropertyInfo)me.Member;

                var FunctionDataParam = Expression.Parameter(typeof(object), "functionData");
                var eventDataParam = Expression.Parameter(typeof(object), "eventData");

                var isValueType = property.PropertyType.IsClass == false && property.PropertyType.IsInterface == false;

                Expression valueExpression;
                if (isValueType)
                    valueExpression = Expression.Unbox(eventDataParam, property.PropertyType);
                else
                    valueExpression = Expression.Convert(eventDataParam, property.PropertyType);

                var thisExpression = Expression.Property(Expression.Convert(FunctionDataParam, typeof(FunctionData)), property);


                Expression body = Expression.Assign(thisExpression, valueExpression);

                var block = Expression.Block(new[]
                {
                                    body,
                                    Expression.Empty ()
                                });

                var lambda = Expression.Lambda(block, FunctionDataParam, eventDataParam);

                var set = lambda.Compile() as Action<object, object>;
                if (set != null && _function.Data != null)
                    set(_function.Data, data);
            }

        }
    }
}
