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
    public class FunctionRunner
    {
        private readonly string _currentRunner;
        private readonly int _state;
        private readonly object _functionClass;
        private readonly EventWait _currentWait;
        private object? _activeRunner;

        public FunctionRunner(
            EventWait currentWait,
            int state,
            object functionClass)
        {
            _currentWait = currentWait;
            _currentRunner = _currentWait.InitiatedByFunction;
            _state = state;
            _functionClass = functionClass;
        }

        public async Task<WaitResult1> Run()
        {
            var functionRunner = GetCurrentRunner();
            SetActiveRunnerState(_state);
            if (functionRunner is null)
                throw new Exception($"Can't initiate runner `{_currentWait.InitiatedByFunction}` for {_currentWait.EventType.FullName}");
            if (await functionRunner.MoveNextAsync())
            {
                var incommingWait = functionRunner.Current;
                var state = GetActiveRunnerState();
                return new WaitResult1(incommingWait, state, false);
            }
            else
            {
                //if current Function runner name is "RunFunction"
                //await _function.OnFunctionEnd();
                var state = GetActiveRunnerState();
                return new WaitResult1(null, state, true);
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

        private IAsyncEnumerator<Wait>? GetCurrentRunner()
        {
            if (_activeRunner == null)
            {
                var FunctionRunnerType = _currentWait.InitiatedByClass
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
                thisField?.SetValue(_activeRunner, _functionClass);

                //set in start state
                SetActiveRunnerState(int.MinValue);
                return _activeRunner as IAsyncEnumerator<Wait>;
            }
            return _activeRunner as IAsyncEnumerator<Wait>;
        }
    }
}
