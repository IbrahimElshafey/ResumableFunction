using System.Formats.Asn1;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using WorkflowInCode.Abstraction.Engine;
using WorkflowInCode.Abstraction.Samples;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var po = new ProjectApproval(
                new ProjectRequestedEvent() { EventData = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" } },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) },
                new ManagerApprovalEvent() { EventData = new ProjectApprovalResult(122, true, false) });

            //event come to engine
            //engine search subscribtion table to find if any workflow use this type
            //engine load related query for matched workflows
            //Translate matching function to database query
            //If event is first then add workflow instance to the database
            //if not first then search the database for instance/s that match query and load workflow data context

            var workflowRunnerType = po.GetType()
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SuppressChangeType)
                .FirstOrDefault(x => x.Name.StartsWith("<RunWorkflow>"));
            if (workflowRunnerType != null)
            {
                po.InstanceData.Project = new Project { DueDate = DateTime.Now, Id = 122, Name = "Project1" };
                po.InstanceData.SponsorApprovalResult = new ProjectApprovalResult(122, true, false);
                po.InstanceData.OwnerApprovalResult = new ProjectApprovalResult(122, true, false);
                po.InstanceData.ManagerApprovalResult = new ProjectApprovalResult(122, true, false);
                ConstructorInfo? ctor = workflowRunnerType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                    new Type[] { typeof(int) });
                if (ctor != null)
                {
                    IAsyncEnumerator<ISubscribedEvent>? workflowRunner = 
                        ctor.Invoke(new object[] { -2 }) as IAsyncEnumerator<ISubscribedEvent>;

                    if (workflowRunner != null)
                    {
                        //set parent class who call
                        var thisField = workflowRunnerType.GetField("<>4__this");
                        thisField?.SetValue(workflowRunner, po);

                        var stateField = workflowRunnerType.GetField("<>1__state");
                        //set state field from last session that was saved to the database
                        stateField?.SetValue(workflowRunner, -7);

                        try
                        {
                            if (await workflowRunner.MoveNextAsync())
                            {
                                var incomingEvent = workflowRunner.Current;
                                var value = stateField?.GetValue(workflowRunner);
                                //if (incomingEvent.MatchFunction != null)
                                //{
                                //    var matchingFunction = incomingEvent.MatchFunction.Compile() as Func<object, object, bool>;
                                //    if (matchingFunction != null && matchingFunction(incomingEvent.EventData, po.InstanceData))
                                //    {

                                //    }
                                //}
                                SetContextData(po.InstanceData, incomingEvent.ContextProp, incomingEvent.EventData);
                            }
                            else
                            {
                                //workflow ended
                            }
                            
                        }
                        finally { if (workflowRunner != null) await workflowRunner.DisposeAsync(); }
                    }
                   
                    //var moveNextMethod = workflowRunnerType.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
                    //if (moveNextMethod != null)
                    //{
                    //    moveNextMethod.Invoke(workflowRunner, null);
                    //    var currentField = workflowRunnerType.GetProperty("<>2__current", BindingFlags.NonPublic|BindingFlags.Instance);
                    //     var currentEvent = currentField?.GetValue(workflowRunner);
                    //}
                }

            }
            
        }

        ///To use Expression trees <see cref="TestSomething.PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))
        private static void SetContextData(ProjectApprovalContextData instanceData, LambdaExpression contextProp, object eventData)
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
        private static void SetContextData(ProjectApprovalContextData instanceData, string contextProp, object eventData)
        {

            var piInstance = instanceData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(instanceData, eventData);
            //save data to database
        }
    }
}
