using Covid.Core.Common;
using Covid.Core.DBEntities.LocalBody;
using Covid.Core.DBEntities.UserDetails;
using Covid.Core.IRepo;
using Covid.Infrastructure.Helpers;
using Covid.Infrastructure.ViewModel.Dashboard;
using Covid.Infrastructure.ViewModel.Person;
using Covid.Presentation.Common;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covid.Presentation.Controllers
{
   [SessionExpire]
    public class DashBoardController : Controller
    {
        IDashboardRepository  ICRepo;
        private readonly ICommonRepository dCommon;
        private readonly IUserRepository dUser;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public DashBoardController(IDashboardRepository ICRepo, ICommonRepository dCommon, IPersonRepository dPerson, IUserRepository dUser)
        {
            this.ICRepo = ICRepo;
            this.dCommon = dCommon;
            this.dUser = dUser;
        }

        // GET: DashBoard
        public ActionResult DashBoard()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmDashboard dashboardList = new vmDashboard();
            dashboardList.DashboardCountList = ICRepo.GetDashboardList(SessionHelper.UserDetails.UserTypeId,null,SessionHelper.UserDetails.UserId);
            dashboardList.LocalBodyList = dCommon.GetLocalyBody();
            dashboardList.GetZoneList = dCommon.GetZoneOrWardByParentID(dashboardList.DashboardCount.LocalBodyId);
           // dashboardList.GetWardList = dCommon.GetZoneOrWardByParentID(dashboardList.DashboardCount.ZoneId);
            TempData["Msg"]=dashboardList.DashboardCount.UserType;
            //return View(AdminList);
            return View(Views.Dashboard, dashboardList);
        }
        [HttpGet]
        public JsonResult FetchZone(int ID)
        {
            var data = dCommon.GetZoneOrWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchWard(int ID)
        {
            var Wardlist = new List<mLocalBody>().Select(l=> new { Value = 0, Text = "" });
            if (SessionHelper.UserDetails.UserType.ToUpper() == UserTypeCommon.IncidentManager.ToUpper())
            {
                Wardlist = dCommon.GetWardByUserIDParentID(SessionHelper.UserDetails.UserId,ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            }
            else
            {
                Wardlist = dCommon.GetWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            }
            
            return Json(Wardlist, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult ZoneCountURlz(int ID)
        {
            vmDashboard dashboardList = new vmDashboard();
            //dashboardList.DashboardCount = ICRepo.GetDashboardList(SessionHelper.UserDetails.UserTypeId, ID).Select(z=>z.ZoneWiseQuarantine==);
            dashboardList.DashboardCountList = ICRepo.GetDashboardList(SessionHelper.UserDetails.UserTypeId,ID,SessionHelper.UserDetails.UserId);
            var  x =dCommon.GetDetailsbyid(ID);
            var y = 0;var a = 0;var b = 0;
            foreach (var item in dashboardList.DashboardCountList)
            {
                y = item.ZoneWiseQuarantine;
                a = item.QuarantineCheckcompleted;
                b = item.QuarantineCheckRemaining;
            }
            //dashboardList.DashboardCount.ZoneWiseQuarantine=y;
            //var y= dashboardList.DashboardCountList.se
            var data = new { Value = y,Completed=a,Remaning=b, Text =x.LocalBodyName };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult WardURlz(int ID)
        {
            vmDashboard dashboardList = new vmDashboard();
            //dashboardList.DashboardCount = ICRepo.GetDashboardList(SessionHelper.UserDetails.UserTypeId, ID).Select(z=>z.ZoneWiseQuarantine==);
            dashboardList.DashboardCountList = ICRepo.GetDashboardList(SessionHelper.UserDetails.UserTypeId, ID, SessionHelper.UserDetails.UserId);
            var x = dCommon.GetDetailsbyid(ID);
            var twac = 0; var twaoc = 0; var twacc = 0; var twawc = 0; var twarc = 0;
            foreach (var item in dashboardList.DashboardCountList)
            {
                twac = item.TotalWardComplaints;
                twaoc = item.TotalWardOpenComplaints;
                twacc = item.TotalWardClosedComplaint;
                twawc = item.TotalWardWorkingComplaint;
                twarc = item.TotalWardRejectComplaint;
            }
            //dashboardList.DashboardCount.ZoneWiseQuarantine=y;
            //var y= dashboardList.DashboardCountList.se
            var data = new { TWAC = twac, TWAOC = twaoc, TWACC = twacc, TWAWC=twawc, TWARC=twarc, Text = x.LocalBodyName };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetIPersonDeailByFilter(int Isquarantine,int IsPositive)
        {
            
            vmDashboard vmPersonData = new vmDashboard();
            var LOcalBodyID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId)==null?0: dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).LocalBodyId;
            var ZoneID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId) == null ? 0 : dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).ZoneId;
            var WardID =dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId) == null ? 0 : dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).WardId;
            vmPersonData.CheckPointList = dCommon.GetCheckPoint();
            vmPersonData.IsQ = Isquarantine;
            vmPersonData.IsP = IsPositive;
            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.PersonAllDetalsList = ICRepo.GetIPersonDeailByFilter(LOcalBodyID, ZoneID, WardID, Isquarantine, IsPositive,null);

            if(Isquarantine==0 && IsPositive==0)
            {
                TempData["Dispaly"] = "Person Assign to me";

            }
            else
            {
                TempData["Dispaly"] = "Quarantine Check";
            }

            //var ust = dCommon.GetUserType(UserTypeCommon.RRT.ToUpper());
            //if (SessionHelper.UserDetails.UserTypeId == ust.UserTypeId)
            //{ }
            return View(Views.ShowPersonData, vmPersonData);
            //return PartialView(Views.PersonSearchPartialDashBoard, vmPersonData);
        }
        public ActionResult GetPartialPersonDeailByFilter(int Isquarantine, int IsPositive, int? CheckPoint)
        {
            
            vmDashboard vmPersonData = new vmDashboard();
            var LOcalBodyID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).LocalBodyId;
            var ZoneID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).ZoneId;
            var WardID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).WardId;
            

            vmPersonData.LocalBodyList = dCommon.GetLocalyBody();
            vmPersonData.PersonAllDetalsList = ICRepo.GetIPersonDeailByFilter(LOcalBodyID, ZoneID, WardID, Isquarantine, IsPositive, CheckPoint);


            //var ust = dCommon.GetUserType(UserTypeCommon.RRT.ToUpper());
            //if (SessionHelper.UserDetails.UserTypeId == ust.UserTypeId)
            //{ }
            return View(Views.PersonSearchPartialDashBoard, vmPersonData);
            //return PartialView(Views.PersonSearchPartialDashBoard, vmPersonData);
        }
        public ActionResult GetComplaintDetails()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = dCommon.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = dCommon.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            //var LOcalBodyID = 0;
            //var ZoneID = 0;
            //var WardID = 0;
            vmDashboard vm = new vmDashboard();
            //    if(SessionHelper.UserDetails.UserType.ToUpper()==UserTypeCommon.RRT.ToUpper())
            //    {
            //        LOcalBodyID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).LocalBodyId;
            //        ZoneID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).ZoneId;
            //        WardID = dUser.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId).WardId;
            //    }



            //vm.LocalBodyList = dCommon.GetLocalyBody();
            //    vm.ComplaintDetailsList = ICRepo.GetComplaintsByFilter(LOcalBodyID, ZoneID, WardID,SessionHelper.UserDetails.UserTypeId);
            //    foreach (var x in vm.ComplaintDetailsList)
            //    {
            //        x.localbodyname =dCommon.GetDetailsbyid(x.LocalbodyId).LocalBodyName;
            //        x.zonename = dCommon.GetDetailsbyid(x.ZoneId).LocalBodyName;
            //        x.wardname = dCommon.GetDetailsbyid(x.WardId) is null ? "" : dCommon.GetDetailsbyid(x.WardId).LocalBodyName; ;
            //    }
            vm.ComplaintDetailsList = ICRepo.GetComplaintsByUserID(SessionHelper.UserDetails.UserId, SessionHelper.UserDetails.UserTypeId, SessionHelper.UserDetails.UserType);
            return View(Views.ShowComplaints, vm);
        }

    }
}