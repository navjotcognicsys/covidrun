using Covid.Core.Common;
using Covid.Core.DBEntities.QuarantineCheck;
using Covid.Core.DBEntities.UserDetails;
using Covid.Core.IRepo;
using Covid.Infrastructure.Helpers;
using Covid.Infrastructure.ViewModel.Quarantine;
using Covid.Presentation.Common;
using Covid.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covid.Presentation.Controllers.QuarantineCheck
{
    public class QuarantineController : Controller
    {
        IQuarantineRepository QRepo;
        ICommonRepository commonRepo;
        IPersonRepository personRepo;
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        // GET: QuarantineCheck
        public QuarantineController(IQuarantineRepository rep, ICommonRepository cRep, IPersonRepository personRepo)
        {
            this.QRepo = rep;
            this.commonRepo = cRep;
            this.personRepo = personRepo;
        }
        public ActionResult OpenQuarantineForm(int PersonId)
        {
            vmQuarantine vm = new vmQuarantine();
            var p = personRepo.GePerson(PersonId);
            vm.QuarantineDetails.PersonName = p.PersonName;
            vm.QuarantineDetails.PersonId = Convert.ToInt32(p.PersonId);
            vm.QuarantineDetails.StickerDate = p.StickerDate;
            // vm.QuarantineDetails= QRepo.GetQuarantineDetails(PersonId);
            return View(Views.QurantineCheckForm,vm);
        }

        public ActionResult OpenHomeQuarantineForm(int PersonId)
        {
            vmHomeQuarantine vm = new vmHomeQuarantine();
            var p = personRepo.GetCovidPerson(PersonId);
            vm.QuarantineDetails.PersonName = p.PatientName;
            vm.QuarantineDetails.PersonId = Convert.ToInt32(p.Id);
            return View(Views.HomeQurantineCheckForm, vm);
        }
        public ActionResult UpdateHomeQuarantineDetails(FormCollection form)
        {
            mHomeQuarantine QuarantineDetails = new mHomeQuarantine();
            QuarantineDetails.PersonName = form["Name"];
            QuarantineDetails.PersonId = Convert.ToInt32(form["PersonId"]);
            QuarantineDetails.Temp = Convert.ToDecimal(form["Temp"]);
            QuarantineDetails.IsTemp = form["Fev"] == "on" ? true : false;
            QuarantineDetails.IsBreath = form["BreathingProb"] == "on" ? true : false;
            QuarantineDetails.IsCough = form["Cuf"] == "on" ? true : false;
            QuarantineDetails.IsMedicalKit = form["IsMedicalKit"] == "on" ? true : false;
            QuarantineDetails.IsYogaInstructor = form["IsYogaInstructor"] == "on" ? true : false;
            QuarantineDetails.IsYogaIntrested = form["IsYogaIntrested"] == "on" ? true : false;
            QuarantineDetails.IsYogaDoneToday = form["IsYogaDoneToday"] == "on" ? true : false;
            QuarantineDetails.IsVideoCall = form["IsVideoCall"] == "on" ? true : false;
            QuarantineDetails.ContactWithFamilyMember = form["ContactWithFamilyMember"] == "on" ? true : false;
            QuarantineDetails.AnyFamilyMemberIssue = form["AnyFamilyMemberIssue"] == "on" ? true : false;
            QuarantineDetails.AnyProblem = form["AnyProblem"] == "on" ? true : false;
            QuarantineDetails.SPO2 = Convert.ToInt32(form["SPO2"]);
            QuarantineDetails.DCCCRemark = form["DCCCRemark"];
            QuarantineDetails.DrName = form["DrName"];
            QuarantineDetails.EPD = form["EPD"];
            QuarantineDetails.DoctorRemark = form["DoctorRemark"];
            QuarantineDetails.CreatedBy = SessionHelper.UserDetails == null ? 0 : SessionHelper.UserDetails.UserId;
            QRepo.InsertHomeQuarantineDetails(QuarantineDetails);
            return Content("OK");
        }
        //[HttpPost]
        public ActionResult UpdateQuarantineDetails(FormCollection form)
        {
            mQuarantine QuarantineDetails = new mQuarantine();
            QuarantineDetails.PersonName = form["Name"];
            QuarantineDetails.PersonId = Convert.ToInt32(form["PersonId"]);
            QuarantineDetails.StickerDate =Convert.ToDateTime( form["stickerDate"]==""?null: form["stickerDate"]);
            QuarantineDetails.Fever = form["Fever"] == "on" ? true : false;
            QuarantineDetails.Cough = form["Cough"] == "on" ? true : false;
            QuarantineDetails.BreathingProblem = form["BreathingProblem"] == "on" ? true : false;
            QuarantineDetails.NA = form["BreathingProblem"] == "on" ? true : false;
            QuarantineDetails.AnyTrouble = form["AnyTroub"] == "1" ? true : false;
            QuarantineDetails.IsSticker =form["Stick"] == "1" ? true : false;
            QuarantineDetails.AnyNeed =  form["AnyNeed"] =="True"?true:false;
            QuarantineDetails.Remark = form["Remarks"];
            QuarantineDetails.Checkedby = SessionHelper.UserDetails == null ? 0 : SessionHelper.UserDetails.UserId;
            QRepo.UpdateQuarantineDetails(QuarantineDetails);
            if((SessionHelper.UserDetails == null) || SessionHelper.UserDetails.UserType.ToUpper()==UserTypeCommon.CommandCenter.ToUpper())
            {
                
                return Content("OK");
            }
            return RedirectToAction("DashBoard","DashBoard");
        }
        [SessionExpire]
        public ActionResult GetQuarantineDetails()
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
            vmQuarantine vm = new vmQuarantine();
            return View(Views.GetQuarantineDetails,vm);
        }

        public ActionResult SearchGetQuarantineDetails(string SelectedDate)
        {
            DateTime? StartDate, EndDate;
            vmQuarantine vm = new vmQuarantine();
            if (string.IsNullOrEmpty(SelectedDate))
            {
                StartDate = null;
                EndDate = null;
            }
            else
            {
                string[] Idates = SelectedDate.Split('-');
                StartDate = Convert.ToDateTime(Idates[0]);
                EndDate = Convert.ToDateTime(Idates[1]);
            }
           // var QPL = QRepo.GetQuarantinePersonByPersonId(StartDate, EndDate).Select(x => new { x.PersonId, x.PersonName, x.MoileNo, x.Address });
            vm.QList = QRepo.GetQuarantinePersonByPersonId(StartDate, EndDate);
            if(vm.QList==null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetQuarantineDetailsPartial, vm);
        }
        [SessionExpire]
        public ActionResult GetPersonNotQuarantineDetails()
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
            vmQuarantine vm = new vmQuarantine();
            vm.PositivePersonDetailList = personRepo.GetPositivePersonDetail();
            return View(Views.GetPersonNotQuarantineDetails, vm);
        }

        public ActionResult HomeQuarantineSearch()
        {
            vmHomeQuarantine vm = new vmHomeQuarantine();
            vm.GetRefGroupList = personRepo.GetRefGroupDetail();
            return View(Views.HomeQuarantineSearch, vm);
        }

        public ActionResult SearchGetPersonNotQuarantineDetails(string SelectedDate,long PersonId)
        {
            DateTime? StartDate, EndDate;
            vmQuarantine vm = new vmQuarantine();
            if (string.IsNullOrEmpty(SelectedDate))
            {
                StartDate = null;
               // EndDate = null;
            }
            else
            {
                //string[] Idates = SelectedDate.Split('-');
                StartDate = Convert.ToDateTime(SelectedDate);
                //EndDate = Convert.ToDateTime(Idates[1]);
            }
            // var QPL = QRepo.GetQuarantinePersonByPersonId(StartDate, EndDate).Select(x => new { x.PersonId, x.PersonName, x.MoileNo, x.Address });
            vm.PList = QRepo.GetNotQuarantinePersonByPersonId(StartDate, PersonId);
            if (vm.PList == null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetPersonNotQuarantineDetailsPartial, vm);
        }

        public ActionResult SearchHomeQuarantineDetails( String RefGroup)
        {
            vmHomeQuarantine vm = new vmHomeQuarantine();
            vm.PositivePersonDetailList = QRepo.GetHomeQuarantinePersonByPersonId( RefGroup);
            if (vm.PositivePersonDetailList == null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetHomeQuarantinePartial, vm);
        }
    }
}