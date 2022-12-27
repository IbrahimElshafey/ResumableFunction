using System.Threading.Tasks;
using static WorkflowInCode.Abstraction.Engine.WorkflowEngine;
namespace WorkflowInCode.Abstraction.Engine
{
    public class test
    {
        private readonly IFakeProcess Manager1Approval;
        private readonly IFakeProcess Manager2Approval;
        private readonly IFakeProcess Manager3Approval;
        private readonly IFakeProcess2 Applicant;

        public test(IFakeProcess p1, IFakeProcess p2, IFakeProcess p3, IFakeProcess2 ap)
        {
            Manager1Approval = p1;
            Manager2Approval = p2;
            Manager3Approval = p3;
            Applicant = ap;
            ////Sequence

            Path("AcceptancePath",
                Applicant.Submit, 
                Manager1Approval.WakeUp().Accept,
                Manager2Approval.WakeUp().Accept, 
                Manager3Approval.WakeUp().Accept);
            //Path("Acceptance",Applicant.Start,Manager1.Accept,Manager2.Accept,Manager3.Accept)
            Path("Rejection", 
                SelectOf("AnyRejection",Selection.FirstOne, 
                    Manager1Approval.Reject,
                    Manager2Approval.Reject,
                    Manager3Approval.Reject),
                    Applicant.WakeUp("Rejected"));
            //any rework the process start again
            Path("Rework", SelectOf("AnyRework", Selection.FirstOne, Manager1Approval.Rework, Manager2Approval.Rework, Manager3Approval.Rework, Applicant.WakeUp().ReSubmit));

            //Custom rework path
            //Path("Manager2Rework",Manager2.Rework,Applicant.Start,Manager2.Any)
            Path("Manager2Rework", Manager2Approval.Rework, Applicant.WakeUp().ReSubmit, Manager2Approval.WakeUp());


            //Pattern 1 (Sequence)
            //A task in a process in enabled after the completion of a preceding task in the same process.
            //Path("Sequence",Manager1.Accept,Manager2.Accept)

            //Pattern 2(Parallel Split)
            //After completion of the capture enrolment task, run the create student profile and issue enrolment confirmation tasks simultaneously.
            //Path("EnrolmentPath",Enrolment.Finshed,SameTime(CreateProfile.Done,EnrollmentConfirmation.Done))
            Path("EnrolmentPath", 
                Manager1Approval.WakeUp().Accept,
                SameStart("TwoManagersApproval", Selection.AllCompleted, Manager2Approval.WakeUp().Accept, Manager3Approval.WakeUp().Accept));

            //Pattern 3 (Synchronization) (Do Step3 only after both Step1 and Step2 are completed)
            //Path("Process",SameTime(Step1.Fire().Done,Step2.Fire().Done),Step3.Fire().Done)
            //Path("Fail",Any(Step1.Fail,Step2.Fail,Step3.Fail),ErrorHappened.Fire().Done)
            Path("Approval", SameStart("TwoManagersStart", Selection.AllCompleted, Manager1Approval.WakeUp(), Manager2Approval.WakeUp()), Manager3Approval.WakeUp());


            //Pattern 4 (Exclusive Choice) [after a, do b or c]
            //this is by default the branching nodes
            //Path("Process",A.Done,SameTime(Optional(1,B.Done,C.Done)))
            Path("Process", Manager1Approval.WakeUp().Accept, SameStart("TwoManagersAndOneIsSufficent", Selection.FirstOne, Manager2Approval.WakeUp(), Manager3Approval.WakeUp()));


            //Pattern 5 (Simple Merge) [as pattern 3 Synchronization]

            //Pattern 6 (Multi-Choice)
            //Depending on the nature of the emergency call, one or more of the despatch-police, despatch-fire-engine and despatch-ambulance tasks is immediately initiated.
            //Path("despatch-police",Start.PoliceRequired,Police.Any)
            //Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.Any)
            //Path("despatch-fire",Start.FireRequired,Fire.Any)
            Path("Manager2", Applicant.Submit, Manager1Approval.WakeUp().Manager2Activated, Manager2Approval.WakeUp().Accept);
            Path("Manager3", Manager1Approval.Manager3Activated, Manager3Approval.WakeUp().Accept);

            //Path("emergency call",Call.Received,
            //SameTime("AfterCall",
            //    Path("Police", Call.PoliceRequired, Police.CallDone),
            //    Path("Ambulance", Call.AmbulanceRequired, Ambulance.CallDone),
            //    Path("Fire", Call.FireRequired, Fire.CallDone)
            //)


            /*
            Pattern 7 (Structured Synchronizing Merge)
            Depending on the type of emergency, either or both of the despatch - police and despatch-ambulance tasks are initiated simultaneously.
            When all emergency vehicles arrive at the accident, the transfer - patient task commences.
            Path("despatch-police",Start.PoliceRequired,Police.VehiclesArrived)
            Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.VehiclesArrived)
            CombineActivePaths("despatch-police","despatch-ambulance",TransferPatient.Any)


            Process("Emergency Call",Call.Received,
                    SameTime("AfterCall",
                        Path("despatch-police",Call.PoliceRequired,Police.VehiclesArrived)
                        Path("despatch-ambulance",Call.AmbulanceRequired,Ambulance.VehiclesArrived)
                    ),
                    CombineActivePaths("despatch-police","despatch-ambulance",TransferPatient.Any)
            )

            */

            Path("Request Approval", Applicant.Submit, Manager1Approval.WakeUp().Accept,
                SameStart(
                    "Manager2&3",
                    Selection.StartedPaths,
                    Path("Manager2Path", Manager2Approval.WakeUp().CanAccept, Manager2Approval.Accept),
                    Path("Manager3Path", Manager3Approval.WakeUp().CanAccept, Manager3Approval.Accept)
                    ));

            //Pattern 8(Multi - Merge) as simple merge 5

            /* Pattern 9 (Structured Discriminator)(1-out-of-M join.)
             * When handling a cardiac arrest, the check_breathing and check_pulse tasks run in parallel. 
             * Once the first of these has completed, the triage task is commenced. 
             * Completion of the other task is ignored and does not result in a second instance of the triage task.
             * 
             * Process("handling a cardiac arrest",
             *  SameTime("Check",CheckBreathing.Done,CheckPulse.Done)
             */

            Path("Request Approval", Applicant.Submit, Manager1Approval.WakeUp().Accept,
                SameStart(
                    "First Manager Approval",
                    Selection.FirstOneAndCancelOthers,
                    Manager2Approval.WakeUp().Accept,
                    Manager3Approval.WakeUp().Accept)
                );
            Path("Any Manager Reject",
                SelectOf("Any Rejection", Selection.FirstOne, Manager1Approval.Reject, Manager2Approval.Reject, Manager3Approval.Reject), Applicant.WakeUp().ReSubmit,
                GoToPath("Request Approval"));
            //Optional operator
            //Path("AcceptancePath",Manager1.Accept,Optional(Manager2.Accept),Manager3.Accept)
            //Order opertaor for SameTime tasks
            //Path("AcceptancePath",Applicant.SendRequest,Order(1,Manager1.Accept),Order(1,Manager2.Accept),Manager3.Accept)

            /*
             Path("ComplexPath",
		            A.Accept,
		            Order(1,Path("BPath",B1.Accept,Optional(B2.Accept))),
		            Order(1,Path("CPath",C1.Accept,C2.Accept)),
		            Order(2,D.Accept)
	             );
            
             1.1.1
             1.1.2
             1.2.1
             1.2.2
             */

            /*SameTime instead of order
             Path("ComplexPath",
		            A.Accept,
		            SameTime("B&C",Path("BPath",B1.Accept,Optional(B2.Accept))),
		            SameTime("B&C",Path("CPath",C1.Accept,C2.Accept)),
		            D.Accept //will activated after BPath and CPath 
	             );
            */

            /*Define opertor to not go out of current path
            Path("ComplexPath",
                   A.Accept,
                   SameTime("B&C",Path("BPath",B1.Accept,Optional(B2.Accept))),
                   Define(Path("BPathSub",B1.Accept,B3.Review,B4.Review)),
                   SameTime("B&C",Path("CPath",C1.Accept,C2.Accept)),
                   D.Accept //will activated after BPath and CPath 
                );
           */

            //Pattern 7 (Structured Synchronizing Merge)
            //Depending on the type of emergency, either or both of the despatch - police and despatch-ambulance tasks are initiated simultaneously.
            //When all emergency vehicles arrive at the accident, the transfer - patient task commences.
            /*
             Path("AccidentHappen",UserCallIncome.Any,
                    SameTime("ProcessStart",Optional(Police.start),Optional(Ambulance.start),Optional(Fire.start)),
                    PatientTransfer.Any)
            */


        }
    }

    public interface IFakeProcess2 : IProcess
    {
        new IFakeProcess2 WakeUp();
        

        ProcessOutputNode ReSubmit => null;
        ProcessOutputNode Submit => null;
    }
    public interface IFakeProcess : IProcess
    {
        new IFakeProcess WakeUp();
        ProcessOutputNode CanAccept => null;
        ProcessOutputNode Accept => null;
        ProcessOutputNode Reject => null;
        ProcessOutputNode Rework => null;
        ProcessOutputNode Manager2Activated => null;
        ProcessOutputNode Manager3Activated => null;
    }
}
