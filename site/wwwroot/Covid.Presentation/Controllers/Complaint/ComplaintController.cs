using Covid.Core.DBEntities.Complaint;
using Covid.Core.IRepo;
using Covid.Infrastructure.ViewModel.Complaint;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Covid.Core.Enum;
using Covid.Presentation.Common;
using Covid.Infrastructure.Helpers;
using Covid.Core.Common;
using Covid.Core.DBEntities.LocalBody;
using Covid.Core.DBEntities.UserDetails;

namespace Covid.Presentation.Controllers.Complaint
{
   
    public class ComplaintController : Controller
    {
        IComplaintRepository cRepo;
        ICommonRepository commonRepo;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public ComplaintController(IComplaintRepository ICRepo, ICommonRepository common)
        {
            this.commonRepo = common;
            this.cRepo = ICRepo;
        }
        // GET: Complaint

        public ActionResult OpenComplaintForm()
        {

            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = commonRepo.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmComplaint vm = new vmComplaint();
            vm.ComplaintTypeList = cRepo.GetAllComplaintTypes();
            vm.LocalBodyList = commonRepo.GetLocalyBody();
            return View(Views.ComplaintForm, vm);
        }

        public ActionResult OpenComplaintSearchForm()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = commonRepo.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmComplaint vm = new vmComplaint();
            return View(Views.ComplaintSearchPublic, vm);
        }
        public ActionResult OpenComplaintSearchInternalForm()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = commonRepo.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmComplaint vm = new vmComplaint();
            return View("~/Views/Complaint/ComplaintSearchInternal.cshtml", vm);
        }


        public ActionResult AddComplaint(FormCollection form)
        {
            mComplaint complaintDetails = new mComplaint();
            complaintDetails.Name = form["Name"];
            complaintDetails.Address = form["Address"];
            complaintDetails.MobileNo = Convert.ToInt64(form["MobileNo"]);
            complaintDetails.ComplaintTypeId = Convert.ToInt32(form["ComplaintType"]);
            complaintDetails.Email = form["Email"];
            complaintDetails.Details = form["Details"];
            complaintDetails.LocalbodyId = Convert.ToInt32(form["LocalBody"]);
            complaintDetails.WardId = Convert.ToInt32(form["Ward"] == "" ? null : form["Ward"]);
            complaintDetails.ZoneId = commonRepo.GetWardParentid(complaintDetails.WardId).LocalBodyParentId;

                //Convert.ToInt32(form["Zone"]);
            
            complaintDetails.Status = 1;
            if (SessionHelper.UserDetails is null)
            {
                complaintDetails.CreatedBy = 0;
            }
            else
            {
                complaintDetails.CreatedBy = SessionHelper.UserDetails.UserId;
            }
                cRepo.InsertComplaint(complaintDetails);
            ViewBag.Message = "Complaint Added ! ! !";
            if (SessionHelper.UserDetails is null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            else
            {
                
                return RedirectToAction("OpenComplaintForm");
            }
        }
        public ActionResult UpdateComplaint(int ComplaintId)
        {
            vmComplaint vm = new vmComplaint();
            vm.ComplaintDetails = cRepo.GetComplaintDetailsBasedonId(ComplaintId);
            vm.ComplaintTypeList = cRepo.GetAllComplaintTypes();
            vm.LocalBodyList = commonRepo.GetLocalyBody();
            return View(Views.ComplaintForm, vm);
        }

        public ActionResult OpenAssignmentForm(int ComplaintId,int complaintTypeID)
        {
            
            vmComplaint vm = new vmComplaint();
            vm.ComplaintDetailsList = cRepo.GetComplaintDetailsListBasedonId(ComplaintId);
            vm.ComplaintDetails = cRepo.GetComplaintDetailsBasedonId(ComplaintId);
            vm.LocalBody.LocalBodyID = Convert.ToInt32(vm.ComplaintDetails.LocalbodyId);
            if (vm.ComplaintDetails.ZoneId != 0)
                vm.ZoneBody= commonRepo.GetDetailsbyid(vm.ComplaintDetails.ZoneId);
            if(vm.ComplaintDetails.WardId!=0)
                vm.WardBody = commonRepo.GetDetailsbyid(vm.ComplaintDetails.WardId);
            vm.UserNames = cRepo.GetAllUserNamesBasedonComplaintType(complaintTypeID);
            vm.UserType = commonRepo.GetAllUserType();
            vm.LocalBodyList = commonRepo.GetLocalyBody();
            return View(Views.ComplaintAssignment, vm);
        }
        public ActionResult UpdateandAssignComplaint(FormCollection form)
        {
            vmComplaint vm = new vmComplaint();
            mComplaint complaintDetails = new mComplaint();
            complaintDetails.ComplaintId = Convert.ToInt32(form["ComplaintId"]);
            complaintDetails.LocalbodyId = Convert.ToInt32(form["LocalBody"]);
            complaintDetails.ZoneId = Convert.ToInt32(form["Zone"]);
            complaintDetails.WardId = Convert.ToInt32(form["Ward"]);
            complaintDetails.Status = Convert.ToByte( form["Status"]);
            complaintDetails.Remark = form["Remark"];
            //complaintDetails.AssignTo = Convert.ToInt32(form["AssignTo"]);
            cRepo.UpdateAssignmentDetails(complaintDetails);
            return (SessionHelper.UserDetails.UserType.ToUpper() == UserTypeCommon.CommandCenter.ToUpper()? RedirectToAction("ComplaintDetails"): RedirectToAction("GetComplaintDetails","DashBoard"));

        }
        public JsonResult GetZoneDropDown(int LocalBodyId)
        {
            var ZoneList = commonRepo.GetZoneOrWardByParentID(LocalBodyId);
            return Json(ZoneList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWardByZoneDropDown(int LocalBodyId)
        {
            var Wardlist = new List<mLocalBody>();
            if(SessionHelper.UserDetails.UserType.ToUpper()==UserTypeCommon.IncidentManager.ToUpper())
            {
                Wardlist = commonRepo.GetZoneOrWardByUserIdParentID(SessionHelper.UserDetails.UserId,LocalBodyId);
            }
            else
            {
                Wardlist = commonRepo.GetZoneOrWardByParentID(LocalBodyId);
            }
             
            return Json(Wardlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWardDropDown(int LocalBodyId)
        {
            var ZoneList = commonRepo.GetWardByParentID(LocalBodyId);
            return Json(ZoneList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComplaintDetails()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = commonRepo.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
                SessionHelper.UserDetails = user;
                var ust = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                if (ust != null)
                {
                    SessionHelper.UserDetails.UserType = ust.UserType;
                }
            }
            vmComplaint vm = new vmComplaint();
            vm.ComplaintDetailsList = cRepo.GetAllComplaintDetails(null, null, null, null, 0,"0");

            //foreach(var x in vm.ComplaintDetailsList)
            //{
            //    x.localbodyname = commonRepo.GetDetailsbyid(x.LocalbodyId).LocalBodyName;
            //    x.zonename = commonRepo.GetDetailsbyid(x.ZoneId).LocalBodyName;
            //    x.wardname = commonRepo.GetDetailsbyid(x.WardId) is null ? "" : commonRepo.GetDetailsbyid(x.WardId).LocalBodyName;
            //}
            
            vm.ComplaintTypeList = cRepo.GetAllComplaintTypes();
            vm.LocalBodyList = commonRepo.GetLocalyBody();

            return View(vm);
        } 

        public ActionResult GetComplaintByFilter(string LOcalBodyID, string ZoneID, string WardID, string DateOfCreation, int? ComplaintType,string Status)
            {
                vmComplaint vm = new vmComplaint();
                vm.ComplaintDetailsList = cRepo.GetAllComplaintDetails(LOcalBodyID, ZoneID, WardID, DateOfCreation, ComplaintType, Status);
            //foreach (var x in vm.ComplaintDetailsList)
            //{
            //    x.localbodyname = commonRepo.GetDetailsbyid(x.LocalbodyId).LocalBodyName;
            //    x.zonename = commonRepo.GetDetailsbyid(x.ZoneId).LocalBodyName;
            //    x.wardname = commonRepo.GetDetailsbyid(x.WardId).LocalBodyName;
            //}
            vm.ComplaintTypeList = cRepo.GetAllComplaintTypes();
                vm.LocalBodyList = commonRepo.GetLocalyBody();
                return PartialView("~/Views/Complaint/PartialViews/ComplaintDetailsPartial.cshtml", vm);
            }

        public ActionResult GetComplaintInternalDetails( String FromDate, String ToDate)
        {
            vmComplaint vm = new vmComplaint();
            vm.ComplaintDetailsList = cRepo.GetAllComplaintDetails(Convert.ToDateTime(FromDate == "" ? null : FromDate),
                Convert.ToDateTime(ToDate == "" ? null : ToDate), SessionHelper.UserDetails.UserId);
            return PartialView("~/Views/Complaint/PartialViews/ComplaintDetailsInternalPartial.cshtml", vm);
        }

        public ActionResult GetComplaintPublicDetails(String MobileNumber)
        {
            vmComplaint vm = new vmComplaint();
            vm.ComplaintDetailsList = cRepo.GetAllComplaintDetails(MobileNumber);
            return PartialView("~/Views/Complaint/PartialViews/ComplaintViewPublicPartial.cshtml", vm);
        }
        }
}