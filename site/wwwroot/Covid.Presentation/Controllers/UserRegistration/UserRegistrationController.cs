using Covid.Core.IRepo;
using Covid.Infrastructure.Helpers;
using Covid.Infrastructure.ViewModel.vmUserDetails;
using Covid.Presentation.Common;
using Covid.Core.Common;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Covid.Core.DBEntities.UserDetails;
using System.IO;

namespace Covid.Presentation.Controllers
{   
    public class UserRegistrationController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ICommonRepository commonRepo;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public UserRegistrationController(IUserRepository userRepo, ICommonRepository commonRepo)
        {
            this.userRepo = userRepo;
            this.commonRepo = commonRepo;
        }
        [SessionExpire]
        public ActionResult UserRegistration()
        {
            try
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
                    var usrty = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                    if (usrty != null)
                    {
                        SessionHelper.UserDetails.UserType = usrty.UserType;
                    }
                }
                vmUserDetails vm = new vmUserDetails();
                var ust = commonRepo.GetUserType(UserTypeCommon.RRT.ToUpper());
                if(SessionHelper.UserDetails.UserTypeId==ust.UserTypeId )
                {
                    vm.usertypeList = userRepo.GetAllUserType().Where(x => x.UserTypeId == ust.UserTypeId).ToList();
                }
                else
                {
                    vm.usertypeList = userRepo.GetAllUserType();
                }
               
                  return View(Views.UserRegistration, vm);
            }
            catch (Exception ex)
            {
                return View(Views.UserRegistration, null);
            }
        }
        public ActionResult SumbitData(FormCollection form)
        {
            try
            {
                vmUserDetails vm = new vmUserDetails();
                vm.UserReg.UserTypeId = Convert.ToInt32(form["UserTypeID"]);
                vm.UserReg.Name = Convert.ToString(form["Name"]).Trim();
                vm.UserReg.EmailId = Convert.ToString(form["EmailID"]);
                vm.UserReg.MobileNo = Convert.ToInt64(form["MobileNo"]);
                vm.UserReg.Password = Convert.ToString(form["Password"]);
                var us = userRepo.GetUserDetailByMobileNo(vm.UserReg.MobileNo);
                if(us!=null)
                {
                    TempData["UserMessage"] = "Data Already Exists";
                    return RedirectToAction("UserRegistration");
                }
                  bool check = userRepo.AddUser(vm.UserReg);
                if(check==false)
                {
                    TempData["UserMessage"] = "Data not submitted something went wrong";
                    return RedirectToAction("UserRegistration");
                }
                TempData["UserMessage"] = "Data Submit Sucessfully";
                return RedirectToAction("UserRegistration");
            }
            catch(Exception ex)
            {
                TempData["UserMessage"] = "Something Went Wrong";
                return View(Views.UserRegistration, null);

            }
        }
        [SessionExpire]
        public ActionResult RRTUserMapping()
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
            vmUserDetails vm = new vmUserDetails();
           
            //var ust = commonRepo.GetUserType(UserTypeCommon.RRT.ToUpper());
            if (SessionHelper.UserDetails.UserType.ToUpper() == UserTypeCommon.RRT.ToUpper())
            {
                var usm = userRepo.GetUserRRTZoneMappingByUserId(SessionHelper.UserDetails.UserId);
                if (usm != null)
                {
                    vm.LocalBody = commonRepo.GetDetailsbyid(usm.LocalBodyId);
                    vm.ZoneBody = commonRepo.GetDetailsbyid(usm.ZoneId);
                    vm.WardBody = commonRepo.GetDetailsbyid(usm.WardId);
                }
                else
                {
                    TempData["UserMessage"] = "आपको अभी तक मैप नहीं किया गया है। कृपया कमांड सेंटर से संपर्क करें";
                   
                }
            }
            else
            {
                vm.LocalBodyList = commonRepo.GetLocalyBody();
            }
                return View(Views.RRTUserMapping,vm);
        }
        public ActionResult SubmitRRTMapping(FormCollection form)
        {
            try
            {
                vmUserDetails vm = new vmUserDetails();
                vm.RRTZM.UserId = Convert.ToInt32(form["UserId"]);
                vm.RRTZM.LocalBodyId = Convert.ToInt32(form["LocalBodyId"]);
                vm.RRTZM.ZoneId = Convert.ToInt32(form["ZoneId"]);
                vm.RRTZM.WardId = Convert.ToInt32(form["WardId"]);

                var us = userRepo.GetUserRRTZoneMappingByUserId(vm.RRTZM.UserId);
                if (us != null)
                {
                    TempData["UserMessage"] = "Data Already Exists";
                    return RedirectToAction("RRTUserMapping");
                }
                bool check = userRepo.AddRRTZMUser(vm.RRTZM);
                if (check == false)
                {
                    TempData["UserMessage"] = "Data not submitted something went wrong";
                    return RedirectToAction("RRTUserMapping");
                }
                TempData["UserMessage"] = "Data Submit Sucessfully";
                return RedirectToAction("RRTUserMapping");
            }
            catch (Exception ex)
            {
                TempData["UserMessage"] = "Something Went Wrong";
                return View(Views.RRTUserMapping, null);

            }
        }

        public JsonResult AutoCompleteByMoileNumber(string firstterm)
        {
            var list = userRepo.GetAutoUserNameByMobNo(firstterm).Select(l => new { Value = l.Name, Text = l.UserId });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteByMoileNumberNotRRT(string firstterm)
        {
            var list = userRepo.GetAutoUserNameByMobNoExceptRRT(firstterm).Select(l => new { Value = l.Name, Text = l.UserId });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FetchZone(int ID)
        {
            var data = commonRepo.GetZoneOrWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchWard(int ID)
        {
            
            var data = commonRepo.GetZoneOrWardByParentID(ID).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]
        public ActionResult UserTypeWiseMenu()
        {
            
            List<UserTypeMenuMaster> um = new List<UserTypeMenuMaster>();
            try
            {
                um = userRepo.GetMenuByUserType(SessionHelper.UserDetails.UserTypeId);
                return PartialView(Views.UserTypeWiseMenu, um);
            }
            catch(Exception ex)
            {
                return View(Views.Login);
            }
            
        }

        public ActionResult UserMappingWard()
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
            vmUserDetails vm = new vmUserDetails();
              
             vm.LocalBodyList = commonRepo.GetLocalyBody();
            
            return View(Views.UserMappingWard, vm);

        }
        public ActionResult SubmittedUserMappingWard(int UserId,string[] Wards)
        {
            try
            {
                vmUserDetails vm = new vmUserDetails();
                var w = Wards.ToList();
                for (int i = 0; i < w.Count(); i++)
                {
                    var uwm = userRepo.GetUserwardMappingByUserId(SessionHelper.UserDetails.UserId,Convert.ToInt32(w[i]));
                    if (uwm != null)
                    {
                        continue;
                    }
                    else
                    {
                        bool check = userRepo.AddUserMappingWard(UserId, Convert.ToInt32(w[i]));
                    }
                    
                }
                TempData["UserMessage"] = "Data Submit Sucessfully";
                return RedirectToAction("UserMappingWard");
            }
            catch (Exception ex)
            {
                TempData["UserMessage"] = "Something Went Wrong";
                return View(Views.UserMappingWard, null);

            }


        }
        public JsonResult GetWardDropDown(int LocalBodyId)
        {
            var WardList = commonRepo.GetWardByParentID(LocalBodyId).Select(l => new { Value = l.LocalBodyID, Text = l.LocalBodyName });
            return Json(WardList, JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]
        public ActionResult UserAttendance()
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
            vmUserDetails um = new vmUserDetails();
            try
            {
                um.UserATT = userRepo.GetUserAttendanceByUserIDandDate(SessionHelper.UserDetails.UserId,DateTime.Now);
               
                return View(Views.UserAttendance, um);
            }
            catch (Exception ex)
            {
                return View(Views.Login);
            }

        }
        public ActionResult SubmitUserAttendance(int UserId,string Longitude,string Latitude, string src)
        {
           
            try
            {
                ////var imageBytes = TempData["BytesImage"] as Byte[];

                if (Longitude != "" && Latitude!="" && (src!=""))
                {
                    
                    int q = userRepo.InsertUserAttendanceByUserIDandDate(SessionHelper.UserDetails.UserId, Convert.ToDouble(Longitude), Convert.ToDouble(Latitude),src,"","",0.0);
                    if (q == 1)
                    {
                        

                        TempData["UserMessage"] = "Attendance done for today";
                    }
                    return RedirectToAction("UserAttendance");
                }
                {
                    TempData["UserMessage"] = "Something went wrong";
                }
                return RedirectToAction("UserAttendance");

            }
            catch (Exception ex)
            {
                return View(Views.UserAttendance);
            }

        }
        
        public ActionResult SubmitUserAttendanceAPI(int UserId, string Longitude, string Latitude, string src,string Notes,string InOffice,double Distance)
        {

            try
            {

                if (Longitude != "" && Latitude != "" && (src != ""))
                {
                    vmUserDetails um = new vmUserDetails();
                    um.UserATT = userRepo.GetUserAttendanceByUserIDandDate(UserId, DateTime.Now);
                    if (um.UserATT == null)
                    {


                        int q = userRepo.InsertUserAttendanceByUserIDandDate(UserId, Convert.ToDouble(Longitude), Convert.ToDouble(Latitude), src,Notes,InOffice,Distance);
                        if (q == 1)
                        {
                            var result = new { Status = "true", Message = "Attendance done for today" };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var result = new { Status = "false", Message = "Attendance is already done for today" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                {
                    
                    var result = new { Status = "false", Message = "Null Data provided " };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                

            }
            catch (Exception ex)
            {
                var result = new { Status = "false", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Capture(HttpPostedFileBase Photo)
        {
            var stream = Photo.InputStream;
            

            using (var reader = new StreamReader(stream))
            {
                
                var path = "";

                string date = DateTime.Now.ToString("yyyyMMdd");
                path = Path.Combine(Server.MapPath("/WebImages/"), SessionHelper.UserDetails.Name+date+ "test.jpg");
                Photo.SaveAs(path);
                //TempData["BytesImage"] = String_To_Bytes2(dump);

                //System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));

                ViewData["path"] = SessionHelper.UserDetails.Name +date+ "test.jpg";

                Session["val"] = SessionHelper.UserDetails.Name +date+ "test.jpg";
            }

            return RedirectToAction("UserAttendance");
        }
        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }
        public JsonResult Rebind()
        {
            var scheme = Request.Url.Scheme; // will get http, https, etc.
            var host = Request.Url.Host; // will get www.mywebsite.com
            var port = Request.Url.Port; // will get the port
            var ip= scheme+"://"+host+":"+port+"/";
            string path = ip+"/WebImages/" + Session["val"].ToString();
            return Json(path, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetOfficeByUserID(int UserID)
        {
            List<mOffice> x = new List<mOffice>();
            try
            {

                 x = userRepo.GetUserOfficeByUserId(UserID);
                    var result = x.Select(z=>new {z.UserID,z.UserName,z.MobileNumber,z.Longitude,z.Latitude,z.OfficeName,z.LocalBodyName,z.ZoneName,z.WardName });
                    return Json(result, JsonRequestBehavior.AllowGet);
               


            }
            catch (Exception ex)
            {
                var result = x.Select(z => new { z.UserID, z.UserName, z.MobileNumber, z.Longitude, z.Latitude, z.OfficeName, z.LocalBodyName, z.ZoneName, z.WardName });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UserAttendanceDetails()
        {
            try
            {
                vmUserDetails vm = new vmUserDetails();
                return View(Views.UserAttendanceDetails, vm);
            }
            catch (Exception ex)
            {
                return View(Views.UserAttendanceDetails, null);
            }
        }
        public ActionResult UserAttendanceDetailsSearch(string SelectedDate,int? UserId)
        {
            try
            {

                DateTime? SD;
                if(SelectedDate!="")
                {
                    SD = Convert.ToDateTime(SelectedDate);
                }
                else
                {
                    SD = null;
                }
                
                vmUserDetails vm = new vmUserDetails();
                vm.OfList = userRepo.GetUserOfficeByUserIdandDate(SD, UserId);
                return PartialView(Views.UserAttendanceDetailsPartial, vm);
            }
            catch (Exception ex)
            {
                return PartialView(Views.UserAttendanceDetailsPartial, null);
            }
        }
    }
}