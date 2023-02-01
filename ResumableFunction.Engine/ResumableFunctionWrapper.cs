using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.Helpers;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.InOuts;
using System.Linq.Expressions;
using System.Reflection;

namespace ResumableFunction.Engine
{
    public class ResumableFunctionWrapper : IResumableFunction
    {
        private Wait _currentWait;
        private object? _activeRunner;
        public ResumableFunctionWrapper(Wait eventWait)
        {
            _currentWait = eventWait;
            var functionClassType = _currentWait.FunctionRuntimeInfo.InitiatedByClassType;
            var isResumableFunctionClass = functionClassType.IsSubclassOf(typeof(ResumableFunctionInstance));
            if (isResumableFunctionClass is false)
                throw new Exception("functionClassType must inherit ResumableFunction<>");

            FunctionClassInstance = (ResumableFunctionInstance)Activator.CreateInstance(functionClassType);
            if (FunctionClassInstance is null)
                throw new Exception($"Can't create instance of `{functionClassType}` with .ctor()");

            FunctionClassInstance.FunctionRuntimeInfo = eventWait.FunctionRuntimeInfo;
        }

        public async Task<NextWaitResult> BackToCaller(Wait functionWait)
        {
            _currentWait = functionWait;
            _activeRunner = null;
            return await GetNextWait();
        }



        public ResumableFunctionInstance FunctionClassInstance { get; internal set; }

        public FunctionRuntimeInfo FunctionRuntimeInfo
        {
            get => FunctionClassInstance.FunctionRuntimeInfo;
            private set => FunctionClassInstance.FunctionRuntimeInfo = value;
        }

        public Task OnFunctionEnd()
        {
            return FunctionClassInstance.OnFunctionEnd();
        }

        /// <summary>
        /// called by the engine to resume function execution
        /// </summary>
        public async Task<NextWaitResult> GetNextWait()
        {
            var functionRunner = GetCurrentRunner();

            if (functionRunner is null)
                throw new Exception(
                    $"Can't initiate runner `{_currentWait.InitiatedByFunctionName}` for {_currentWait.FunctionRuntimeInfo.InitiatedByClassType.FullName}");
            SetActiveRunnerState(_currentWait.StateAfterWait);
            bool waitExist = await functionRunner.MoveNextAsync();
            //after function resumed data may be changed (for example user set some props)
            //FunctionRuntimeInfo.Data = Data;
            //update current runner state
            //if (FunctionRuntimeInfo.FunctionsStates.ContainsKey(_currentWait.InitiatedByFunction))
            //    FunctionRuntimeInfo.FunctionsStates[_currentWait.InitiatedByFunction] = GetActiveRunnerState();
            //else
            //    FunctionRuntimeInfo.FunctionsStates.Add(_currentWait.InitiatedByFunction, GetActiveRunnerState());

            if (waitExist)
            {
                functionRunner.Current.StateAfterWait = GetActiveRunnerState();
                return new NextWaitResult(functionRunner.Current, false, false);
            }
            else
            {
                //if current Function runner name is the main function start
                if (_currentWait.InitiatedByFunctionName == nameof(Start))
                {
                    await OnFunctionEnd();
                    return new NextWaitResult(null, true, false);
                }
                return new NextWaitResult(null, false, true);
            }
        }



        internal void SetActiveRunnerState(int state)
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
        internal int GetActiveRunnerState()
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
                var functionRunnerType = _currentWait.FunctionRuntimeInfo.InitiatedByClassType
                   .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SuppressChangeType)
                   .FirstOrDefault(x => x.Name.StartsWith(CurrentRunnerName()));

                if (functionRunnerType == null) return null;
                ConstructorInfo? ctor = functionRunnerType.GetConstructor(
                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                   new Type[] { typeof(int) });

                if (ctor == null) return null;
                _activeRunner = ctor.Invoke(new object[] { -2 });

                if (_activeRunner == null) return null;
                //set parent class who call
                var thisField = functionRunnerType.GetFields().FirstOrDefault(x => x.Name.EndsWith("__this"));
                //var thisField = FunctionRunnerType.GetField("<>4__this");
                thisField?.SetValue(_activeRunner, FunctionClassInstance);
                //var xx=thisField?.GetValue(_activeRunner);

                //set in start state
                SetActiveRunnerState(int.MinValue);
                return _activeRunner as IAsyncEnumerator<Wait>;
            }
            return _activeRunner as IAsyncEnumerator<Wait>;


        }

        private string CurrentRunnerName()
        {
            return $"<{_currentWait.InitiatedByFunctionName}>";
        }

        //private int CurrentRunnerLastState()
        //{
        //    int result = int.MinValue;
        //    FunctionRuntimeInfo?.FunctionsStates.TryGetValue(_currentWait.InitiatedByFunction, out result);
        //    return result;
        //}

        public IAsyncEnumerable<Wait> Start()
        {
            return FunctionClassInstance.Start();
        }
    }
}
