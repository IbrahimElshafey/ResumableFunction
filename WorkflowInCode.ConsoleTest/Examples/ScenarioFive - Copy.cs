using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ScenarioFive2 : WorkflowDefinition<dynamic>
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
				* ارسال مهمة للمتقدم لمرفة تفضيلاته في وجبات الطعام وأوقات العمل
				    * ارسال ردود المتقدم لقسم الموارد البشرية للمراجعة
			* بعد انتهاء التتابع السابق
				* ارسال رسالة للمتقدم بتأكيد موعد أول يوم عمل
				* وبمواصفات الجهاز والبرامج عليه 
				* وخطة المدير لليوم الأول
				* ورأي قسم الموارد في أوقات العمل والوجبات
         */

        private async Task Test()
        {
            var userAddedTask = await WaitEvent("NewUserAdded");
            var taskResults = await WaitTasks("WelcomeNewJoinEmployee", "MachineAssigment", "FirstDayPlan", "EmployeePreferences");
            var finalTask = await StartTask("SendEmployeeMessage");

            var managerMachineResposne = await StartTask("AskManagerForMachine");
            var itMachineResponse = await StartTask("SendMachineSpecsToIt");
            if (itMachineResponse.NoMachineExist)
            { 
                var salesNewMachineResponse = StartTask("AskSalesForNewMachine");
                var askManagerForBestExist =  StartTask("AskManagerForBestExistMachine");
                await Task.WhenAny(salesNewMachineResponse, askManagerForBestExist);
            }
            else if (itMachineResponse.MachineExist)
            {
                await new BasicCommand("SendToManagerMachineReady", "").Execute();
            }
        }

        private Task<dynamic> StartTask(string v)
        {
            throw new NotImplementedException();
        }

        private async Task<dynamic> WaitEvent(string v)
        {
            throw new NotImplementedException();
        }
        private async Task<dynamic[]> WaitTasks(params string[] tasks)
        {
            throw new NotImplementedException();
        }

        public ScenarioFive2(IWorkflowEngine workflow, string name = null) : base(workflow, name)
        {

            
        }

        

        public override void RegisterSteps()
        {
            Workflow.Step(
                stepName: "NewEmployeeAccepted",
                stepStartWhen: new BasicEvent<dynamic>("NewEmployeeAccepted"),
                afterStepDone: async (newEmployeeAcceptedEvent) =>
                {
                    await new BasicCommand("SendWelcomeOnBoardEmail", newEmployeeAcceptedEvent.EmployeeId).Execute();

                    Workflow.Step(
                        stepName: "Manager Submit Machine Specefications",
                        intiateStep: new BasicCommand("AskManagerForMachineSpecefications", newEmployeeAcceptedEvent.EmployeeId),
                        stepStartWhen: new BasicEvent<dynamic>("ManagerSubmitForMachineSpecefications"),
                        afterStepDone: async (managerSubmitForMachineEvent) =>
                        {
                            Workflow.ExpectNextStep(
                                stepName: "Ask IT to Prepare a Machine",
                                intiateStep: new BasicCommand("SendTaskForIT_ToPerpareMachine", managerSubmitForMachineEvent).Execute(),
                                stepStartWhen: new BasicEvent<dynamic>("ItResponseToPrepareMachine"),
                                afterStepDone: async (itResponseToPrepareMachine) =>
                                {
                                    if (itResponseToPrepareMachine.Done)
                                    {
                                        CurrentInstance.ContextData.IsMachineReady = true;
                                    }
                                    else if (itResponseToPrepareMachine.NoMachine)
                                    {
                                        await new BasicCommand("AskSalesToGetNewMachine", itResponseToPrepareMachine).Execute();
                                        await Workflow.AddEventExpectation(new BasicEvent<dynamic>("SalesResponseToNewMachine"));
                                        await new BasicCommand("AskManagerToAcceptBestMachineAvailable", itResponseToPrepareMachine).Execute();
                                        await Workflow.AddEventExpectation(new BasicEvent<dynamic>("ManagerResponseForBestMachineAvailable"));
                                    }
                                });
                        }
                        );

                    Workflow.Step(
                       stepName: "Ask Manager For First Day Plan",
                       intiateStep: new BasicCommand("AskManagerForFirstDayPlan", newEmployeeAcceptedEvent.EmployeeId),
                       stepStartWhen: new BasicEvent<dynamic>("ManagerSubmitFirstDayPlan"),
                       afterStepDone: async (managerSubmitForMachineEvent) =>
                       {
                           await Workflow.PushInternalEvent(
                        "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "first day plan");
                       }
                       );

                    Workflow.Step(
                       stepName: "Ask Employee about Preferences",
                       intiateStep: new BasicCommand("AskNewEmployeeAboutPreferences", newEmployeeAcceptedEvent.EmployeeId),
                       stepStartWhen: new BasicEvent<dynamic>("EmployeePreferencesResponse"),
                       afterStepDone: async (managerSubmitForMachineEvent) =>
                       {
                           await new BasicCommand("SendToHrReview", employeePreferencesResponse).Execute();
                           //await Workflow.ExpectReceivingEvent(new BasicEvent<dynamic>("HrEmployeePreferencesReview"));
                           await Workflow.PushInternalEvent(
                               "CollectEvents_EmployeePreferences_MachineReady_ManagerFirstDayPlan", "employee response");
                       }
                       );
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

    

            
        }
    }
}
