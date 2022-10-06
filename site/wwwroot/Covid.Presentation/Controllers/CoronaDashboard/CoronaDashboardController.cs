using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Covid.Core.Common;
using Covid.Infrastructure.Helpers;
using Covid.Core.DBEntities.CoronaUpdate;
using Covid.Infrastructure.ViewModel.CoronaDashboard;
using Covid.Presentation.Helper;
using System.Web.Mvc;
using Covid.Presentation.Common;
using Covid.Core.IRepo;

namespace Covid.Presentation.Controllers.CoronaDashboard
{
    public class CoronaDashboardController : Controller
    {
        IPublicDashboardRepository ICRepo;

        public CoronaDashboardController(IPublicDashboardRepository ICRepo)
        {
            this.ICRepo = ICRepo;
        }

        public ActionResult CoronaDashBoard()
        {
            vmCoronaDashboard dashboardList = new vmCoronaDashboard();

            dashboardList.DashboardCoroanCurentStatus = ICRepo.GetCurrentCoronaDetails();
            dashboardList.DashboardHospitalStatusList = ICRepo.GetCurrentHospitalStatus();

            return View(Views.CoronaDashboard, dashboardList);
        }

        public ActionResult GetHospitalData()
        {
            vmCoronaDashboard dashboardList = new vmCoronaDashboard();
            dashboardList.DashboardHospitalStatusList = ICRepo.GetCurrentHospitalStatus();
            return View(Views.CoronaDataUpload, dashboardList);
        }

        public ActionResult GetCoronaStatus()
        {
            vmCoronaDashboard dashboardList = new vmCoronaDashboard();
            return View(Views.CoronaStatus, dashboardList);
        }

        public ActionResult UpdateHospitalData(FormCollection form)
        {
            mHospitalStatus HospitalStatus = new mHospitalStatus();
            HospitalStatus.OccICUBed = Convert.ToInt32(form["ICU"]);
            HospitalStatus.OccIsolationBed = Convert.ToInt32(form["withouto2"]);
            HospitalStatus.OccOxygenBed = Convert.ToInt32(form["witho2"]);
            HospitalStatus.HoshId = Convert.ToInt32(form["HoshId"]);
            HospitalStatus.UpdatedDate = Convert.ToDateTime(DateTime.Now);
            ICRepo.UpdateHospitalData(HospitalStatus);

            return RedirectToAction("GetHospitalData", "CoronaDashboard");
        }

        public ActionResult CreateCoranaData(FormCollection form)
        {
            mCoronaCases CoronaStatus = new mCoronaCases();
            CoronaStatus.ActiveCases = Convert.ToInt32(form["ActiveCases"]);
            CoronaStatus.AdmitInHospital = Convert.ToInt32(form["AdmitInHospital"]);
            CoronaStatus.HomeIsolation = Convert.ToInt32(form["HomeIsolation"]);
            CoronaStatus.RecoveredCases = Convert.ToInt32(form["RecoveredCases"]);
            CoronaStatus.Deaths = Convert.ToInt32(form["Deaths"]);
            ICRepo.CreateCoranaData(CoronaStatus);
            return RedirectToAction("Dashboard", "Dashboard");
        }

        public ActionResult GetHospitalMasterData()
        {
            vmCoronaDashboard dashboardList = new vmCoronaDashboard();
            dashboardList.DashboardHospitalStatusList = ICRepo.GetCurrentHospitalStatus();
            return View(Views.CoronaHospitalMaster, dashboardList);
        }

        public ActionResult UpdateHospitalMaster(FormCollection form)
        {
            mHospitalStatus HospitalStatus = new mHospitalStatus();
            HospitalStatus.TotalICUBed = Convert.ToInt32(form["ICU"]);
            HospitalStatus.TotalIsolationBed = Convert.ToInt32(form["IsolationBed"]);
            HospitalStatus.TotalOxygenBed = Convert.ToInt32(form["OxygenBed"]);
            HospitalStatus.HoshId = Convert.ToInt32(form["HoshId"]);
            HospitalStatus.Ventilators = Convert.ToInt32(form["Ventilators"]);
            ICRepo.UpdateHospitalMasterData(HospitalStatus);

            return RedirectToAction("GetHospitalMasterData", "CoronaDashboard");
        }
    }
}