using ResumableFunction.Abstraction;

namespace ResumableFunction.Engine
{
    public class FunctionWrapper
    {
        public FunctionWrapper(Type functionClassType)
        {
            FunctionClassType = functionClassType;
            var isResumableFunctionClass = functionClassType.IsSubclassOfRawGeneric(typeof(ResumableFunction<>));
            if (isResumableFunctionClass is false)
                throw new Exception("functionClassType must inherit ResumableFunction<>");

            dynamic instance = Activator.CreateInstance(functionClassType);
            if (instance is null) return;

            InstanceId = instance.InstanceId;
            if (instance.Data == null)
            {
                var propType = functionClassType.GetProperty("Data").PropertyType;
                Data = Activator.CreateInstance(propType);
            }
            Data = instance.Data;
        }
        public Guid InstanceId { get; private set; }
        public dynamic Data { get; private set; }
        public Type FunctionClassType { get; private set; }
    }
}
