using Covid.Core.DBEntities.SeroSurve;
using Covid.Core.IRepo;
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
using Covid.Infrastructure.ViewModel.SeroSurve;
using System.Web.SessionState;

namespace Covid.Presentation.Controllers.SeroSurve
{
    [SessionState(SessionStateBehavior.Default)]
    public class SeroSurveController : Controller
    {
        ISeroSurveRepository cRepo;
        ICommonRepository commonRepo;

        public SeroSurveController(ISeroSurveRepository ICRepo, ICommonRepository common)
        {
            this.commonRepo = common;
            this.cRepo = ICRepo;
        }

        public ActionResult OpenSeroSurveForm()
        {
            vmSeroSurve vm = new vmSeroSurve();
            vm.WardList = commonRepo.GetWardByParentID(1); // for Jabalpur only
            return View(Views.SeroSurveForm, vm);
        }
       
        [HttpPost]
        [SessionExpire]
        public ActionResult AddSeroSurve(FormCollection form)
        {
            if (SessionHelper.UserDetails == null)
            {
                return View(Views.Login);
            }

            mSero SeroFormDetails = new mSero();
            String  WardName;
            int WardId;
            string[] WardValue = form["WardId"].Split('-');

            WardId = Convert.ToInt32(WardValue[0]);
            WardName = WardValue[1];

            SeroFormDetails.Name = form["Name"];
            SeroFormDetails.Address = form["Address"];
            SeroFormDetails.ParentsName = form["ParentsName"];
            SeroFormDetails.Age = Convert.ToInt32(form["Age"]);

            SeroFormDetails.Gender = Convert.ToBoolean(form["Gender"]);
            SeroFormDetails.IsSamplePossible = Convert.ToBoolean(form["IsSamplePossible"]);
            SeroFormDetails.CauseForNoSample = Convert.ToInt32(form["CauseForNoSample"]);
            SeroFormDetails.Mobile = Convert.ToInt64(form["Mobile"]);
            SeroFormDetails.Email = form["Email"];
            SeroFormDetails.HighestEdu = Convert.ToInt32(form["HighestEdu"]);
            SeroFormDetails.Occupation = form["Occupation"];
            SeroFormDetails.OccupationType = Convert.ToInt32(form["OccupationType"]);
            SeroFormDetails.IsWorkEmergency = Convert.ToBoolean(form["IsWorkEmergency"]);
            SeroFormDetails.WhoWork = Convert.ToInt32(form["WhoWork"]);
            SeroFormDetails.GovtId = form["GovtId"];
            SeroFormDetails.GovtIdType = Convert.ToInt32(form["GovtIdType"]);
            SeroFormDetails.NumberofFamily = Convert.ToInt32(form["NumberofFamily"]);

            SeroFormDetails.MaleMember = Convert.ToInt32(form["MaleMember"]);
            SeroFormDetails.FemaleMember = Convert.ToInt32(form["FemaleMember"]);
            SeroFormDetails.KidsNumber = Convert.ToInt32(form["KidsNumber"]);
            SeroFormDetails.AdlutNumber = Convert.ToInt32(form["AdlutNumber"]);
            SeroFormDetails.FamilyMontlyIncome = Convert.ToInt32(form["FamilyMontlyIncome"]);
            SeroFormDetails.IsBPL = Convert.ToBoolean(form["IsBPL"]);
            SeroFormDetails.HomeType = Convert.ToInt32(form["HomeType"]);
            SeroFormDetails.TotalRoom = Convert.ToInt32(form["TotalRoom"]);
            SeroFormDetails.TotalArea = Convert.ToInt32(form["TotalArea"]);
            SeroFormDetails.HomeArea = Convert.ToBoolean(form["HomeArea"]);
            SeroFormDetails.Batroom = Convert.ToInt32(form["Batroom"]);
            SeroFormDetails.IsDiabities = Convert.ToBoolean(form["IsDiabities"]);
            SeroFormDetails.BP = Convert.ToBoolean(form["BP"]);
            SeroFormDetails.IsCancer = Convert.ToBoolean(form["IsCancer"]);
            SeroFormDetails.IsKidney = Convert.ToBoolean(form["IsKidney"]);
            SeroFormDetails.IsHeart = Convert.ToBoolean(form["IsHeart"]);
            SeroFormDetails.IsLungs = Convert.ToBoolean(form["IsLungs"]);
            SeroFormDetails.IsLiver = Convert.ToBoolean(form["IsLiver"]);
            SeroFormDetails.IsOrgantransplant = Convert.ToBoolean(form["IsOrgantransplant"]);
            SeroFormDetails.IsDisable = Convert.ToBoolean(form["IsDisable"]);
            SeroFormDetails.IsBlood = Convert.ToBoolean(form["IsBlood"]);
            SeroFormDetails.IsPCR = Convert.ToBoolean(form["IsPCR"]);
            SeroFormDetails.IsILI = Convert.ToBoolean(form["IsILI"]);
            SeroFormDetails.IsSARI = Convert.ToBoolean(form["IsSARI"]);
            SeroFormDetails.CreatedBy =  SessionHelper.UserDetails.UserId;
            SeroFormDetails.WardId = WardId;
            SeroFormDetails.WardName = WardName;

            long id = cRepo.InsertSeroSurve(SeroFormDetails);
            TempData["msg"] = "Sero Data Added successfully ! ! ! Id for newly inserte record is " + id;

            return RedirectToAction("OpenSeroSurveForm");
        }

        [SessionExpire]
        public ActionResult OpenSeroSurveDetails()
        {
            if (SessionHelper.UserDetails == null)
            {
                return View(Views.Login);
            }
            vmSeroSurve vm = new vmSeroSurve();
            return View(Views.GetSeroSurveDetails, vm);
        }
        public ActionResult OpenSeroSurveReports()
        {
            vmSeroSurve vm = new vmSeroSurve();
            return View(Views.GetSeroSurveReports, vm);
        }


        public ActionResult GetSeroSurveDetails(string SelectedDate)
        {
            DateTime? StartDate, EndDate;
            vmSeroSurve vm = new vmSeroSurve();
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

            vm.SeroSurveList = cRepo.GetSeroSurveForUser(StartDate, EndDate, SessionHelper.UserDetails.UserId);
            if (vm.SeroSurveList == null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetSeroSurveDetailsPartial, vm);
        }

        public ActionResult GetSeroSurveReports(string SelectedDate)
        {
            DateTime? StartDate, EndDate;
            vmSeroSurve vm = new vmSeroSurve();
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

            vm.SeroSurveList = cRepo.GetSeroSurveReport(StartDate, EndDate);
            if (vm.SeroSurveList == null)
            {
                TempData["QMessage"] = "No Search Found!!";
            }
            return PartialView(Views.GetSeroSurveDetailsPartial, vm);
        }

    }
}