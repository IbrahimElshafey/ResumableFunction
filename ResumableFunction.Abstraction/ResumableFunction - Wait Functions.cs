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

    public abstract partial class ResumableFunctionInstance
    {
        protected async Task<FunctionWait> Function(string eventIdentifier, Func<IAsyncEnumerable<Wait>> function, [CallerMemberName] string callerName = "")
        {
            var result = new FunctionWait(eventIdentifier, function)
            {
                InitiatedByFunctionName = callerName,
                IsNode = true,
            };
            var asyncEnumerator = function().GetAsyncEnumerator();
            await asyncEnumerator.MoveNextAsync();
            var firstWait = asyncEnumerator.Current;
            firstWait.ParentFunctionWaitId = result.Id;
            result.CurrentWait = firstWait;
            result.InitiatedByFunctionName = result.FunctionName;
            SetCommonProps(result);
            return result;
        }

        protected async Task<AllFunctionsWait> Functions
            (string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, [CallerMemberName] string callerName = "")
        {
            return
                  (await ManyFunctions(eventIdentifier, subFunctions, callerName)).ToAllFunctionsWait();
        }



        protected Func<IAsyncEnumerable<Wait>>[] FunctionGroup(params Func<IAsyncEnumerable<Wait>>[] subFunctions) => subFunctions;
        protected async Task<AnyFunctionWait> AnyFunction
            (string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, [CallerMemberName] string callerName = "")
        {
            return
                (await ManyFunctions(eventIdentifier, subFunctions, callerName)).ToAnyFunctionWait();
        }

        private async Task<ManyFunctionsWait> ManyFunctions(string eventIdentifier, Func<IAsyncEnumerable<Wait>>[] subFunctions, string callerName)
        {
            var result = new ManyFunctionsWait
            {
                WaitingFunctions = new List<FunctionWait>(subFunctions.Length),
                EventIdentifier = eventIdentifier,
                InitiatedByFunctionName = callerName,
                IsNode = true
            };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(Constants.NoName, currentFunction, callerName);
                currentFuncResult.IsNode = false;
                currentFuncResult.CurrentWait.ParentFunctionWaitId = result.Id;
                currentFuncResult.ParentFunctionGroupId = result.Id;
                result.WaitingFunctions[i] = currentFuncResult;
            }
            SetCommonProps(result);
            return result;
        }
    }

}
