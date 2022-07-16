using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.Examples
{
    internal class ProjectApprovalWorkflowSingleMethodBased
    {
        private const string SendToPmo = "";
        private const string SendToOwner = "";
        private const string SendToSponsor = "";
        private const string ProjectAccepted = "";
        private const string ResendToProjectManager = "";
        private const string ProjectManagerCancelProject = "";

        //Command: is when we ask system/service to do something
        //Event: is when system/person/service finish doing something
        //Context: is data we need to pass/save/keep 

        //The action is what executed when command requested by the workflow
        //After / while the action executed it throw one or more Events 
        //the workflow subscribe to these events and send another commands based on the workflow 

        //in workflow we receive/subscribe to events and not sending/raising it
        //we initiate/execute/start the commands
        //we use queries
        //[WorkFlow]
        public async Task ProjectAcceptanceWorkflow()
        {
            //wait for event ProjectAdded that raised when user add a project
            var projectInfo = await ReceivingEvent(WorkflowEvents.ProjectAdded);
            //wait for event project pact added
            var projectPact = await ReceivingEvent(WorkflowEvents.ProjectPactAdded, projectInfo.Id);

            //Request system/service to do an action
            await Command(SendToPmo, projectInfo.Id, SendToPmo);
            var pmoResponse = await ReceivingEvent(WorkflowEvents.PMO_Approval, projectInfo.Id);
            if (pmoResponse.Accepted)
                await Command(SendToOwner, projectInfo.Id);
            else if (pmoResponse.Rejected)
                await SubStep_ResendToProjectManager(projectInfo.Id);

            var ownerResponse = await ReceivingEvent(WorkflowEvents.Owner_Approval, projectInfo.Id);
            if (ownerResponse.Accepted)
                await Command(SendToSponsor, projectInfo.Id);
            else if (ownerResponse.Rejected)
                await SubStep_ResendToProjectManager(projectInfo.Id);

            var sponsorResponse = await ReceivingEvent(WorkflowEvents.Sponsor_Approval, projectInfo.Id);
            if (sponsorResponse.Accepted)
            {
                await Command(ProjectAccepted, projectInfo.Id);
                await EndWorkflow();
            }
            else if (sponsorResponse.Rejected)
                await SubStep_ResendToProjectManager(projectInfo.Id);
        }


        //[WorkFlowSubStep]
        private async Task SubStep_ResendToProjectManager(object projectId)
        {
            await Command(ResendToProjectManager, projectId);
            var projectManagerResponse = await ReceivingEvent(WorkflowEvents.ProjectManager_Approval, projectId);
            if (projectManagerResponse.Cancel)
            {
                await Command(ProjectManagerCancelProject, projectId);
                await EndWorkflow();
            }
            else if (projectManagerResponse.Resend)
            {
                //back to specific postion in workflow
                await BackToWorkflowStep(SendToPmo);
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
