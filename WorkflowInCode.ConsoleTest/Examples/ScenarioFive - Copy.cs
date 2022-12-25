using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    //internal class ScenarioLoop : WorkflowDefinition<dynamic>
    //{
    //    public override void RegisterSteps()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    internal class ScenarioSix : WorkflowDefinition<dynamic>
    {
        private const string FirstDayPlan = "Ask Manager For First Day Plan";
        private const string ManagerMachineSpecefications = "Manager Submit Machine Specefications";
        private const string IT_PrepareMahine = "IT Response to Prepare a Machine";
        private const string ManagerSelectBestMachine = "Manager Response for Best Machine Available";
        private const string SalesResonse = "Sales Response to New Machine";
        private const string EmployeePreferences = "After Employee Preferences Response";

        /*
        * البداية هي دخول موظف جديد للشركة
        * بعد البداية يمكن تنفيذ العمليات
            * رسالة ترحيب بالموظف فيها أفراد إدارته
            * رسالة لمدير الموظف لتحديد مواصفات الجهاز المطلوبة للموظف الجديد
                * ارسال مهمة لقسم الأي تي لتخصيص جهاز مناسب للوظيفة بعد رد المدير بالمواصفات
                    * إذا فشل في تخصيص جهاز يرسل طلب لقسم المشتريات لشراء جهاز
                    * يرسل طلب للمدير بإمكانيات أفضل جهاز متاح أو بتخصيص الجهاز
                        * إذا جاءت موافقة المدير يتم الغاء طلب الشراء إذا لم يكن تمت الموافقة عليه
                        * إذا جاءت موافقة قسم المشتريات يتم اعلام المدير وقسم الأي تي
            * ارسال مهمة للمدير المباشر للموظف الجديد بتخصيص خطة عمل وتعريف بالشركة
            * ارسال مهمة للمتقدم لمرفة تفضيلاته في وجبات الطعام وأوقات العمل
                * ارسال ردود المتقدم لقسم الموارد البشرية للمراجعة
        * بعد انتهاء السابق
            * ارسال رسالة للمتقدم بتأكيد موعد أول يوم عمل
            * وبمواصفات الجهاز والبرامج عليه 
            * وخطة المدير لليوم الأول
            * ورأي قسم الموارد في أوقات العمل والوجبات
        */
        public ScenarioSix(IWorkflowEngine workflow, string name = null) : base(workflow, name)
        {
            

        }
        private class Steps
        {

        }
        public override void RegisterSteps()
        {
            Workflow.RegisterStep(
                "NewEmployeeAccepted",
                new BasicEvent<dynamic>("NewEmployeeAccepted"),
               async (newEmployeeAcceptedEvent) =>
               {
                   await new BasicCommand("SendWelcomeOnBoardEmail", newEmployeeAcceptedEvent.EmployeeId).Execute();

                   //await Workflow.UserTask(
                   //    taskName: ManagerMachineSpecefications,
                   //    initiationCommand: new BasicCommand("AskManagerForMachineSpecefications", newEmployeeAcceptedEvent.EmployeeId),
                   //    userActionEvent: new BasicEvent<dynamic>("ManagerSubmitForMachineSpecefications"),
                   //    afterUserAction:async(eventdata)=>await Workflow.UserTask()
                   //    );
                   await new BasicCommand("AskManagerForMachineSpecefications", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.ExpectNextStep(ManagerMachineSpecefications);

                   await new BasicCommand("AskManagerForFirstDayPlan", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.ExpectNextStep(FirstDayPlan);

                   await new BasicCommand("AskNewEmployeeAboutPreferences", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.ExpectNextStep(EmployeePreferences);
               });

            Workflow.RegisterStep(
                FirstDayPlan,
                new BasicEvent<dynamic>("ManagerSubmitFirstDayPlan"),
                async (managerSubmitForMachineEvent) =>
                {
                    CurrentInstance.ContextData.IsFirstDayPlanReady = true;
                    await Collect();
                }); 
            
            Workflow.RegisterStep(
                ManagerMachineSpecefications,
                new BasicEvent<dynamic>("ManagerSubmitForMachineSpecefications"),
                async (managerSubmitForMachineEvent) =>
                {
                    await new BasicCommand("SendTaskForIT_ToPerpareMachine", managerSubmitForMachineEvent).Execute();
                    await Workflow.ExpectNextStep(IT_PrepareMahine);
                });

            Workflow.RegisterStep(
                IT_PrepareMahine,
                new BasicEvent<dynamic>("ItResponseToPrepareMachine"),
                async (itResponseToPrepareMachine) =>
                {
                    if (itResponseToPrepareMachine.Done)
                    {
                        CurrentInstance.ContextData.IsMachineReady = true;
                        await Collect();
                    }
                    else if (itResponseToPrepareMachine.NoMachine)
                    {
                        await new BasicCommand("AskSalesToGetNewMachine", itResponseToPrepareMachine).Execute();
                        await Workflow.ExpectNextStep(SalesResonse);
                        await new BasicCommand("AskManagerToAcceptBestMachineAvailable", itResponseToPrepareMachine).Execute();
                        await Workflow.ExpectNextStep(ManagerSelectBestMachine);
                    }
                });

            Workflow.RegisterStep(
                ManagerSelectBestMachine,
                new BasicEvent<dynamic>("ManagerResponseForBestMachineAvailable"),
                async (managerSubmitEventData) =>
                {
                    if (managerSubmitEventData.Accepted)
                    {
                        await new BasicCommand("SendCancelRequestForSales", managerSubmitEventData).Execute();
                        CurrentInstance.ContextData.IsMachineReady = true;
                        await Collect();
                    }
                    else if (managerSubmitEventData.Rejected)
                    {
                        await new BasicCommand("InformIt", managerSubmitEventData).Execute();
                    }
                });

            Workflow.RegisterStep(
                SalesResonse,
                new BasicEvent<dynamic>("SalesResponseToNewMachine"),
                async (salesResponse) =>
                {
                    if (salesResponse.Accepted)
                    {
                        await new BasicCommand("InformManagerThatSalesAccept", salesResponse).Execute();
                        CurrentInstance.ContextData.IsMachineReady = true;
                        await Collect();
                    }
                    await new BasicCommand("InformItThatSales", salesResponse).Execute();
                });

            Workflow.RegisterStep(
                EmployeePreferences,
                new BasicEvent<dynamic>("EmployeePreferencesResponse"),
                async (employeePreferencesResponse) =>
                {
                    await new BasicCommand("SendToHrReview", employeePreferencesResponse).Execute();
                    //await Workflow.ExpectReceivingEvent(new BasicEvent<dynamic>("HrEmployeePreferencesReview"));//trigger external workflow
                    CurrentInstance.ContextData.IsEmployeePreferencesResponseExist = true;
                    await Collect();
                });

           async Task Collect()
            {
                
                if (CurrentInstance.ContextData.IsFirstDayPlanReady &&
                    CurrentInstance.ContextData.IsMachineReady &&
                    CurrentInstance.ContextData.IsEmployeePreferencesResponseExist)
                {
                    await new BasicCommand("InformNewEmployeeAboutEveryThing", CurrentInstance.ContextData).Execute();
                    await Workflow.End();
                }
            }
        }
    }
}
