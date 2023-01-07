//using System.Threading.Tasks;
//using WorkflowInCode.Abstraction.Engine;
//using static WorkflowInCode.Abstraction.Engine.WorkflowInstance;
//namespace WorkflowInCode.Abstraction.Samples
//{
//    public class test
//    {
//        private readonly IManagerApprovalProcess Manager1Approval;
//        private readonly IManagerApprovalProcess Manager2Approval;
//        private readonly IManagerApprovalProcess Manager3Approval;
//        private readonly IApplicantProcess Applicant;

//        public test(IManagerApprovalProcess p1, IManagerApprovalProcess p2, IManagerApprovalProcess p3, IApplicantProcess ap)
//        {
//            Manager1Approval = p1;
//            Manager2Approval = p2;
//            Manager3Approval = p3;
//            Applicant = ap;
//            ////Sequence
//            Path("AcceptancePath",
//                Applicant.ApplicationSubmitted,
//                Manager1Approval.AskApproval().Accept,
//                Manager2Approval.AskApproval().Accept,
//                Manager3Approval.AskApproval().Accept).Sequential();
//            //Path("Acceptance",Applicant.Start,Manager1.Accept,Manager2.Accept,Manager3.Accept)
//            Path("Rejection",
//                Path("AnyRejection",
//                    Manager1Approval.Reject,
//                    Manager2Approval.Reject,
//                    Manager3Approval.Reject).FirstMatch(),
//                    Applicant.InformAllManagers("Rejected"));
//            //any rework the process start again
//            Path("Rework", Path("AnyRework", Manager1Approval.Rework,
//                    Manager2Approval.Rework,
//                    Manager3Approval.Rework),
//                Applicant.AskReSubmitt().ApplicationReSubmitted);

//            //Custom rework path
//            //Path("Manager2Rework",Manager2.Rework,Applicant.Start,Manager2.Any)
//            Path("Manager2Rework", Manager2Approval.Rework, Applicant.AskReSubmitt().ApplicationReSubmitted, Manager2Approval.AskApproval());


//            //Pattern 1 (Sequence)
//            //A task in a process in enabled after the completion of a preceding task in the same process.
//            //Path("Sequence",Manager1.Accept,Manager2.Accept)

//            //Pattern 2(Parallel Split)
//            //After completion of the capture enrolment task, run the create student profile and issue enrolment confirmation tasks simultaneously.
//            //Path("EnrolmentPath",Enrolment.Finshed,SameTime(CreateProfile.Done,EnrollmentConfirmation.Done))
//            Path("EnrolmentPath",
//                Manager1Approval.AskApproval().Accept,
//                Path("TwoManagersApproval", Manager2Approval.AskApproval().Accept, Manager3Approval.AskApproval().Accept));

//            //Pattern 3 (Synchronization) (Do Step3 only after both Step1 and Step2 are completed)
//            //Path("Process",SameTime(Step1.Fire().Done,Step2.Fire().Done),Step3.Fire().Done)
//            //Path("Fail",Any(Step1.Fail,Step2.Fail,Step3.Fail),ErrorHappened.Fire().Done)
//            Path("Approval", Path("TwoManagersStart", Manager1Approval.AskApproval(), Manager2Approval.AskApproval()), Manager3Approval.AskApproval());


//            //Pattern 4 (Exclusive Choice) [after a, do b or c]
//            //this is by default the branching nodes
//            //Path("Process",A.Done,SameTime(Optional(1,B.Done,C.Done)))
//            Path("Process", Manager1Approval.AskApproval().Accept, Path("TwoManagersAndOneIsSufficent", Manager2Approval.AskApproval(), Manager3Approval.AskApproval()));


//            //Pattern 5 (Simple Merge) [as pattern 3 Synchronization]

//            //Pattern 6 (Multi-Choice)
//            //Depending on the nature of the emergency call, one or more of the despatch-police, despatch-fire-engine and despatch-ambulance tasks is immediately initiated.
//            //Path("despatch-police",Start.PoliceRequired,Police.Any)
//            //Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.Any)
//            //Path("despatch-fire",Start.FireRequired,Fire.Any)
//            Path("Manager2", Applicant.ApplicationSubmitted, Manager1Approval.AskApproval().Manager2Activated, Manager2Approval.AskApproval().Accept);
//            Path("Manager3", Manager1Approval.Manager3Activated, Manager3Approval.AskApproval().Accept);

//            //Path("emergency call",Call.Received,
//            //SameTime("AfterCall",
//            //    Path("Police", Call.PoliceRequired, Police.CallDone),
//            //    Path("Ambulance", Call.AmbulanceRequired, Ambulance.CallDone),
//            //    Path("Fire", Call.FireRequired, Fire.CallDone)
//            //)


//            /*
//            Pattern 7 (Structured Synchronizing Merge)
//            Depending on the type of emergency, either or both of the despatch - police and despatch-ambulance tasks are initiated simultaneously.
//            When all emergency vehicles arrive at the accident, the transfer - patient task commences.
//            Path("despatch-police",Start.PoliceRequired,Police.VehiclesArrived)
//            Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.VehiclesArrived)
//            CombineActivePaths("despatch-police","despatch-ambulance",TransferPatient.Any)


//            Process("Emergency Call",Call.Received,
//                    SameTime("AfterCall",
//                        Path("despatch-police",Call.PoliceRequired,Police.VehiclesArrived)
//                        Path("despatch-ambulance",Call.AmbulanceRequired,Ambulance.VehiclesArrived)
//                    ),
//                    CombineActivePaths("despatch-police","despatch-ambulance",TransferPatient.Any)
//            )

//            */

//            Path("Request Approval", Applicant.ApplicationSubmitted, Manager1Approval.AskApproval().Accept,
//                Path(
//                    "Manager2&3",
//                    Path("Manager2Path", Manager2Approval.AskApproval().CanAccept, Manager2Approval.Accept),
//                    Path("Manager3Path", Manager3Approval.AskApproval().CanAccept, Manager3Approval.Accept)
//                    ));

//            //Pattern 8(Multi - Merge) as simple merge 5

//            /* Pattern 9 (Structured Discriminator)(1-out-of-M join.)
//             * When handling a cardiac arrest, the check_breathing and check_pulse tasks run in parallel. 
//             * Once the first of these has completed, the triage task is commenced. 
//             * Completion of the other task is ignored and does not result in a second instance of the triage task.
//             * 
//             * Process("handling a cardiac arrest",
//             *  SameTime("Check",CheckBreathing.Done,CheckPulse.Done)
//             */

//            Path("Request Approval", Applicant.ApplicationSubmitted, Manager1Approval.AskApproval().Accept,
//                Path(
//                    "First Manager Approval",
//                    Manager2Approval.AskApproval().Accept,
//                    Manager3Approval.AskApproval().Accept)
//                );

//            Path("Any manager reject ask applicant to submitt again",
//                Path("Any rejection", Manager1Approval.Reject,
//                    Manager2Approval.Reject,
//                    Manager3Approval.Reject),
//                Applicant.AskReSubmitt().ApplicationReSubmitted);
//            //Optional operator
//            //Path("AcceptancePath",Manager1.Accept,Optional(Manager2.Accept),Manager3.Accept)
//            //Order opertaor for SameTime tasks
//            //Path("AcceptancePath",Applicant.SendRequest,Order(1,Manager1.Accept),Order(1,Manager2.Accept),Manager3.Accept)

//            /*
//             Path("ComplexPath",
//		            A.Accept,
//		            Order(1,Path("BPath",B1.Accept,Optional(B2.Accept))),
//		            Order(1,Path("CPath",C1.Accept,C2.Accept)),
//		            Order(2,D.Accept)
//	             );
            
//             1.1.1
//             1.1.2
//             1.2.1
//             1.2.2
//             */

//            /*SameTime instead of order
//             Path("ComplexPath",
//		            A.Accept,
//		            SameTime("B&C",Path("BPath",B1.Accept,Optional(B2.Accept))),
//		            SameTime("B&C",Path("CPath",C1.Accept,C2.Accept)),
//		            D.Accept //will activated after BPath and CPath 
//	             );
//            */

//            /*Define opertor to not go out of current path
//            Path("ComplexPath",
//                   A.Accept,
//                   SameTime("B&C",Path("BPath",B1.Accept,Optional(B2.Accept))),
//                   Define(Path("BPathSub",B1.Accept,B3.Review,B4.Review)),
//                   SameTime("B&C",Path("CPath",C1.Accept,C2.Accept)),
//                   D.Accept //will activated after BPath and CPath 
//                );
//           */

//            //Pattern 7 (Structured Synchronizing Merge)
//            //Depending on the type of emergency, either or both of the despatch - police and despatch-ambulance tasks are initiated simultaneously.
//            //When all emergency vehicles arrive at the accident, the transfer - patient task commences.
//            /*
//             Path("AccidentHappen",UserCallIncome.Any,
//                    SameTime("ProcessStart",Optional(Police.start),Optional(Ambulance.start),Optional(Fire.start)),
//                    PatientTransfer.Any)
//            */


//        }
//    }

//    public interface IApplicantProcess : IWorkFlowProcess
//    {

//        IApplicantProcess SubmitApplication();


//        IApplicantProcess AskReSubmitt();


//        IWorkFlowProcess InformAllManagers(object input);


//        bool ApplicationReSubmitted => true;

//        bool ApplicationSubmitted => true;
//    }
//    public interface IManagerApprovalProcess : IWorkFlowProcess
//    {

//        IManagerApprovalProcess AskApproval();


//        IManagerApprovalProcess SendApproval();


//        bool CanAccept => true;


//        bool Accept => true;


//        bool Reject => true;


//        bool Rework => true;


//        bool Manager2Activated => true;


//        bool Manager3Activated => true;
//    }
//}
