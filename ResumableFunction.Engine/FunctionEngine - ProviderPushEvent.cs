using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumableFunction.Engine
{
    public partial class FunctionEngine
    {

        public async Task WhenProviderPushEvent(PushedEvent pushedEvent)
        {
            //engine search waits list with(ProviderName, EventType)
            var matchedWaits = await _waitsRepository.GetMatchedWaits(pushedEvent);
            foreach (var wait in matchedWaits)
            {
                var functionClass = new ResumableFunctionWrapper(wait);
                functionClass.UpdateDataWithEventData();
                //get next event wait
                var waitResult = await functionClass.GetNextWait();
                await _functionRepository.SaveFunctionState(wait.ParentFunctionState);
                await WaitRequested(waitResult, functionClass);
                //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
            }
        }


    }
}
