using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{

    public class ProjectDiscussionWorkflow : WorkflowInstance<ProjectDiscussionWorkflow_ContextData>
    {
        public ProjectDiscussionWorkflow(IWorkflow workflow) : base(workflow)
        {
            workflow.RegisterStep(
                stepEvent: new BasicEvent<dynamic>("DiscussionStartedOnRequest"),
                stepAction: DiscussionStartedStep);

            workflow.RegisterStep(
               stepEvent: new BasicEvent<dynamic>("MemberActionDoneOnTopic"),
               stepAction: MemberTopicApprovalStep,
               eventFilter: (eventData) =>
                ContextData.RequestBasicData.RequestId == eventData.RequestId &&
                ContextData.CurrentTopic.TopicId == eventData.TopicId);

            workflow.RegisterStep(
              stepEvent: new BasicEvent<dynamic>("ChefSkipsMembersDecisions"),
              stepAction: ChefSkipsMembersDecisions,
              eventFilter: (eventData) =>
               ContextData.RequestBasicData.RequestId == eventData.RequestId &&
               ContextData.CurrentTopic.TopicId == eventData.TopicId);

            workflow.RegisterStep(
              stepEvent: new BasicEvent<dynamic>("ChefActionDoneOnTopic"),
              stepAction: ChefTopicApprovalStep,
              eventFilter: (eventData) =>
               ContextData.RequestBasicData.RequestId == eventData.RequestId &&
               ContextData.CurrentTopic.TopicId == eventData.TopicId);

            workflow.RegisterStep(
             stepEvent: new BasicEvent<dynamic>("ChefFinalActionDoneOnRequest"),
             stepAction: ChefFinalApprovalStep,
             eventFilter: (eventData) =>
              ContextData.RequestBasicData.RequestId == eventData.RequestId);
        }


        private async Task DiscussionStartedStep(dynamic requestBasicData)
        {
            ContextData.RequestBasicData = requestBasicData;
            await SaveState();//save state here if the next queries failes

            var members = new BasicQuery<dynamic>("GetDiscussionMembers", requestBasicData.RequestId).Execute();
            var topics = new BasicQuery<dynamic>("GetDiscussionTopics", requestBasicData.RequestId).Execute();
            await Task.WhenAll(members, topics);
            ContextData.Members = members.Result;
            ContextData.Topics = topics.Result;
            ContextData.CurrentTopic = ContextData.Topics.First();
            await SaveState();

            //send discussion invitation foreach member
            await SendTopicInvitationsToMembers("Start first topic discussion");
            //send invitation for the chef
            await ChefCanSkipTopic("Chef can skip topic one");
        }

        private async Task MemberTopicApprovalStep(dynamic memperApproval)
        {
            ContextData.MemberApprovals.Add(memperApproval);
            await SaveState();
            if (memperApproval.Accepted)
            {
                await new BasicCommand("MemberAcceptedTopic",
                    new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            else if (memperApproval.Rejected)
            {
                await new BasicCommand("MemberRejectedTopic",
                    new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            else if (memperApproval.RequestDataComplete)
            {
                await new BasicCommand("MemberRequestDataCompleteForTopic",
                  new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            ContextData.CurrentTopicVotedMembersCount += 1;
            await SaveState();
            var allMembersVoted = ContextData.CurrentTopicVotedMembersCount < ContextData.Members.Count;
            if (allMembersVoted)
            {
                await new BasicCommand("SendTopicForChefApproval",
                    new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
                await Workflow.ExpectNextStep(ChefTopicApprovalStep,"After all members voted on a topic");
            }
            else
            {
                await Workflow.ExpectNextStep(MemberTopicApprovalStep,"Next member approval for current topic");
                await Workflow.ExpectNextStep(ChefSkipsMembersDecisions,"Chef can skip topic discussion");
            }
        }

        private async Task ChefSkipsMembersDecisions(dynamic chefResponse)
        {
            ContextData.ChefApprovals.Add(chefResponse);
            await SaveState();
            if (chefResponse.SkipMembers)
            {
                await new BasicCommand("SendTopicForChefApproval", ContextData.CurrentTopic.Id).Initiate();
                await Workflow.ExpectNextStep(ChefTopicApprovalStep,"After chef skip he will approve");
            }
        }
        private async Task ChefTopicApprovalStep(dynamic chefResponse)
        {
            ContextData.ChefApprovals.Add(chefResponse);
            await SaveState();
            if (chefResponse.Rejected)
            {
                await new BasicCommand("ChefRejectedTopic", ContextData.CurrentTopic.TopicId).Initiate();
            }

            else if (chefResponse.Accepted)
            {
                await new BasicCommand("ChefAcceptedTopic", ContextData.CurrentTopic.TopicId).Initiate();
            }
            var isLastTopic = ContextData.CurrentTopic == ContextData.Topics.Last();
            await (isLastTopic ? DiscussNextTopic() : TriggerFinalApproval());
        }

        private async Task ChefFinalApprovalStep(dynamic chefResponse)
        {
            ContextData.ChefFinalApproval = chefResponse;
            await SaveState();
            if (chefResponse.Accepted)
            {
                await new BasicCommand("ChefAcceptedRequest", ContextData.RequestBasicData.RequestId).Initiate();
            }
            else if (chefResponse.Rejected)
            {
                await new BasicCommand("ChefRejectedRequest", ContextData.RequestBasicData.RequestId).Initiate();
            }
            else if (chefResponse.CompleteData)
            {
                await new BasicCommand("ChefRequestDataComplete", ContextData.RequestBasicData.RequestId).Initiate();
            }
            await Workflow.End();
        }

        private async Task ChefCanSkipTopic(string arrowName)
        {
            await new BasicCommand("ChefCanSkipTopic",
                            new { ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            await Workflow.ExpectNextStep(ChefSkipsMembersDecisions, arrowName);
        }

        private async Task SendTopicInvitationsToMembers(string arrowName)
        {
            await new BasicCommand("SendDiscussionRequestToMembers",
                    new { ContextData.Members, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            await Workflow.ExpectNextStep(MemberTopicApprovalStep,arrowName);
        }

        private async Task DiscussNextTopic()
        {
            var currentTopicIndex = ContextData.CurrentTopic.TopicIndex;
            ContextData.CurrentTopic = ContextData.Topics[currentTopicIndex + 1];
            ContextData.CurrentTopicVotedMembersCount = 0;
            await SaveState();
            await SendTopicInvitationsToMembers("After chef approval send discus next topic");
            await ChefCanSkipTopic("Skip next topic discussion");
        }

        private async Task TriggerFinalApproval()
        {
            await new BasicCommand("TopicsDiscussionFinished", ContextData.CurrentTopic.Id).Initiate();
            await Workflow.ExpectNextStep(ChefFinalApprovalStep,"Chef final approval after last topic");
        }



    }
}
