using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Covid.Presentation.Common
{
    public class Views
    {
        public static string Login = "~/Views/Login/LogIn.cshtml";
        public static string LoginAPIError = "~/Views/Login/LoginAPIError.cshtml";
        public static string UserRegistration = "~/Views/UserRegistration/UserRegistration.cshtml";
        public static string UserAttendanceDetails = "~/Views/UserRegistration/UserAttendanceDetails.cshtml";
        public static string UserAttendanceDetailsPartial = "~/Views/UserRegistration/PartialViews/UserAttendancePartial.cshtml";
        public static string RRTUserMapping = "~/Views/UserRegistration/RRTUserMapping.cshtml";
        public static string UserMappingWard = "~/Views/UserRegistration/UserMappingWard.cshtml";
        public static string UserTypeWiseMenu = "~/Views/UserRegistration/UserTypeWiseMenu.cshtml";
        public static string UserAttendance = "~/Views/UserRegistration/UserAttendance.cshtml";
        public static string PersonCreation = "~/Views/Person/PersonCreation.cshtml";
        public static string Dashboard = "~/Views/DashBoard/DashBoard.cshtml";
        public static string CoronaDashboard = "~/Views/Home/CoronaDashboard.cshtml";
        public static string CoronaDataUpload= "~/Views/CoronaUpdate/HoshpitalDataUpdate.cshtml";
        public static string CoronaStatus = "~/Views/CoronaUpdate/CoronaStatusUpdate.cshtml";
        public static string CoronaHospitalMaster = "~/Views/CoronaUpdate/HospitalMasterUpdate.cshtml";

        public static string PersonWithNoWardPartial = "~/Views/Person/PartialViews/PersonWithNoWardPartial.cshtml";
        public static string PersonSearch = "~/Views/Person/PersonSearch.cshtml";
        public static string CovidPersonSearch = "~/Views/Person/CovidPersonSearch.cshtml";
        public static string PersonDetailsModal = "~/Views/Person/PartialViews/PersonDetailsModal.cshtml";
        public static string PersonSearchPartial = "~/Views/Person/PartialViews/PersonSearchPartial.cshtml";
        public static string CovidPersonSearchPartial = "~/Views/Person/PartialViews/CovidPersonSearchPartial.cshtml";
        public static string PersondetailsHierarichal = "~/Views/Person/PersondetailsHierarichal.cshtml";
        public static string PersondetailsHierarichalPartial = "~/Views/Person/PartialViews/PersondetailsHierarichalPartial.cshtml";
        public static string PersondetailsHierarichalPartialMap = "~/Views/Person/PartialViews/PersondetailsHierarichalPartialMap.cshtml";
        public static string NewEntryPersonDetails = "~/Views/Person/NewEntryPersonDetails.cshtml";
        public static string NewEntryPersonDetailsPartial = "~/Views/Person/PartialViews/NewEntryPersonDetailsPartial.cshtml";
        public static string ComplaintForm = "~/Views/Complaint/ComplaintForm.cshtml";
        public static string QurantineCheckForm = "~/Views/QuarantineCheck/QurantineCheckForm.cshtml";
        public static string HomeQurantineCheckForm = "~/Views/QuarantineCheck/HomeQurantineCheckForm.cshtml";
        public static string PersonDetailUpdateCheckForm = "~/Views/Person/PersonDetailsUpdateForm.cshtml";

        
        public static string GetQuarantineDetails = "~/Views/QuarantineCheck/GetQuarantineDetails.cshtml";
        public static string GetQuarantineDetailsPartial = "~/Views/QuarantineCheck/PartialViews/GetQuarantineDetailsPartial.cshtml";
        public static string GetQuarantineDetailsPersonPartial = "~/Views/Person/PartialViews/GetQuarantineDetailsPersonPartial.cshtml";
        public static string GetPersonNotQuarantineDetails = "~/Views/QuarantineCheck/GetPersonNotQuarantineDetails.cshtml";
        public static string HomeQuarantineSearch = "~/Views/QuarantineCheck/HomeQuarantineSearch.cshtml";
        public static string GetPersonNotQuarantineDetailsPartial = "~/Views/QuarantineCheck/PartialViews/GetPersonNotQuarantineDetailsPartial.cshtml";
        public static string GetHomeQuarantinePartial = "~/Views/QuarantineCheck/PartialViews/GetHomeQuarantinePartial.cshtml";
        public static string ComplaintAssignment = "~/Views/Complaint/ComplaintAssignment.cshtml";
        public static string ComplaintSearchPublic = "~/Views/Complaint/ComplaintSearchPublic.cshtml";
        public static string HealthUpdate = "~/Views/Person/HealthUpdate.cshtml";
        public static string HealthUpdatePartial = "~/Views/Person/PartialViews/HealthUpdatePartial.cshtml";

        public static string ComplaintViewPublicPartial = "~/Views/Complaint/ComplaintViewPublicPartial.cshtml";


        //for dashboard
        public static string PersonSearchPartialDashBoard = "~/Views/DashBoard/PartialViews/PersonSearchPartialDashBoard.cshtml";
        public static string ShowPersonData = "~/Views/DashBoard/ShowPersonData.cshtml";
        public static string ShowComplaints= "~/Views/DashBoard/ShowComplaints.cshtml";

        public static string SeroSurveForm = "~/Views/SeroSurve/SeroSurveForm.cshtml";
        public static string GetSeroSurveDetailsPartial = "~/Views/SeroSurve/PartialViews/GetSeroSurveDetailsPartial.cshtml";
        public static string GetSeroSurveDetails = "~/Views/SeroSurve/GetSeroSurveDetails.cshtml";
        public static string GetSeroSurveReports= "~/Views/SeroSurve/GetSeroSurveReports.cshtml";

        public static string StreetLightSurve = "~/Views/PoleSurve/StreetLightSurveForm.cshtml";
        public static string SwitchingSurve = "~/Views/PoleSurve/SwitchingSurveForm.cshtml";
        public static string GetPoleSurveDetailsPartial = "~/Views/PoleSurve/PartialViews/GetPoleSurveDetailsPartial.cshtml";
        public static string GetPoleSurveReports = "~/Views/PoleSurve/GetPoleSurveReports.cshtml";

    }
}