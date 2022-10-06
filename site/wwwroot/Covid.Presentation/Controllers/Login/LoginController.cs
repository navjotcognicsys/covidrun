using Covid.Core.Common;
using Covid.Core.DBEntities.UserDetails;
using Covid.Core.IRepo;
using Covid.Infrastructure.Helpers;
using Covid.Presentation.Common;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covid.Presentation.Controllers.Login
{
    public class LoginController : Controller
    {
        private ILoginRepository loginrepo;
        private ICommonRepository commonRepo;
        private IUserRepository userRepo;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public LoginController(ILoginRepository loginrepo, ICommonRepository commonRepo,IUserRepository userRepo)
        {
            this.loginrepo = loginrepo;
            this.commonRepo = commonRepo;
            this.userRepo = userRepo;
        }
        public ActionResult LogIn()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                return RedirectToAction("LoggedInAPI", new { MobileNoAPI = MobileNumber, PasswordAPI = Password });
            }
            return View(Views.Login);
        }
        public ActionResult LoggedIn(FormCollection form)
        {
            try
            {
                long MobileNo = Convert.ToInt64(form["MobileNo"]);
                string Password = Convert.ToString(form["Password"]);
               
                bool IsVaild = CheckUser(MobileNo, Password);

                if (IsVaild == true)
                {
                    UserTypeMenuMaster x = new UserTypeMenuMaster();
                    x = userRepo.GetMenuByUserType(SessionHelper.UserDetails.UserTypeId).OrderBy(a=>a.OrderBy).FirstOrDefault();
                    if (x!=null)
                    {
                        return Redirect(x.MenuURL);
                    }
                    TempData["LoginMessage"] = "No page Assign";
                    return RedirectToAction("LogIn");
                }
                TempData["LoginMessage"] = "Wrong Credentials";
                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                TempData["LoginMessage"] = "Something Went Wrong";
                return RedirectToAction("LogIn");

            }

        }
        public ActionResult LoggedInAPI(string MobileNoAPI, string PasswordAPI)
        {
            try
            {
                long MobileNo = Convert.ToInt64(MobileNoAPI);
                string Password = Convert.ToString(PasswordAPI);

                bool IsVaild = CheckUser(MobileNo, Password);

                if (IsVaild == true)
                {
                    UserTypeMenuMaster x = new UserTypeMenuMaster();
                    x = userRepo.GetMenuByUserType(SessionHelper.UserDetails.UserTypeId).OrderBy(a => a.OrderBy).FirstOrDefault();
                    if (x != null)
                    {
                        var scheme = Request.Url.Scheme; // will get http, https, etc.
                        var host = Request.Url.Host; // will get www.mywebsite.com
                        var port = Request.Url.Port; // will get the port
                        var ip = scheme + "://" + host + ":" + port + x.MenuURL;
                        var y = new { Status = "true", Message = "Login Sucessfull",RedirectUrl=ip,UserID=SessionHelper.UserDetails.UserId };
                        return Json(y, JsonRequestBehavior.AllowGet);
                    }
                   
                    var z = new { Status = "false", Message = "No page Assign", RedirectUrl = "", UserID = "" };
                    return Json(z, JsonRequestBehavior.AllowGet);
                }
                
                var result = new { Status = "false", Message = "Wrong Credentials", RedirectUrl = "", UserID = "" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
               
                var result = new { Status = "false", Message = "Something Went Wrong" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }

        }
        
        public bool CheckUser(long MobNo, string Password)
        {
            try
            {
                mUserDetails user = loginrepo.GetUserDetailByMobileNo(MobNo, Password);
                if (user != null)
                {
                    SessionHelper.UserDetails = user;
                    var ust = commonRepo.GetUserTypeByUserTypeId(user.UserTypeId);
                    if (ust != null)
                    {
                        SessionHelper.UserDetails.UserType = ust.UserType;
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult LogOut()
        {

            SessionHelper.UserDetails = null;
            return RedirectToAction("LogIn");

        }
        [SessionExpire]
        public ActionResult ChangePassword()
        {
            if (Request.QueryString.Count > 1)
            {
                MobileNumber = Request.QueryString["MobileNo"];
                Password = Request.QueryString["Password"];
                long MobileNo = Convert.ToInt64(MobileNumber);
                string Passwords = Convert.ToString(Password);
                mUserDetails user = loginrepo.GetUserDetailByMobileNo(MobileNo, Passwords);
                if (user == null)
                {
                    return View(Views.LoginAPIError);
                }
            }
            return View();
        }
        public ActionResult ChangedPassword(string NewPassword)
        {
            bool YN = loginrepo.ChangePassword(NewPassword, SessionHelper.UserDetails.MobileNo);
            if (YN == true)
            {
                TempData["LoginMessage"] = "Password Changed Sucessfully";
            }
            else
            {
                TempData["LoginMessage"] = "Something wrong went Passord not changed";
            }
            return RedirectToAction("ChangePassword");
        }
    }
}