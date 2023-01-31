using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ResumableFunction.Abstraction.Helpers;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{

    public abstract partial class ResumableFunction<FunctionData>
    {
        protected async Task<OneFunctionWait> Function(string eventIdentifier, Func<IAsyncEnumerable<Wait>> function, [CallerMemberName] string callerName = "")
        {
            var result = new OneFunctionWait(eventIdentifier, function)
            {
                InitiatedByFunctionName = callerName,
                IsNode = true,
            };
            var asyncEnumerator = function().GetAsyncEnumerator();
            await asyncEnumerator.MoveNextAsync();
            var firstWait = asyncEnumerator.Current;
            firstWait.FunctionWaitId = result.Id;
            result.CurrentWait = firstWait;
            result.InitiatedByFunctionName = result.FunctionName;
            return result;
        }

        protected async Task<AllFunctionsWait> Functions
            (string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, [CallerMemberName] string callerName = "")
        {
            return
                  (AllFunctionsWait)await ManyFunctions(eventIdentifier, subFunctions, callerName);
        }



        protected Func<IAsyncEnumerable<Wait>>[] FunctionGroup(params Func<IAsyncEnumerable<Wait>>[] subFunctions) => subFunctions;
        protected async Task<AnyFunctionWait> AnyFunction
            (string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, [CallerMemberName] string callerName = "")
        {
            return
                (AnyFunctionWait)await ManyFunctions(eventIdentifier, subFunctions, callerName);
        }

        private async Task<ManyFunctionsWait> ManyFunctions(string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, string callerName)
        {
            var result = new ManyFunctionsWait
            {
                WaitingFunctions = new OneFunctionWait[subFunctions.Length],
                EventIdentifier = eventIdentifier,
                InitiatedByFunctionName = callerName,
                IsNode = true
            };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(Constants.NoName, currentFunction, callerName);
                currentFuncResult.IsNode = false;
                currentFuncResult.CurrentWait.FunctionWaitId = result.Id;
                currentFuncResult.ParentFunctionGroupId = result.Id;
                result.WaitingFunctions[i] = currentFuncResult;
            }

            return result;
        }
    }

}
