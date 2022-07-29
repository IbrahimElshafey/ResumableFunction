using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ScenarioFive : WorkflowDefinition<dynamic>
    {
        /*
			* البداية هي دخول موظف جديد للشركة
			* بعد البداية يمكن تنفيذ العمليات
				* رسالة ترحيب بالموظف فيها أفراد إدارته
				* رسالة لمدير الموظف لتحديد مواصفات الجهاز المطلوبة للموظف الجديد
				* ارسال مهمة لقسم الأي تي لتخصيص جهاز مناسب للوظيفة بعد رد المدير بالمواصفات
					* إذا فشل في تخصيص جهاز يرسل طلب لقسم المشتريات لشراء جهاز
					* يرسل طلب للمدير بإمكانيات أفضل جهاز متاح
					* إذا جاءت موافقة المدير يتم الغاء طلب الشراء إذا لم يكن تمت الموافقة عليه
					* إذا جاءت موافقة قسم المشتريات يتم اعلام المدير
				* ارسال مهمة للمدير المباشر للموظف الجديد بتخصيص خطة عمل وتعريف بالشركة
			* بعد البداية يمكن تنفيذ العمليات
				* ارسال مهمة للمتقدم لمرفة تفضيلاته في وجبات الطعام وأوقات العمل
				* ارسال ردود المتقدم لقسم الموارد البشرية للمراجعة
			* بعد انتهاء التتابع الأول والثاني يتم
				* ارسال رسالة للمتقدم بتأكيد موعد أول يوم عمل
				* وبمواصفات الجهاز والبرامج عليه 
				* وخطة المدير لليوم الأول
				* ورأي قسم الموارد في أوقات العمل والوجبات
         */
        public ScenarioFive(IWorkflowEngine workflow, string name = null) : base(workflow, name)
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
                   await new BasicCommand("AskManagerForMachineSpecefications", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.AddEventExpectation(new BasicEvent<dynamic>("ManagerSubmitForMachineSpecefications"));
                   await new BasicCommand("AskManagerForFirstDayPlan", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.AddEventExpectation(new BasicEvent<dynamic>("ManagerSubmitFirstDayPlan"));
                   await new BasicCommand("AskNewEmployeeAboutPreferences", newEmployeeAcceptedEvent.EmployeeId).Execute();
                   await Workflow.AddEventExpectation(new BasicEvent<dynamic>("EmployeePreferencesResponse"));
               });

            Workflow.RegisterStep(
                "Ask Manager For First Day Plan",
                new BasicEvent<dynamic>("ManagerSubmitFirstDayPlan"),
                async (managerSubmitForMachineEvent) =>
                {
                    await Workflow.PushInternalEvent(
                       "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "first day plan");
                }); 
            
            Workflow.RegisterStep(
                "Manager Submit Machine Specefications",
                new BasicEvent<dynamic>("ManagerSubmitForMachineSpecefications"),
                async (managerSubmitForMachineEvent) =>
                {
                    await new BasicCommand("SendTaskForIT_ToPerpareMachine", managerSubmitForMachineEvent).Execute();
                    await Workflow.AddEventExpectation(new BasicEvent<dynamic>("ItResponseToPrepareMachine"));
                });

            Workflow.RegisterStep(
                "IT Response to Prepare a Machine",
                new BasicEvent<dynamic>("ItResponseToPrepareMachine"),
                async (itResponseToPrepareMachine) =>
                {
                    if (itResponseToPrepareMachine.Done)
                    {
                        await Workflow.PushInternalEvent(
                       "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "machine ready");
                    }
                    else if (itResponseToPrepareMachine.NoMachine)
                    {
                        await new BasicCommand("AskSalesToGetNewMachine", itResponseToPrepareMachine).Execute();
                        await Workflow.AddEventExpectation(new BasicEvent<dynamic>("SalesResponseToNewMachine"));
                        await new BasicCommand("AskManagerToAcceptBestMachineAvailable", itResponseToPrepareMachine).Execute();
                        await Workflow.AddEventExpectation(new BasicEvent<dynamic>("ManagerResponseForBestMachineAvailable"));
                    }
                });

            Workflow.RegisterStep(
                "Manager Response for Best Machine Available",
                new BasicEvent<dynamic>("ManagerResponseForBestMachineAvailable"),
                async (managerSubmitEventData) =>
                {
                    if (managerSubmitEventData.Accepted)
                    {
                        await new BasicCommand("SendCancelRequestForSales", managerSubmitEventData).Execute();
                        await Workflow.PushInternalEvent(
                       "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "machine ready");
                    }
                    else if (managerSubmitEventData.Rejected)
                    {
                        await new BasicCommand("InformIt", managerSubmitEventData).Execute();
                    }
                });

            Workflow.RegisterStep(
                "Sales Response to New Machine",
                new BasicEvent<dynamic>("SalesResponseToNewMachine"),
                async (salesResponse) =>
                {
                    if (salesResponse.Accepted)
                    {
                        await new BasicCommand("InformManagerThatSalesAccept", salesResponse).Execute();
                        await Workflow.PushInternalEvent(
                        "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "machine ready");
                    }
                    await new BasicCommand("InformItThatSales", salesResponse).Execute();
                });

            Workflow.RegisterStep(
                "After Employee Preferences Response",
                new BasicEvent<dynamic>("EmployeePreferencesResponse"),
                async (employeePreferencesResponse) =>
                {
                    await new BasicCommand("SendToHrReview", employeePreferencesResponse).Execute();
                    //await Workflow.ExpectReceivingEvent(new BasicEvent<dynamic>("HrEmployeePreferencesReview"));
                    await Workflow.PushInternalEvent(
                        "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "employee response");
                });

            Workflow.RegisterStep(
               "Collect Events EmployeePreferences and MachineReady and ManagerFirstDayPlan",
               new BasicEvent<dynamic>("CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan"),
               async (employeePreferencesResponse) =>
               {
                   await new BasicCommand("SendToHrReview", employeePreferencesResponse).Execute();
                   await Workflow.AddEventExpectation(new BasicEvent<dynamic>("HrEmployeePreferencesReview"));
               });
        }
    }
}
