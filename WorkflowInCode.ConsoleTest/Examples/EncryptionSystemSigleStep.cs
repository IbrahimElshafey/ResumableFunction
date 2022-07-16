using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.Examples
{
    public class EncryptionSystemSigleStep
    {
        public async Task EncryptionSystemAcceptanceWorkflow()
        {
            var encryptionSystemData = await ReceivingEvent("NewEncryptionSystemAdded");
            var whoAddedTheProject = await Query<dynamic>("WhoAddedTheProject", encryptionSystemData);
            if (whoAddedTheProject.Secretary)
            {
                await Command("NewRequestReadyForDisscusion", encryptionSystemData);
            }
            else if (whoAddedTheProject.Member)
            {
                await Command("SendNewEncryptionSystemToSecretaryReview", encryptionSystemData);
                var secretaryResponse = await ReceivingEvent("SecrataryApproval", encryptionSystemData.SystemId);
                if (secretaryResponse.Accepted)
                {
                    await Command("NewRequestReadyForDisscusion", encryptionSystemData);
                }
                else if (secretaryResponse.CompleteData)
                {
                    await Command("SecrataryRequestCompleteData", encryptionSystemData.SystemId);
                    var userResponse =await ReceivingEvent("UserCompletedDataResponse");
                    if(userResponse.Cancled)
                    {
                        await Command("UserCancledRequest", encryptionSystemData.SystemId);
                        await EndWorkflow();
                    }
                    else if(userResponse.RequestReview)
                    {
                        await Command("SendNewEncryptionSystemToSecretaryReview", encryptionSystemData);
                    }
                    else if (userResponse.RequestChefAction)
                    {
                        await Command("ChefActionRequiredOnRequest", encryptionSystemData);
                    }
                }
                else if (secretaryResponse.Rejected)
                {
                    await Command("SecrataryRejectedRequest", encryptionSystemData.SystemId);
                    await EndWorkflow();
                }
            }
        }
        private Task EndWorkflow()
        {
            //throw new NotImplementedException();
            return null;
        }

        private async Task BackToWorkflowStep(string step)
        {
            //throw new NotImplementedException();
        }
        private async Task<Data> Query<Data>(string name,string parameters)
        {
            return default;
        } 
        private async Task Command(string commandName, dynamic commandData, string stepName = "")
        {
            //throw new NotImplementedException();
        }
        private async Task<dynamic> ReceivingEvent(string eventName, dynamic? eventIdentifier = null)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
